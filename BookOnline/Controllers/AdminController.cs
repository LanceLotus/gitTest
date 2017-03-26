using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BookOnline.Controllers
{
    public class AdminController : Controller
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
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM 管理員表 WHERE 帳號=@account AND 密碼=@password", cnn);
            cmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = Account;
            cmd.Parameters.Add("@password", SqlDbType.VarBinary).Value = hashPassword;

            cnn.Open();
            int cc = (int)cmd.ExecuteScalar();
            cmd.Dispose();
            cnn.Close();


            if (cc > 0)
            {
                Session["AdminLoginName"] = Account;

                return RedirectToAction("Index", "Book");
            }
            else
            {
                return RedirectToAction("Main", new System.Web.Routing.RouteValueDictionary { { "et", 1 } });
            }
        }

    }
}
