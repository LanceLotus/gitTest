using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;

namespace BookOnline.Controllers
{
    public class NewMemberController : Controller
    {
        public ActionResult AddNewMemberInfo()
        {
            return View();
        }

        public ActionResult AddNewMemberProcess(Models.MemberInfo mi)
        {
            byte[] originalPassword = System.Text.Encoding.UTF8.GetBytes(mi.Password);
            System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
            byte[] hashPassword = sha.ComputeHash(originalPassword);
            
            SqlConnection cnn = new SqlConnection("server=localhost;database=WebData;UID=SQLAdmin;PWD=1234");
            SqlCommand cmd = new SqlCommand("INSERT INTO 會員表(帳號,密碼,姓,名,性別,生日,縣市,鄉鎮市,地址,學歷,電子郵件) VALUES(@account,@password,@lastName,@firstName,@sex,@birth,@county,@region,@address,@edu,@email)", cnn);
            cmd.Parameters.Add("@account", SqlDbType.NVarChar).Value = mi.Account;
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

            return RedirectToAction("AddMemberFinish");
        }

        public ActionResult AddMemberFinish()
        {
            return View();
        }


    }
}
