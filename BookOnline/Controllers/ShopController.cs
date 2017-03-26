using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BookOnline.Controllers
{
    public class ShopController : Controller
    {
        public ActionResult Main(int? p)
        {
            int currentPage = 1;
            if (p != null)
                currentPage = p.Value;

            List<Models.BookInfo> result = null;

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234;");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "產品分頁瀏覽";
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@page", SqlDbType.Int).Value = currentPage;
            cmd.Parameters.Add("@total", SqlDbType.Int);
            cmd.Parameters["@page"].Direction = ParameterDirection.Input;
            cmd.Parameters["@total"].Direction = ParameterDirection.Output;

            cnn.Open();
            SqlDataReader mydr = cmd.ExecuteReader();
            if (mydr.HasRows)
            {
                result = new List<Models.BookInfo>();
                while (mydr.Read())
                {
                    Models.BookInfo bi = new Models.BookInfo();
                    bi.Id = Convert.ToInt32(mydr["id"]);
                    bi.Title = Convert.ToString(mydr["title"]);
                    bi.Author = Convert.ToString(mydr["author"]);
                    bi.Price = Convert.ToDouble(mydr["price"]);
                    bi.Times = Convert.ToString(mydr["times"]);
                    result.Add(bi);
                }
            }
            mydr.Close();

            int total = (int)cmd.Parameters["@total"].Value;

            cmd.Dispose();
            cnn.Close();

            int totalPage = (int)Math.Ceiling(total / 10.0);
            StringBuilder sb = new StringBuilder();
            UrlHelper uu = new UrlHelper(this.ControllerContext.RequestContext);
            for (int i = 1; i <= totalPage; i++)
            {
                if (i == currentPage)
                    sb.Append(i + "&nbsp;&nbsp;");
                else
                {
                    string url = uu.Action("Main", "Shop", new System.Web.Routing.RouteValueDictionary { { "p", i } });
                    sb.Append(string.Format("<a href='{0}'>{1}</a>&nbsp;&nbsp;", url, i));
                }
            }
            ViewData["PageLink"] = sb.ToString();

            return View(result);

        }

        public string AddToCart(int? pid)
        {
            HashSet<Models.CartItem> myCart = null;
            if (Session["MyCart"] == null)
                myCart = new HashSet<Models.CartItem>();
            else
                myCart = Session["MyCart"] as HashSet<Models.CartItem>;

            Models.CartItem ci = new Models.CartItem();
            ci.Id = pid.Value;

            if (!myCart.Contains(ci))
            {
                SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
                SqlCommand cmd = new SqlCommand("SELECT id,title,price FROM Book WHERE id=@pid", cnn);
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = pid.Value;
                cnn.Open();

                SqlDataReader mydr = cmd.ExecuteReader();
                mydr.Read();

                ci.Title = Convert.ToString(mydr["Title"]);
                ci.Price = Convert.ToDouble(mydr["Price"]);
                ci.Quantity = 1;

                mydr.Close();
                cmd.Dispose();
                cnn.Close();

                myCart.Add(ci);
                Session["MyCart"] = myCart;
            }

            StringBuilder sb = new StringBuilder();

            double total = 0;
            foreach (Models.CartItem ii in myCart)
            {
                total += ii.SubTotal;
            }

            sb.Append("<p style='text-align:center;line-height:16pt;margin-top:60px;'><span style='font-weight:bold;color:red;font-size:16pt'>NT$").Append(total).Append("</span><br />");
            sb.Append("<span style='text-decoration:underline;color:blue;'>(").Append(myCart.Count).Append(")&nbsp;結帳</span>");

            return sb.ToString();

        }

        [Models.IsLogin_Filter]
        public ActionResult ShoppingCart()
        {
            if (Session["MyCart"] == null)
                return RedirectToAction("Main");

            HashSet<Models.CartItem> myCart = (HashSet<Models.CartItem>)Session["MyCart"];

            int amount = 0, shipping = 0;
            double total = 0;

            foreach (Models.CartItem ii in myCart)
            {
                total += ii.SubTotal;
            }

            if (total < 5000)
                shipping = 70;
            else if (total >= 5000)
                shipping = 0;

            amount = (int)Math.Round(total, MidpointRounding.AwayFromZero) + shipping;

            ViewData["Shipping"] = shipping;
            ViewData["Total"] = total;
            ViewData["Amount"] = amount;
            Session["Total"] = total;

            return View(myCart);
        }

        public ActionResult ClearCart()
        {
            Session["MyCart"] = null;
            Session.Remove("MyCart");
            return RedirectToAction("Main");
        }

        public ActionResult UpdateCart()
        {
            if (Session["MyCart"] == null)
                return RedirectToAction("Main");

            HashSet<Models.CartItem> myCart = (HashSet<Models.CartItem>)Session["MyCart"];

            foreach (Models.CartItem ci in myCart)
            {
                ci.Quantity = int.Parse(Request.Form["QQ_" + ci.Id]);
            }
            Session["MyCart"] = myCart;

            return RedirectToAction("ShoppingCart");
        }

        public ActionResult CheckOut(HashSet<Models.CartItem> myCart)
        {
            if (Session["MyCart"] == null)
                return RedirectToAction("Main");

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLAdmin;PWD=1234");
            SqlCommand cmd = new SqlCommand("INSERT INTO 訂單 (金額,訂單時間) VALUES(@total,@dateTime)", cnn);

            double total = 0;

            total = Double.Parse(Session["Total"].ToString());

            cmd.Parameters.Add("@total", SqlDbType.Decimal).Value = total;
            DateTime dt = DateTime.Now;
            cmd.Parameters.Add("@dateTime", SqlDbType.DateTime).Value = dt;

            cnn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cnn.Close();

            SqlConnection cnn2 = new SqlConnection("server=localhost;database=WebData;UID=SQLUSer;PWD=1234");
            SqlCommand cmd2 = new SqlCommand("SELECT 訂單編號 FROM 訂單 WHERE 訂單時間=@dateTime2", cnn2);
            cmd2.Parameters.Add("@dateTime2", SqlDbType.DateTime).Value = dt;

            cnn2.Open();
            SqlDataReader mydr = cmd2.ExecuteReader();

            if (mydr.HasRows)
            {
                mydr.Read();
                Session["detailId"] = mydr["訂單編號"].ToString();
            }
            mydr.Close();
            cmd2.Dispose();
            cnn2.Close();

            return RedirectToAction("ShopDetail");
        }

        public ActionResult ShopDetail()
        {
            HashSet<Models.CheckOut> myCheck = new HashSet<Models.CheckOut>();

            if (Session["detailId"] == null)
                return RedirectToAction("Main");

            Models.CheckOut co = new Models.CheckOut();

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
            SqlCommand cmd = new SqlCommand("SELECT 訂單編號,金額,訂單時間 FROM 訂單 WHERE 訂單編號=@detailId", cnn);
            cmd.Parameters.Add("@detailId", SqlDbType.Char).Value = Session["detailId"];

            cnn.Open();
            SqlDataReader mydr = cmd.ExecuteReader();

            mydr.Read();
            co.ProductId = Convert.ToString(Session["detailId"]);
            co.Total = Convert.ToDouble(mydr["金額"]);
            co.DataTime = Convert.ToString(mydr["訂單時間"]);
            myCheck.Add(co);

            mydr.Close();
            cmd.Dispose();
            cnn.Close();

            return View(myCheck);
        }


    }
}