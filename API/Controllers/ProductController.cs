﻿using BLL.Dtos;
using BLL.Dtos.Product;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger logger,
            IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        /// <summary>
        /// Create Product (base include related product)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBaseProduct([FromBody] BaseProductRequest productRequest)
        {
            _logger.Information($"POST api/product START Request: {JsonSerializer.Serialize(productRequest)}");

            Stopwatch watch = new();
            watch.Start();

            //create product
            BaseResponse<ExtendProductResponse> response = await _productService.CreateProduct(productRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information("POST api/product END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Add Related Product
        /// </summary>
        [HttpPost("related/{id}")]
        public async Task<IActionResult> AddRelatedProduct(string id, [FromBody] List<ProductRequest> relatedProductRequest)
        {
            _logger.Information($"POST api/product/related/{id} START Request: {JsonSerializer.Serialize(relatedProductRequest)}");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //create product
            BaseResponse<ProductResponse> response = await _productService.AddRelatedProduct(id, relatedProductRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"POST api/product/related/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get Base Product By Id
        /// </summary>
        [AllowAnonymous]
        [HttpGet("base/{id}")]
        public async Task<IActionResult> GetBaseProductById(string id)
        {
            _logger.Information($"GET api/product/base/{id} START");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //get base product
            BaseResponse<ExtendProductResponse> response = await _productService.GetBaseProductById(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/base/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get All Base Product
        /// </summary>
        [AllowAnonymous]
        [HttpGet("base")]
        public async Task<IActionResult> GetAllBaseProduct()
        {
            _logger.Information($"GET api/product/base START");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //get base product
            BaseResponse<List<ExtendProductResponse>> response = await _productService.GetAllBaseProduct();

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/base END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get Related Product By Id
        /// </summary>
        [AllowAnonymous]
        [HttpGet("related/{id}")]
        public async Task<IActionResult> GetRelatedProductById(string id)
        {
            _logger.Information($"GET api/product/related/{id} START");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //get related product
            BaseResponse<ProductResponse> response = await _productService.GetRelatedProductById(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/related/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Request Update Product
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> RequestUpdateProduct(string id,
            [FromBody] ProductRequest productRequest)
        {
            _logger.Information($"PUT api/product/{id} START Request: {JsonSerializer.Serialize(productRequest)}");

            Stopwatch watch = new();
            watch.Start();

            //update product
            BaseResponse<ExtendProductResponse> response = await _productService.RequestUpdateProduct(id, productRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/product/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Delete Base Product by Id
        /// </summary>
        [HttpDelete("base/{id}")]
        public async Task<IActionResult> DeleteBaseProduct(string id)
        {
            _logger.Information($"DELETE api/product/base/{id} START");

            Stopwatch watch = new();
            watch.Start();

            //delete product
            BaseResponse<ExtendProductResponse> response = await _productService.DeleteBaseProduct(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"DELETE api/product/base/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Delete Related Product by Id
        /// </summary>
        [HttpDelete("related/{id}")]
        public async Task<IActionResult> DeleteRelatedProduct(string id)
        {
            _logger.Information($"DELETE api/product/related/{id} START");

            Stopwatch watch = new();
            watch.Start();

            //delete product
            BaseResponse<ProductResponse> response = await _productService.DeleteRelatedProduct(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"DELETE api/product/related/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get Products By Status
        /// </summary>
        [AllowAnonymous]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetProductsByStatus(int status)
        {
            _logger.Information($"GET api/product/status/{status} START");

            Stopwatch watch = new();
            watch.Start();

            //get Product
            BaseResponse<List<ProductResponse>> response =
                await _productService.GetProductsByStatus(status);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/status/{status} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get Pendings Products
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProducts()
        {
            _logger.Information($"GET api/product/pending START");

            Stopwatch watch = new();
            watch.Start();

            //get Product
            BaseResponse<List<ExtendProductResponse>> response = await _productService.GetPendingProducts();

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/pending END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Approve  Product With ID
        /// </summary>
        [HttpPut("approval/{id}")]
        public async Task<IActionResult> ApproveProduct(string id)
        {
            _logger.Information($"PUT api/product/approval/{id} START");

            Stopwatch watch = new();
            watch.Start();

            //approve Product
            BaseResponse<ProductResponse> response = await _productService.VerifyProductById(id, true);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/product/approval/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Reject Product With ID
        /// </summary>
        [HttpPut("rejection/{id}")]
        public async Task<IActionResult> RejectCreateProduct(string id)
        {
            _logger.Information($"PUT api/product/rejection/{id} START");

            Stopwatch watch = new();
            watch.Start();

            //reject Product
            BaseResponse<ProductResponse> response = await _productService.VerifyProductById(id, false);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/rejection/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }
    }
}
