using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOnline.Models
{
    public class MemberInfo
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Birth { get; set; }
        public byte Sex { get; set; }
        public string County { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public byte Edu { get; set; }
        public string Email { get; set; }
    }
}