﻿using System;

namespace BLL.Dtos.ProductInMenu
{
    [Serializable]
    public class ProductInMenuResponse
    {
        public string ProductInMenuId { get; set; }
        public double? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }
        public string ProductId { get; set; }
        public string MenuId { get; set; }
    }
}
