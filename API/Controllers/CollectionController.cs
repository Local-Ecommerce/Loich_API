﻿using BLL.Dtos;
using BLL.Dtos.Collection;
using BLL.Dtos.CollectionMapping;
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
    [Route("api/collection")]
    public class CollectionController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICollectionService _collectionService;

        public CollectionController(ILogger logger,
            ICollectionService collectionService)
        {
            _logger = logger;
            _collectionService = collectionService;
        }

        /// <summary>
        /// Create Collection
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCollection([FromBody] CollectionRequest collectionRequest)
        {
            _logger.Information($"POST api/collection/create START Request: " +
                $"{JsonSerializer.Serialize(collectionRequest)}");

            Stopwatch watch = new();
            watch.Start();

            //create Collection
            BaseResponse<CollectionResponse> response = await _collectionService.CreateCollection(collectionRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information("POST api/collection/create END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }



        /// <summary>
        /// Get Collection By Id
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollectionById(string id)
        {
            _logger.Information($"GET api/collection/{id} START");

            Stopwatch watch = new();
            watch.Start();

            //get Collection
            BaseResponse<CollectionResponse> response = await _collectionService.GetCollectionById(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/collection/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get All Collections
        /// </summary>
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCollections()
        {
            _logger.Information($"GET api/collection/all START");

            Stopwatch watch = new();
            watch.Start();

            //get Collection
            BaseResponse<List<CollectionResponse>> response = await _collectionService.GetAllCollections();

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"GET api/collection/all END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Update Collection
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCollectionById(string id,
                                              [FromBody] CollectionUpdateRequest collectionRequest)
        {
            _logger.Information($"PUT api/collection/{id} START Request: " +
                $"{JsonSerializer.Serialize(collectionRequest)}");

            Stopwatch watch = new();
            watch.Start();

            //update Collection
            BaseResponse<CollectionResponse> response = await _collectionService.UpdateCollectionById(id, collectionRequest);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/collection/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Delete collection
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection(string id)
        {
            _logger.Information($"DELETE api/collection/{id} START");

            Stopwatch watch = new();
            watch.Start();

            //delete Collection
            BaseResponse<CollectionResponse> response = await _collectionService.DeleteCollection(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"DELETE api/collection/{id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Add Product To Collection
        /// </summary>
        [HttpPost("{collectionId}/products")]
        public async Task<IActionResult> AddProductToCollection(string collectionId, [FromBody] string[] productIds)
        {
            _logger.Information($"POST api/collection/{collectionId}/products START Request: " +
                $"{JsonSerializer.Serialize(productIds)}");

            Stopwatch watch = new();
            watch.Start();

            //add product to Collection
            BaseResponse<List<CollectionMappingResponse>> response = await _collectionService
                .AddProductsToCollection(collectionId, productIds);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"POST api/collection/{collectionId}/products END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get Products By Collection Id
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductsByCollectionId(string id)
        {
            _logger.Information($"PUT api/collection/{id}/products START");

            Stopwatch watch = new();
            watch.Start();

            //get products
            BaseResponse<List<ExtendProductResponse>> response = await _collectionService.GetProductsByCollectionId(id);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/collection/{id}/products END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Update Product Status In Collection
        /// </summary>
        [HttpPut("{collectionId}/{productId}/{status}")]
        public async Task<IActionResult> UpdateProductStatusInCollection(
            string collectionId, string productId, int status)
        {
            _logger.Information($"PUT api/collection/{collectionId}/{productId}/{status} START Request: " +
                $"{JsonSerializer.Serialize(status)}");

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //update product status in collection
            BaseResponse<CollectionMappingResponse> response = 
                await _collectionService.UpdateProductStatusInCollection(collectionId, productId, status);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"PUT api/collection/{collectionId}/{productId}/{status} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Remove Product From Collection
        /// </summary>
        [HttpDelete("{collectionId}/{productId}")]
        public async Task<IActionResult> RemoveProductFromCollection(string collectionId, string productId)
        {
            _logger.Information($"DELETE api/collection/{collectionId}/{productId} START");

            Stopwatch watch = new();
            watch.Start();

            //update product status in collection
            BaseResponse<CollectionMappingResponse> response =
                await _collectionService.RemoveProductFromCollection(collectionId, productId);

            string json = JsonSerializer.Serialize(response);

            watch.Stop();

            _logger.Information($"DELETE api/collection/{collectionId}/{productId} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }
    }
}
