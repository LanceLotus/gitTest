using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;

namespace BookOnline.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Main(int? et)
        {
            if (et != null && et.Value == 1)
                ViewData["ErrMsg"] = "帳號或密碼有誤！";

            return View();
        }

        public ActionResult LoginValidProcess(string Account, string Password)
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

                return RedirectToAction("Index", "Member");
            }
            else
            {
                return RedirectToAction("Main", new System.Web.Routing.RouteValueDictionary { { "et", 1 } });
            }
        }

        public ActionResult Logout()
        {
            Session["LoginName"] = null;
            Session.Remove("LoginName");

            return View();
        }

        public ActionResult ForgetPWD(int? et)
        {
            if (et != null && et.Value == 1)
                ViewData["ErrMsg"] = "您的資料有誤！請確認是否正確!";

            return View();
        }

        public ActionResult GetPWD()
        {
            ViewData["NewPWD"] = Session["NewPWD"];
            return View();
        }

    }
}
