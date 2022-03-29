﻿using AutoMapper;
using DAL.Constants;
using BLL.Dtos.Exception;
using BLL.Dtos.Product;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BLL.Dtos.ProductInMenu;

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IFirebaseService _firebaseService;
        private readonly IRedisService _redisService;
        private readonly IUtilService _utilService;
        private readonly IProductInMenuService _productInMenuService;
        private const string PREFIX = "PD_";
        private const string TYPE = "Product";
        private const string CACHE_KEY = "Product";
        private const string CACHE_KEY_FOR_UPDATE = "Unverified Updated Product";


        public ProductService(IUnitOfWork unitOfWork,
            ILogger logger,
            IMapper mapper,
            IRedisService redisService,
            IUtilService utilService,
            IProductInMenuService productInMenuService,
            IFirebaseService firebaseService)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _logger = logger;
            _mapper = mapper;
            _redisService = redisService;
            _utilService = utilService;
            _productInMenuService = productInMenuService;
        }


        /// <summary>
        /// Create product
        /// </summary>
        /// <param name="residentId"></param>
        /// <param name="baseProductRequest"></param>
        /// <returns></returns>
        public async Task<BaseProductResponse> CreateProduct(string residentId, BaseProductRequest baseProductRequest)
        {
            BaseProductResponse response;
            try
            {
                Product product = _mapper.Map<Product>(baseProductRequest);

                product.ProductId = _utilService.CreateId(PREFIX); ;
                product.Image = _firebaseService
                                        .UploadFilesToFirebase(baseProductRequest.Image, TYPE, product.ProductId, "Image", 0)
                                        .Result;
                product.Status = (int)ProductStatus.UNVERIFIED_PRODUCT;
                product.CreatedDate = DateTime.Now;
                product.UpdatedDate = DateTime.Now;
                product.IsFavorite = 0;
                product.ApproveBy = "";
                product.BelongTo = null;
                product.ResidentId = residentId;
                product.InverseBelongToNavigation = new Collection<Product>();

                //create related product
                foreach (ProductRequest relatedProductRequest in baseProductRequest.RelatedProducts)
                {
                    Product relatedProduct = _mapper.Map<Product>(relatedProductRequest);

                    relatedProduct.ProductId = _utilService.CreateId(PREFIX);
                    relatedProduct.Image = "";
                    relatedProduct.Status = (int)ProductStatus.UNVERIFIED_PRODUCT;
                    relatedProduct.CreatedDate = DateTime.Now;
                    relatedProduct.UpdatedDate = DateTime.Now;
                    relatedProduct.ApproveBy = "";
                    relatedProduct.IsFavorite = 0;
                    relatedProduct.ResidentId = residentId;
                    relatedProduct.BelongTo = product.ProductId;

                    product.InverseBelongToNavigation.Add(relatedProduct);
                }

                _unitOfWork.Products.Add(product);

                //get base menu Id
                string baseMenu = await _unitOfWork.Menus.GetBaseMenuId(residentId);

                //store product into base menu
                await _productInMenuService.AddProductsToMenu(baseMenu, new List<ProductInMenuRequest>()
                    { new ProductInMenuRequest
                        {
                            ProductId = product.ProductId,
                            Price = product.DefaultPrice
                        }
                    }
                 );

                response = _mapper.Map<BaseProductResponse>(product);
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.CreateBaseProduct()]: " + e.Message);
                throw;
            }

            return response;
        }


        /// <summary>
        /// Add Related Product
        /// </summary>
        /// <param name="baseProductId"></param>
        /// <param name="residentId"></param>
        /// <param name="productRequests"></param>
        /// <returns></returns>
        public async Task AddRelatedProduct(string baseProductId, string residentId,
            List<ProductRequest> productRequests)
        {
            try
            {
                //get base product
                Product baseProduct = await _unitOfWork.Products.FindAsync(p => p.ProductId.Equals(baseProductId));

                productRequests.ForEach(productRequest =>
                {
                    string productId = _utilService.CreateId(PREFIX);

                    Product product = _mapper.Map<Product>(productRequest);

                    product.ProductId = productId;
                    product.Image = "";
                    product.Status = (int)ProductStatus.UNVERIFIED_PRODUCT;
                    product.CreatedDate = DateTime.Now;
                    product.UpdatedDate = DateTime.Now;
                    product.ApproveBy = "";
                    product.ResidentId = residentId;
                    product.BelongTo = baseProductId;

                    _unitOfWork.Products.Add(product);
                });

                baseProduct.Status = (int)ProductStatus.UNVERIFIED_PRODUCT;
                _unitOfWork.Products.Update(baseProduct);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.CreateRelatedProduct()]: " + e.Message);
                throw;
            }
        }


        /// <summary>
        /// Update product
        /// </summary>
        /// <param name="productRequest"></param>
        /// <returns></returns>
        public async Task UpdateProduct(UpdateProductRequest productRequest)
        {

            try
            {
                //get Id of updated product
                List<string> productIds = productRequest.Products.Select(pr => pr.ProductId).ToList();

                //validate ids
                List<Product> products = await _unitOfWork.Products.FindListAsync(p => productIds.Contains(p.ProductId));

                foreach (var pR in productRequest.Products)
                {
                    //get product from database
                    Product product = products.Where(p => p.ProductId.Equals(pR.ProductId)).FirstOrDefault();

                    //store current product to Redis
                    ProductResponse currentProduct = _mapper.Map<ProductResponse>(product);
                    _redisService.StoreToList(CACHE_KEY_FOR_UPDATE, currentProduct,
                        new Predicate<ProductResponse>(up => up.ProductId.Equals(currentProduct.ProductId)));

                    //get the order of the last photo
                    int order = !string.IsNullOrEmpty(product.Image) ? _utilService.LastImageNumber("Image", product.Image) : 0;

                    //upload new image & remove image
                    string imageUrl = product.Image;
                    if (pR.Image.Length > 0)
                    {
                        foreach (var image in pR.Image)
                        {
                            if (image.Contains("https://firebasestorage.googleapis.com/"))
                                imageUrl = imageUrl.Replace(image + "|", "");
                            else
                                imageUrl += _firebaseService
                                    .UploadFilesToFirebase(new string[] { image }, TYPE, product.ProductId, "Image", order).Result;
                        }
                    }
                    pR.Image = null;
                    pR.ProductId = null;

                    product = _mapper.Map<ExtendProductRequest, Product>(pR, product);
                    product.Image = imageUrl;
                    product.ApproveBy = "";
                    product.UpdatedDate = DateTime.Now;
                    product.Status = (int)ProductStatus.UNVERIFIED_PRODUCT;

                    _unitOfWork.Products.Update(product);
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.UpdateProduct()]" + e.Message);
                throw;
            }
        }


        /// <summary>
        /// Delete Product by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteProduct(List<string> ids)
        {
            //biz rule

            //validate id
            List<Product> products;
            try
            {
                products = await _unitOfWork.Products
                                .FindListAsync(p => ids.Contains(p.ProductId) || ids.Contains(p.BelongTo));
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.DeleteProduct()]" + e.Message);

                throw new EntityNotFoundException();
            }

            //delete product
            try
            {
                products.ForEach(product =>
                {
                    product.Status = (int)ProductStatus.DELETED_PRODUCT;
                    product.UpdatedDate = DateTime.Now;
                    product.ApproveBy = "";

                    _unitOfWork.Products.Update(product);
                });

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.DeleteBaseProduct()]" + e.Message);

                throw;
            }

        }


        /// <summary>
        /// Verify Product By Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<BaseProductResponse> VerifyProductById(string productId, bool isApprove, string residentId)
        {
            BaseProductResponse productResponse;

            try
            {
                //get old product from database
                Product baseProduct =
                    (await _unitOfWork.Products
                        .GetProduct(id: productId, include: new string[] { "related" }))
                        .List
                        .First();

                if (isApprove)
                {
                    //get base product update if available
                    ProductResponse newBaseProduct = _redisService.GetList<ProductResponse>(CACHE_KEY_FOR_UPDATE)
                            .Find(p => p.ProductId == productId);
                    if (newBaseProduct != null)
                    {
                        newBaseProduct.ProductId = null;
                        baseProduct = _mapper.Map<Product>(newBaseProduct);
                        _redisService.DeleteFromList(CACHE_KEY_FOR_UPDATE,
                            new Predicate<ProductResponse>(p => p.ProductId.Equals(baseProduct.ProductId)));
                    }

                    //get related product update if available
                    for (int i = 0; i < baseProduct.InverseBelongToNavigation.Count; i++)
                    {
                        Product relatedProduct = baseProduct.InverseBelongToNavigation.ElementAt(i);
                        baseProduct.InverseBelongToNavigation.Remove(relatedProduct);

                        ProductResponse newRelatedProduct = _redisService.GetList<ProductResponse>(CACHE_KEY_FOR_UPDATE)
                                .Find(p => p.ProductId == relatedProduct.ProductId);

                        if (newRelatedProduct != null)
                        {
                            newRelatedProduct.ProductId = null;
                            relatedProduct = _mapper.Map<Product>(newRelatedProduct);
                            _redisService.DeleteFromList(CACHE_KEY_FOR_UPDATE,
                                new Predicate<ProductResponse>(p => p.ProductId.Equals(relatedProduct.ProductId)));
                        }

                        baseProduct.InverseBelongToNavigation.Add(relatedProduct);
                    }
                }

                //verify product
                baseProduct.UpdatedDate = DateTime.Now;
                baseProduct.Status = isApprove ? (int)ProductStatus.VERIFIED_PRODUCT : (int)ProductStatus.REJECTED_PRODUCT;
                baseProduct.ApproveBy = isApprove ? residentId : "";


                //verify related product
                for (int i = 0; i < baseProduct.InverseBelongToNavigation.Count; i++)
                {
                    Product relatedProduct = baseProduct.InverseBelongToNavigation.ElementAt(i);
                    baseProduct.InverseBelongToNavigation.Remove(relatedProduct);

                    relatedProduct.UpdatedDate = DateTime.Now;
                    relatedProduct.Status = isApprove ? (int)ProductStatus.VERIFIED_PRODUCT : (int)ProductStatus.REJECTED_PRODUCT;
                    relatedProduct.ApproveBy = isApprove ? residentId : "";

                    baseProduct.InverseBelongToNavigation.Add(relatedProduct);
                }

                _unitOfWork.Products.Update(baseProduct);

                await _unitOfWork.SaveChangesAsync();

                productResponse = _mapper.Map<BaseProductResponse>(baseProduct);
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.VerifyCreateProductById()]: " + e.Message);
                throw;
            }

            return productResponse;
        }


        /// <summary>
        /// Get Product
        /// </summary>
        /// <param name="role"></param>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="apartmentId"></param>
        /// <param name="sysCateId"></param>
        /// <param name="search"></param>
        /// <param name="limit"></param>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="sort"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public async Task<PagingModel<BaseProductResponse>> GetProduct(
            string role, string id = default, int?[] status = default, string apartmentId = default,
            string sysCateId = default, string search = default, int? limit = default, int? page = default,
            string sort = default, string[] include = default)
        {
            PagingModel<Product> products;
            string propertyName = default;
            bool isAsc = false;

            if (!string.IsNullOrEmpty(sort))
            {
                isAsc = sort[0].ToString().Equals("+");
                propertyName = _utilService.UpperCaseFirstLetter(sort[1..]);
            }

            try
            {
                products = await _unitOfWork.Products.GetProduct
                    (id, status, apartmentId, sysCateId, search, limit, page, isAsc, propertyName, include);
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.GetProduct()]" + e.Message);
                throw;
            }

            //get new products if update
            List<BaseProductResponse> responses = _mapper.Map<List<BaseProductResponse>>(products.List);

            if (status.Contains((int)ProductStatus.UNVERIFIED_PRODUCT))
            {
                foreach (var response in responses)
                {
                    //get new base product
                    response.CurrentProduct = _redisService
                            .GetList<ProductResponse>(CACHE_KEY_FOR_UPDATE)
                            .FirstOrDefault(p => p.ProductId == response.ProductId);

                    //get new related product
                    foreach (var related in response.RelatedProducts)
                    {
                        related.CurrentProduct = _redisService
                                .GetList<ProductResponse>(CACHE_KEY_FOR_UPDATE)
                                .FirstOrDefault(p => p.ProductId == related.ProductId);
                    }
                }
            }

            return new PagingModel<BaseProductResponse>
            {
                List = responses,
                Page = products.Page,
                LastPage = products.LastPage,
                Total = products.Total,
            };
        }


        /// <summary>
        /// Get Product For Customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="apartmentId"></param>
        /// <param name="sysCateId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagingModel<BaseProductResponse>> GetProductForCustomer(
            string id, string apartmentId, string sysCateId, string search)
        {
            List<BaseProductResponse> responses = new List<BaseProductResponse>();
            PagingModel<Product> productsPaging;
            TimeZoneInfo vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vnTime = TimeZoneInfo.ConvertTime(DateTime.Now, vnZone);

            try
            {
                //get product from data base
                productsPaging = await _unitOfWork.Products.GetProduct
                    (id: id, status: new int?[] { (int)ProductStatus.VERIFIED_PRODUCT },
                    apartmentId: apartmentId, categoryId: sysCateId, search: search,
                    include: new string[] { "related", "menu" });

                //get price for product
                foreach (Product product in productsPaging.List)
                {
                    foreach (ProductInMenu pim in product.ProductInMenus)
                    {
                        //check if menu available now
                        if (TimeSpan.Compare(vnTime.TimeOfDay, (TimeSpan)pim.Menu.TimeStart) > 0 &&
                            TimeSpan.Compare(vnTime.TimeOfDay, (TimeSpan)pim.Menu.TimeEnd) < 0 &&
                            pim.Menu.RepeatDate.Contains(((int)vnTime.DayOfWeek).ToString()))
                        {
                            BaseProductResponse response = responses.Where(p => p.ProductId.Equals(product.ProductId))
                                .FirstOrDefault();
                            //if responses has data and that is price of base menu then update it
                            if (response != null && !(bool)pim.Menu.BaseMenu)
                            {
                                int index = responses.IndexOf(response);
                                response.DefaultPrice = pim.Price;
                                responses[index] = response;
                            }
                            else
                            {
                                response = _mapper.Map<BaseProductResponse>(product);
                                response.DefaultPrice = pim.Price;
                                responses.Add(response);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.GetProductForCustomer()]: " + e.Message);
                throw;
            }
            return new PagingModel<BaseProductResponse>
            {
                List = responses,
                Page = productsPaging.Page,
                LastPage = productsPaging.LastPage,
                Total = responses.Count,
            };
        }
    }
}
