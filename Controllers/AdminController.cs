using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using olxProject.Models;
using System.Security;
using System.Web.Security;

namespace olxProject.Controllers
{
    public class AdminController : Controller
    {
        olxDBDataContext DB = new olxDBDataContext();
        //
        // GET: /Admin/

        [Authorize]
        public ActionResult Index()
        {
            var AdResult = DB.Ads.Select(b => b);
            return View(AdResult);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var Result = (from a in DB.Ads
                         where a.AdId == id
                         select a).ToList()[0];
            return View(Result);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id,Ad Model)
        {
            var SelectedAd = (from a in DB.Ads
                          where a.AdId == id
                          select a).ToList()[0];
            SelectedAd.IsApproved = Model.IsApproved;
            DB.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
  	}
}