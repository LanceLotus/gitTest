using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOnline.Models
{
    public class BookInfo
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public double? Price { get; set; }
        public string Times { get; set; }
    }
}