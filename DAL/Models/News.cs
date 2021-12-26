﻿using System;
using System.Collections.Generic;

#nullable disable

namespace DAL.Models
{
    public partial class News
    {
        public string NewsId { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Text { get; set; }
        public int? Status { get; set; }
        public string MarketManagerId { get; set; }
        public string ApartmentId { get; set; }

        public virtual Apartment Apartment { get; set; }
        public virtual MarketManager MarketManager { get; set; }
    }
}