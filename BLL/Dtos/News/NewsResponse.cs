﻿using BLL.Dtos.Apartment;
using BLL.Dtos.MarketManager;
using System;

namespace BLL.Dtos.News
{
    public class NewsResponse
    {
        public string NewsId { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int? Status { get; set; }
        public string MarketManagerId { get; set; }
        public string ApartmentId { get; set; }
        public MarketManagerResponse MarketManager { get; set; }
        public ApartmentResponse Apartment { get; set; }
    }
}
