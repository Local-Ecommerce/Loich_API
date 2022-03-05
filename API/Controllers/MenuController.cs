﻿using DAL.Constants;
using BLL.Dtos;
using BLL.Dtos.Menu;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMenuService _menuService;

        public MenuController(ILogger logger, IMenuService menuService)
        {
            _logger = logger;
            _menuService = menuService;
        }

        /// <summary>
        /// Create menu (Merchant)
        /// </summary>
        [Authorize(Roles = ResidentType.MERCHANT)]
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] MenuRequest menuRequest)
        {
            _logger.Information($"POST api/menus START Request: " +
                $"{JsonSerializer.Serialize(menuRequest)}");

            Stopwatch watch = new();
            watch.Start();

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claim = identity.Claims;

            //get resident id from token
            string claimName = claim.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault().ToString();
            string residentId = claimName.Substring(claimName.LastIndexOf(':') + 2);

            //Create Menu
            MenuResponse response = await _menuService.CreateMenu(residentId, menuRequest);

            string json = JsonSerializer.Serialize(ApiResponse<MenuResponse>.Success(response));

            watch.Stop();

            _logger.Information("POST api/menus END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Get menu (Authentication required)
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMenu(
            [FromQuery] string id,
            [FromQuery] int?[] status,
            [FromQuery] string apartmentid,
            [FromQuery] int? limit,
            [FromQuery] int? page,
            [FromQuery] string sort,
            [FromQuery] string include)
        {
            _logger.Information($"GET api/menus?id={id}&status=" + string.Join("status=", status) +
                $"&apartmentid={apartmentid}&limit={limit}&page={page}&sort={sort}&include={include} START");

            Stopwatch watch = new();
            watch.Start();

            //get Menu
            object responses = await _menuService.GetMenu(id, status, apartmentid, limit, page, sort, include);

            string json = JsonSerializer.Serialize(ApiResponse<object>.Success(responses));

            watch.Stop();

            _logger.Information($"GET api/menus?id={id}&status=" + string.Join("status=", status) +
                $"&apartmentid={apartmentid}&limit={limit}&page={page}&sort={sort}&include={include} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Update menu (Merchant)
        /// </summary>
        [Authorize(Roles = ResidentType.MERCHANT)]
        [HttpPut]
        public async Task<IActionResult> UpdateMenuById([FromQuery] string id, [FromBody] MenuUpdateRequest menuRequest)
        {
            _logger.Information($"PUT api/menus?id={id} START Request: " +
                $"{JsonSerializer.Serialize(menuRequest)}");

            Stopwatch watch = new();
            watch.Start();

            //Update Menu
            MenuResponse response = await _menuService.UpdateMenuById(id, menuRequest);

            string json = JsonSerializer.Serialize(ApiResponse<MenuResponse>.Success(response));

            watch.Stop();

            _logger.Information($"PUT api/menus?id={id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }


        /// <summary>
        /// Delete menu (Merchant)
        /// </summary>
        [Authorize(Roles = ResidentType.MERCHANT)]
        [HttpDelete]
        public async Task<IActionResult> DeleteMenuById([FromQuery] string id)
        {
            _logger.Information($"DELETE api/menus?id={id} START");

            Stopwatch watch = new();
            watch.Start();

            //Delete Menu
            MenuResponse response = await _menuService.DeleteMenuById(id);

            string json = JsonSerializer.Serialize(ApiResponse<MenuResponse>.Success(response));

            watch.Stop();

            _logger.Information($"DELETE api/menus?id={id} END duration: " +
                $"{watch.ElapsedMilliseconds} ms -----------Response: " + json);

            return Ok(json);
        }
    }
}
