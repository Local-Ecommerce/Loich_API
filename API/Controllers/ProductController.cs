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
        /// Create Base Product
        /// </summary>
        [HttpPost("create-base")]
        public async Task<IActionResult> CreateBaseProduct([FromForm] ProductRequest productRequest)
        {
            _logger.Information($"POST api/product/create-base START Request: {JsonSerializer.Serialize(productRequest)}");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //create product
            BaseResponse<BaseProductResponse> response = await _productService.CreateBaseProduct(productRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information("POST api/product/create-base END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Create Related Product
        /// </summary>
        [HttpPost("create-related/{id}")]
        public async Task<IActionResult> CreateRelatedProduct(string id, [FromForm] RelatedProductRequest relatedProductRequest)
        {
            _logger.Information($"POST api/product/create-related/{id} START Request: {JsonSerializer.Serialize(relatedProductRequest)}");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //create product
            BaseResponse<BaseProductResponse> response = await _productService.CreateRelatedProduct(id, relatedProductRequest.productRequests);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"POST api/product/create-related/{id} END duration: " +
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
            BaseResponse<BaseProductResponse> response = await _productService.GetBaseProductById(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/base/{id} END duration: " +
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
        /// Get Related Product By Base Product Id
        /// </summary>
        [AllowAnonymous]
        [HttpGet("related/base={id}")]
        public async Task<IActionResult> GetRelatedProductsByBaseProductId(string id)
        {
            _logger.Information($"GET api/product/related/base={id} START");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //get related product
            BaseResponse<List<ProductResponse>> response = await _productService.GetRelatedProductsByBaseProductId(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/related/base={id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }

        /// <summary>
        /// Update Base Product
        /// </summary>
        [HttpPut("base/{id}")]
        public async Task<IActionResult> UpdateBaseProduct(string id,
            [FromForm] ProductRequest productRequest)
        {
            _logger.Information($"PUT api/product/base/{id} START Request: {JsonSerializer.Serialize(productRequest)}");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //update base product
            BaseResponse<BaseProductResponse> response = await _productService.UpdateBaseProduct(id, productRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/product/base/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Update Related Product
        /// </summary>
        [HttpPut("related/{id}")]
        public async Task<IActionResult> UpdateRelatedProduct(string id,
            [FromForm] ProductRequest productRequest)
        {
            _logger.Information($"PUT api/product/related/{id} START Request: {JsonSerializer.Serialize(productRequest)}");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //update base product
            BaseResponse<ProductResponse> response = await _productService.UpdateRelatedProduct(id, productRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/product/related/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }

        /// <summary>
        /// Delete Base Product by Id
        /// </summary>
        [HttpPut("delete/base/{id}")]
        public async Task<IActionResult> DeleteBaseProduct(string id)
        {
            _logger.Information($"PUT api/product/delete/base/{id} START");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //delete product
            BaseResponse<BaseProductResponse> response = await _productService.DeleteBaseProduct(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/product/delete/base/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }

        /// <summary>
        /// Delete Related Product by Id
        /// </summary>
        [HttpPut("delete/related/{id}")]
        public async Task<IActionResult> DeleteRelatedProduct(string id)
        {
            _logger.Information($"PUT api/product/delete/related/{id} START");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //delete product
            BaseResponse<ProductResponse> response = await _productService.DeleteRelatedProduct(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/product/delete/related/{id} END duration: " +
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
        /// Get Products By Product Type
        /// </summary>
        [AllowAnonymous]
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetProductsByProductType(string type)
        {
            _logger.Information($"GET api/product/status/{type} START");

            Stopwatch watch = new();
            watch.Start();

            //get Product
            BaseResponse<List<ProductResponse>> response =
                await _productService.GetProductsByProductType(type);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/product/status/{type} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }
    }
}
