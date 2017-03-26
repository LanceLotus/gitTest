using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;

namespace BookOnline.Controllers
{
    public class BookController : Controller
    {
        [Models.AdminLogin_Filter]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get(int? id)
        {
            Models.BookInfo bi = null;
            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
            SqlCommand cmd = new SqlCommand("SELECT id,title,author,price,times FROM Book where id=@id", cnn);
            cmd.Parameters.Add("@id",SqlDbType.Int).Value = id;

            cnn.Open();
            SqlDataReader mydr = cmd.ExecuteReader();
            if (mydr.HasRows)
            {
                mydr.Read();
                bi = new Models.BookInfo();
                bi.Id = mydr.GetInt32(0);
                bi.Title = mydr.GetString(1);
                bi.Author = mydr.GetString(2);
                bi.Price = Double.Parse(mydr.GetSqlMoney(3).ToString());
                bi.Times = mydr.GetDateTime(4).ToShortDateString();
            }
            mydr.Close();
            cmd.Dispose();
            cnn.Close();

            return Json(bi, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Insert(Models.BookInfo bi,HttpPostedFileBase pic) {
            if(pic != null){
                if (pic.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(pic.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/FileUpload"), fileName);
                    pic.SaveAs(path);
                }
            }
            
            byte[] pp = new byte[0];
            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLAdmin;PWD=1234");
            SqlCommand cmd = new SqlCommand("INSERT INTO Book (title,author,price,times,pic) VALUES (@title,@author,@price,@times,@pic) ", cnn);
            cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = bi.Title;
            cmd.Parameters.Add("@author", SqlDbType.NVarChar).Value = bi.Author;
            cmd.Parameters.Add("@price", SqlDbType.Money).Value = bi.Price;
            cmd.Parameters.Add("@times", SqlDbType.DateTime).Value = DateTime.Now;
            string nn = pic.FileName.Substring(pic.FileName.LastIndexOf('\\') + 1);
            string picPath = Server.MapPath("~/FileUpload/") + nn;
            pp = System.IO.File.ReadAllBytes(picPath);
            cmd.Parameters.Add("@pic", SqlDbType.Image).Value = pp;

            System.IO.File.Delete(picPath);

            cnn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cnn.Close();

            return RedirectToAction("Index");
        }

        public ActionResult Update(Models.BookInfo bi)
        {

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLAdmin;PWD=1234");
            SqlCommand cmd = new SqlCommand("UPDATE Book SET title=@title,author=@author,price=@price,times=@times WHERE id=@id", cnn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = bi.Id;
            cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = bi.Title;
            cmd.Parameters.Add("@author", SqlDbType.NVarChar).Value = bi.Author;
            cmd.Parameters.Add("@price", SqlDbType.Money).Value = bi.Price;
            cmd.Parameters.Add("@times", SqlDbType.DateTime).Value = DateTime.Now;

            cnn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cnn.Close();

            return Json(bi, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Models.BookInfo bi)
        {

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLAdmin;PWD=1234");
            SqlCommand cmd = new SqlCommand("DELETE FROM Book WHERE id=@id", cnn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = bi.Id;

            cnn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cnn.Close();

            return Json(bi, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Query()
        {
            Models.BookInfo bi = null;
            List<Models.BookInfo> result = null;

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
            SqlCommand cmd = new SqlCommand("SELECT id,title,author,price,times FROM Book", cnn);

            cnn.Open();
            SqlDataReader mydr = cmd.ExecuteReader();
            if (mydr.HasRows)
            {
                result = new List<Models.BookInfo>();
                while(mydr.Read()){
                    bi = new Models.BookInfo();
                    bi.Id = mydr.GetInt32(0);
                    bi.Title = mydr.GetString(1);
                    bi.Author = mydr.GetString(2);
                    bi.Price = Double.Parse(mydr.GetSqlMoney(3).ToString());
                    bi.Times = mydr.GetDateTime(4).ToShortDateString();
                    result.Add(bi);
                }
            }
            mydr.Close();
            cmd.Dispose();
            cnn.Close();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

}
