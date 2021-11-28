﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BLL.Dtos.Product
{
    public class ProductRequest
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public double DefaultPrice { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public double Weight { get; set; }
        public List<IFormFile> Image { get; set; }
    }
}
