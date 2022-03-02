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
        private const string PREFIX = "PD_";
        private const string TYPE = "Product";
        private const string CACHE_KEY = "Product";
        private const string CACHE_KEY_FOR_UPDATE = "Unverified Updated Product";


        public ProductService(IUnitOfWork unitOfWork,
            ILogger logger,
            IMapper mapper,
            IRedisService redisService,
            IUtilService utilService,
            IFirebaseService firebaseService)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _logger = logger;
            _mapper = mapper;
            _redisService = redisService;
            _utilService = utilService;
        }


        /// <summary>
        /// Create product
        /// </summary>
        /// <param name="baseProductRequest"></param>
        /// <returns></returns>
        public async Task<ExtendProductResponse> CreateProduct(BaseProductRequest baseProductRequest)
        {
            //biz rule

            //store product to database
            Product product = _mapper.Map<Product>(baseProductRequest);
            try
            {
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
                product.InverseBelongToNavigation = new Collection<Product>();

                //create related product
                foreach (ProductRequest relatedProductRequest in baseProductRequest.InverseBelongToNavigation)
                {
                    Product relatedProduct = _mapper.Map<Product>(relatedProductRequest);

                    relatedProduct.ProductId = _utilService.CreateId(PREFIX);
                    relatedProduct.Image = _firebaseService
                                        .UploadFilesToFirebase(relatedProductRequest.Image, TYPE, relatedProduct.ProductId, "Image", 0)
                                        .Result;
                    relatedProduct.Status = (int)ProductStatus.UNVERIFIED_PRODUCT;
                    relatedProduct.CreatedDate = DateTime.Now;
                    relatedProduct.UpdatedDate = DateTime.Now;
                    relatedProduct.ApproveBy = "";
                    relatedProduct.BelongTo = product.ProductId;

                    product.InverseBelongToNavigation.Add(relatedProduct);
                }

                _unitOfWork.Products.Add(product);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.CreateBaseProduct()]: " + e.Message);

                throw;
            }

            return _mapper.Map<ExtendProductResponse>(product);
        }


        /// <summary>
        /// Add Related Product
        /// </summary>
        /// <param name="productRequests"></param>
        /// <returns></returns>
        public async Task<ExtendProductResponse> AddRelatedProduct(string baseProductId,
            List<ProductRequest> productRequests)
        {
            try
            {
                productRequests.ForEach(productRequest =>
                {
                    string productId = _utilService.CreateId(PREFIX);

                    //upload image
                    string imageUrl = _firebaseService
                        .UploadFilesToFirebase(productRequest.Image, TYPE, productId, "Image", 0).Result;

                    Product product = _mapper.Map<Product>(productRequest);

                    product.ProductId = productId;
                    product.Image = imageUrl;
                    product.Status = (int)ProductStatus.UNVERIFIED_PRODUCT;
                    product.CreatedDate = DateTime.Now;
                    product.UpdatedDate = DateTime.Now;
                    product.ApproveBy = "";
                    product.BelongTo = baseProductId;

                    _unitOfWork.Products.Add(product);
                });

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.CreateRelatedProduct()]: " + e.Message);

                throw;
            }

            //create response
            var products = await GetProduct(baseProductId, Array.Empty<int?>(), default, default, default, default, default, "related");
            ExtendProductResponse productResponse = products.List.FirstOrDefault();

            return productResponse;
        }


        /// <summary>
        /// Request Update product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productRequest"></param>
        /// <returns></returns>
        public async Task<ExtendProductResponse> RequestUpdateProduct(string id, ProductRequest productRequest)
        {
            //validate id
            Product product;
            try
            {
                product = await _unitOfWork.Products.FindAsync(p => p.ProductId.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.UpdateProduct()]" + e.Message);

                throw new EntityNotFoundException(typeof(Product), id);
            }

            //get the order of the last photo
            int order = _utilService.LastImageNumber("Image", product.Image);

            //upload image
            string imageUrl = _firebaseService.UploadFilesToFirebase(productRequest.Image, TYPE, product.ProductId, "Image", order)
                                              .Result;

            UpdateProductRequest updateProductRequest = _mapper.Map<UpdateProductRequest>(productRequest);
            updateProductRequest.Image = imageUrl;

            ExtendProductResponse extendProductResponse = _mapper.Map<ExtendProductResponse>(product);
            extendProductResponse.UpdatedProduct = updateProductRequest;

            //store product to Redis
            _redisService.StoreToList(CACHE_KEY_FOR_UPDATE, extendProductResponse,
                new Predicate<ExtendProductResponse>(up => up.ProductId.Equals(extendProductResponse.ProductId)));


            return extendProductResponse;
        }


        /// <summary>
        /// Delete Product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ExtendProductResponse> DeleteProduct(string id)
        {
            //biz rule

            //validate id
            List<Product> products;
            try
            {
                products = await _unitOfWork.Products
                                .FindListAsync(p => p.ProductId.Equals(id) || p.BelongTo.Equals(id));
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.DeleteBaseProduct()]" + e.Message);

                throw new EntityNotFoundException(typeof(Product), id);
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

            //create response

            return null;
        }


        /// <summary>
        /// Verify Product By Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ProductResponse> VerifyProductById(string productId, bool isApprove)
        {
            ProductResponse productResponse;
            bool isUpdate = false;

            try
            {
                //get old product from database
                Product product = await _unitOfWork.Products.FindAsync(p => p.ProductId == productId);

                //get new product from redis
                ExtendProductResponse newProduct = _redisService.GetList<ExtendProductResponse>(CACHE_KEY_FOR_UPDATE)
                    .Find(p => p.ProductId == productId);

                if (newProduct != null)
                {
                    product = _mapper.Map<Product>(newProduct);
                    isUpdate = true;
                }

                product.UpdatedDate = DateTime.Now;
                product.Status = isApprove ? (int)ProductStatus.VERIFIED_PRODUCT : (int)ProductStatus.REJECTED_PRODUCT;
                product.ApproveBy = isApprove ? "Han" : ""; //update later

                _unitOfWork.Products.Update(product);

                await _unitOfWork.SaveChangesAsync();

                productResponse = _mapper.Map<ProductResponse>(product);
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.VerifyCreateProductById()]: " + e.Message);

                throw new EntityNotFoundException(typeof(Product), productId);
            }

            if (isUpdate)
                //remove from redis
                _redisService.DeleteFromList(CACHE_KEY_FOR_UPDATE,
                    new Predicate<ExtendProductResponse>(p => p.ProductId.Equals(productId)));

            return productResponse;
        }


        /// <summary>
        /// Get Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="apartmentId"></param>
        /// <param name="type"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="sort"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public async Task<PagingModel<ExtendProductResponse>> GetProduct(
            string id, int?[] status, string apartmentId, string type,
            int? limit, int? page,
            string sort, string include)
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
                    (id, status, apartmentId, type, limit, page, isAsc, propertyName, include);


                if (_utilService.IsNullOrEmpty(products.List))
                    throw new EntityNotFoundException(typeof(Product), "in the url");
            }
            catch (Exception e)
            {
                _logger.Error("[ProductService.GetProduct()]" + e.Message);
                throw;
            }

            return new PagingModel<ExtendProductResponse>
            {
                List = _mapper.Map<List<ExtendProductResponse>>(products.List),
                Page = products.Page,
                LastPage = products.LastPage,
                Total = products.Total,
            };
        }
    }
}
