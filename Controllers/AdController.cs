using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using olxProject.Models;


namespace olxProject.Controllers
{
    public class AdController : Controller
    {
        //
        // GET: /Ad/
        olxDBDataContext DB = new olxDBDataContext();

        public ActionResult ChooseCategory()
        {
            var CategoriesResult = from c in DB.Categories
                                   select c;
            SelectList Categories = new SelectList(CategoriesResult,"CatId","CatName");
            ViewBag.Categories = Categories;
            return View();
        }

        [HttpPost]
        public ActionResult ChooseCategory(int id)
        {
            return RedirectToAction("Create","Ad", new {id=id });
        }
        public ActionResult Create(int id)
        {
            var LocationsResult = from l in DB.Locations
                                 select l;
            SelectList locations = new SelectList(LocationsResult,"locationId","locationName");
            ViewBag.locations = locations;

            var CatDetails = from cd in DB.CategoryDetails
                             where cd.Catid == id
                             select cd;
            ViewBag.CatDetails = CatDetails.ToList();
            return View();
           
        }
        [HttpPost]
        public ActionResult Create(FormCollection Data,int id)
        {
            Ad NewAd = new Ad();
            NewAd.Title = Data["Title"];
            NewAd.Description = Data["Description"];
            NewAd.Price_Salary = Convert.ToDecimal( Data["Price_Salary"] );
            NewAd.LocationId =Convert.ToInt32( Data["locationId"]);
            NewAd.UserId = HomeController.globaluserid;
            NewAd.PublishDate = DateTime.Now;
            NewAd.IsApproved = false;
            NewAd.CatId = id;
            NewAd.ViewsCount = 0;
            Image NewImg =new Image();
            NewImg.adId = id;
            DB.Ads.InsertOnSubmit(NewAd);
            DB.SubmitChanges();

            //int AdId = DB.Ads.Where(d => d.UserId == NewAd.UserId).OrderByDescending(o => o.AdId).Select(ad => ad.AdId).ToList()[0];
            //int AdId = (from a in DB.Ads
            //           where a.UserId == NewAd.UserId
            //           orderby a.AdId descending
            //           select a.AdId).ToList()[0];

            //int AdId = (from a in DB.Ads
            //            where a.UserId == NewAd.UserId
            //            select a.AdId).Max();

            int AdId = DB.Ads.Where(d => d.UserId == NewAd.UserId).Max(c => c.AdId);
            

                 var CatDetails = (from CD in DB.CategoryDetails
                                 where CD.Catid == id
                                 select CD ).ToList();
                 for (int i = 0; i < CatDetails.Count; i++)
                 {
                     for (int j = 0; j < Data.AllKeys.Length; j++)
                     {
                         if (Data.AllKeys[j]== CatDetails[i].DetailName )
                         {
                             AdsCategoryDetail ACD = new AdsCategoryDetail();
                             ACD.AdId = AdId;
                             ACD.DetailId = Convert.ToInt32( Data["_" + CatDetails[i].DetailName] );
                             ACD.Value = Data[CatDetails[i].DetailName];
                             DB.AdsCategoryDetails.InsertOnSubmit(ACD);
                         }
                     }
                 }

                 DB.SubmitChanges();
            return RedirectToAction("Index","Home");

            
        }
	}
}