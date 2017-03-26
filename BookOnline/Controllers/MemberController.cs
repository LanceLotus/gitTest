using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;

namespace BookOnline.Controllers
{
    public class MemberController : Controller
    {
        [Models.IsLogin_Filter]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ModifyMemberInfo()
        {
            Models.MemberInfo mi = null;

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLUser;PWD=1234");
            SqlCommand cmd = new SqlCommand("SELECT 帳號,姓,名,性別,生日,縣市,鄉鎮市,地址,學歷,電子郵件 FROM 會員表 WHERE 帳號=@account", cnn);
            cmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = Session["LoginName"].ToString();

            cnn.Open();
            SqlDataReader mydr = cmd.ExecuteReader();
            if (mydr.HasRows)
            {
                mydr.Read();
                mi = new Models.MemberInfo();
                mi.Account = mydr.GetString(0);
                mi.LastName = mydr.GetString(1);
                mi.FirstName = mydr.GetString(2);
                mi.Sex = Convert.ToByte(mydr[3]);
                mi.Birth = mydr.GetDateTime(4).ToShortDateString();
                mi.County = mydr.GetString(5);
                mi.Region = mydr.GetString(6);
                mi.Address = mydr.GetString(7);
                mi.Edu = mydr.GetByte(8);
                mi.Email = mydr.GetString(9);
            }
            mydr.Close();
            cmd.Dispose();
            cnn.Close();

            return View(mi);
        }

        public ActionResult UpdateMemberProcess(Models.MemberInfo mi)
        {
            byte[] originalPassword = System.Text.Encoding.UTF8.GetBytes(mi.Password);
            System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
            byte[] hashPassword = sha.ComputeHash(originalPassword);

            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLAdmin;PWD=1234");
            SqlCommand cmd = new SqlCommand("UPDATE 會員表 SET 密碼=@password,姓=@lastName,名=@firstName,性別=@sex,生日=@birth,縣市=@county,鄉鎮市=@region,地址=@address,學歷=@edu,電子郵件=@email WHERE 帳號=@account", cnn);
            cmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = Session["LoginName"].ToString();
            cmd.Parameters.Add("@password", SqlDbType.VarBinary).Value = hashPassword;
            cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = mi.LastName;
            cmd.Parameters.Add("@firstName", SqlDbType.NVarChar).Value = mi.FirstName;
            cmd.Parameters.Add("@sex", SqlDbType.Bit).Value = mi.Sex;
            cmd.Parameters.Add("@birth", SqlDbType.VarChar).Value = mi.Birth;
            cmd.Parameters.Add("@county", SqlDbType.NVarChar).Value = mi.County;
            cmd.Parameters.Add("@region", SqlDbType.NVarChar).Value = mi.Region;
            cmd.Parameters.Add("@address", SqlDbType.NVarChar).Value = mi.Address;
            cmd.Parameters.Add("@edu", SqlDbType.TinyInt).Value = mi.Edu;
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = mi.Email;

            cnn.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cnn.Close();

            return RedirectToAction("Index");
        }

    }
}
