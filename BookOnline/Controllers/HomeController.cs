using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace BookOnline.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string path = Server.MapPath("App_Data/VisitCount.txt");

            string nn = System.IO.File.ReadAllText(System.IO.Path.GetFullPath(path));
            nn = (int.Parse(nn) + 1).ToString();

            System.IO.File.WriteAllText(path, nn);

            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= 8 - nn.Length; i++)
                sb.Append("<img src='/images/num2/0.gif' />");
            for (int i = 0; i < nn.Length; i++)
                sb.Append(string.Format("<img src='/images/num2/{0}.gif' />", nn.Substring(i, 1)));

            ViewData["VisitCount"] = sb.ToString();

            string bgPath = Server.MapPath("images/BG");
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.IO.Path.GetFullPath(bgPath));
            System.IO.FileInfo[] files = di.GetFiles();
            Random rr = new Random();
            ViewData["BG"] = files[rr.Next(files.Length)].Name;

            return View();
        }

    }
}
