using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using olxProject.Models;

using System.Web.Security;
using System.Data.SqlClient;

namespace olxProject.Controllers
{
    public class HomeController : Controller
    {
    public static int globaluserid=1;
        //
        // GET: /Home/
        olxDBDataContext DB = new olxDBDataContext();
        public ActionResult Index()
        {
            var Result = from a in DB.Ads
                         where a.IsApproved==true
                         select a;
            var DetailsResult = (from d in DB.AdsCategoryDetails
                                where d.Ad.IsApproved==true
                                select d).ToList();
            ViewBag.AdDetails = DetailsResult;
        
            return View(Result);
        }
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(FormCollection LoginData)
        {
            var UserResult = (from u in DB.Users
                             where u.UserName==LoginData["username"] 
                             && u.Password==LoginData["password"]
                             select u).ToList();

            if (UserResult.Count>0)
            {
                FormsAuthentication.SetAuthCookie(LoginData["username"], true);
                if (UserResult[0].RoleId==1)
                {

                    return RedirectToAction("Index", "Admin");
                }
                else
                {

                    globaluserid = UserResult[0].UserId;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View();
            }
            
        }

        public ActionResult ViewDetails(int id)
        {
            var AdResult = (from a in DB.Ads
                           where a.AdId == id
                           select a).ToList();
            AdResult[0].ViewsCount++;
            DB.SubmitChanges();

            var DetailsResult = (from d in DB.AdsCategoryDetails
                                 where d.AdId == id
                                 select d).ToList();
            ViewBag.AdDetails = DetailsResult;

            //SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=olxDB;Integrated Security=True");
            //string querystring = "SELECT Images.imgurl FROM Images INNER JOIN Ads ON Images.adId = Ads.AdId WHERE(Images.ImageId = 1)";
            //SqlDataAdapter sc = new SqlDataAdapter(querystring,conn);
            //var imgurl = sc.ToString();

            var imgurl = DB.Images.Where(b => b.adId == id).Select(b => b.imgurl).ToList();
            ViewBag.imgurl = imgurl[0];
            return View(AdResult[0]);

        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}