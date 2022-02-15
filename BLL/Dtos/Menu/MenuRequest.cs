﻿using System;

namespace BLL.Dtos.Menu
{
    [Serializable]
    public class MenuRequest
    {
        public string MenuName { get; set; }
        public string MenuDescription { get; set; }
        public string ResidentId { get; set; }
    }
}
