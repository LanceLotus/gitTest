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
    public class ToolController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetBookImage(int? pid)
        {
            byte[] pp = new byte[0];

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
            SqlCommand cmd = new SqlCommand("SELECT pic FROM Book WHERE id=@pid", cnn);
            cmd.Parameters.Add("@pid", SqlDbType.Int).Value = pid;
            cnn.Open();
            object oo = cmd.ExecuteScalar();
            if (oo != DBNull.Value)
                pp = (byte[])oo;
            cmd.Dispose();
            cnn.Close();

            return File(pp, "image/gif");
        }


        public ActionResult GetValidPic()
        {
            ValidNum.ValidNum vn = new ValidNum.ValidNum(6);
            Session["ValidAns"] = vn.ValidAnswer;
            System.Drawing.Bitmap bmp = vn.ValidPic;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return File(ms.ToArray(), "Image/png");
        }

        public string GetValidAns()
        {
            return Session["ValidAns"].ToString();
        }

        public ActionResult GetLoginPane()
        {
            return View();
        }

        public string AjaxLoginValidProcess(string Account, string Password)
        {
            byte[] originalPassword = System.Text.Encoding.UTF8.GetBytes(Password);
            System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
            byte[] hashPassword = sha.ComputeHash(originalPassword);

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM 會員表 WHERE 帳號=@account AND 密碼=@password", cnn);
            cmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = Account;
            cmd.Parameters.Add("@password", SqlDbType.VarBinary).Value = hashPassword;

            cnn.Open();
            int cc = (int)cmd.ExecuteScalar();
            cmd.Dispose();
            cnn.Close();


            if (cc > 0)
            {
                Session["LoginName"] = Account;
                return "y";
            }
            else
            {
                return "n";
            }

        }

        public ActionResult GetDetail(string detailId)
        {
            Session["detailId"] = detailId;

            return RedirectToAction("ShopDetail", "Shop");
        }

        public ActionResult GetPassWord(string Account, string Email)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            Session["NewPWD"] = finalString;

            byte[] originalPassword = System.Text.Encoding.UTF8.GetBytes(finalString);
            System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
            byte[] hashPassword = sha.ComputeHash(originalPassword);

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM 會員表 WHERE 帳號=@account AND 電子郵件=@email", cnn);
            cmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = Account;
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = Email;

            cnn.Open();
            int cc = (int)cmd.ExecuteScalar();
            cmd.Dispose();
            cnn.Close();


            if (cc > 0)
            {
                Session["CheckAcc"] = Account;
                SqlConnection cnn2 = new SqlConnection("server=localhost;database=WebData;UID=SQLAdmin;PWD=1234");
                SqlCommand cmd2 = new SqlCommand("UPDATE 會員表 SET 密碼=@password WHERE 帳號=@account", cnn2);
                cmd2.Parameters.Add("@account", SqlDbType.NVarChar).Value = Session["CheckAcc"];
                cmd2.Parameters.Add("@password", SqlDbType.VarBinary).Value = hashPassword;

                cnn2.Open();
                cmd2.ExecuteNonQuery();
                cmd2.Dispose();
                cnn2.Close();

                return RedirectToAction("GetPWD", "Login");
            }
            else
            {
                return RedirectToAction("ForgetPWD", "Login", new System.Web.Routing.RouteValueDictionary { { "et", 1 } });
            }
        }

    }
}
