using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOnline.Models
{
    public class CheckOut
    {
        public string ProductId { get; set; }
        public double Total { get; set; }
        public string DataTime { get; set; }
    }
}