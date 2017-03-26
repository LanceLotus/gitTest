using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace BookOnline.Models
{
    public class AdminLogin_Filter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["AdminLoginName"] == null)
            {
                //取出原本要前往的網址
                string redirectUrl = filterContext.HttpContext.Request.RawUrl;

                UrlHelper uu = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                string url = uu.Action("Main", "Admin", new System.Web.Routing.RouteValueDictionary { { "url", redirectUrl } });
                filterContext.HttpContext.Response.Redirect(url);
            }
            
        }
    }
}