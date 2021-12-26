﻿using System;
using System.Collections.Generic;

namespace BLL.Dtos.Product
{
    public class BaseProductResponse
    {
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public double DefaultPrice { get; set; }
        public string Image { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ApproveBy { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public double Weight { get; set; }
        public List<ProductResponse> RelatedProducts { get; set; }
    }
}