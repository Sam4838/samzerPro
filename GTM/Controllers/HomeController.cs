using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTM.Models;
namespace GTM.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        makemoneyEntities db = new makemoneyEntities();
        // GET: Home
        public ActionResult Testamin()
        {
            ViewBag.packages = db.PackagesTbls.ToList();
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.packages = db.PackagesTbls.ToList();
            return View();
        }
        public ActionResult CreateMsg()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateMsg(MessageTbl msg)
        {
            
            db.MessageTbls.Add(msg);
            db.SaveChanges();
            
            return RedirectToAction("CreateMsg");
        }

        public ActionResult GetMsg()
        {
            var msg = db.MessageTbls.ToList();
            return View(msg);
        }

        public ActionResult WaysToEarn()
        {
            return View();
        }

        //public ActionResult ShowAd(int id)
        //{
        //    var getAdId = db.AdvertiseTbls.Find(id);

        //    if (getAdId.AdsFilePath == null)
        //    {
        //        getAdId.AdsFilePath = "~/uploads/notAvlImage.jpg";
        //    }
        //    if (getAdId.UnitPrice == null)
        //    {
        //        getAdId.UnitPrice = "Not Specified";
        //    }

        //    return View(getAdId);
        //}

        public ActionResult PageNotFound()
        {
            return View();
        }

        //Maaz Actions

        public ActionResult ViewAds()
        {
            if (User.Identity.IsAuthenticated)
            {
                var uid = Convert.ToInt32(Session["userId"]);
                if (uid == 0 || uid == null)
                {
                    RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    uid = reg.UserId;
                }

                // Removing old date ads
                string dateToday = DateTime.Now.ToString("MM/dd/yyyy");
                List<userWatchList> oldList = db.userWatchLists.Where(x => x.userId == uid && x.showDate != dateToday).ToList();
                db.userWatchLists.RemoveRange(oldList);
                db.SaveChanges();


                ViewBag.balance = db.RegistrationTbls.Where(x => x.UserId == uid).FirstOrDefault();

                var ublnc = db.RegistrationTbls.Where(x => x.UserId == uid).FirstOrDefault();
                var userBlnc = ublnc.balance;
                int absBlnc = Convert.ToInt32(userBlnc);
                var accBlnc = Math.Abs(absBlnc);
                ViewBag.ubalance = accBlnc;
                //accBlnc = 0;
                var count = db.userPackages.Where(x => x.userId == uid && x.isPaid == "Y").Count();

                if (count == 1)
                {
                    userPackage usd = db.userPackages.Where(x => x.userId == uid && x.isPaid == "Y").FirstOrDefault();
                    var pkg = usd.pkgId;
                    PackagesTbl pkgRow = db.PackagesTbls.Where(x => x.packageId == pkg).FirstOrDefault();

                    var aDay = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                    DateTime dateTod = DateTime.ParseExact(aDay, "MM/dd/yyyy HH:mm:ss", null);
                    DateTime cnlDate = DateTime.ParseExact(usd.endDate, "MM/dd/yyyy HH:mm:ss", null);
                    //DateTime cnlDate = Convert.ToDateTime(usd.endDate);

                    if (dateTod > cnlDate)
                    {
                        db.userPackages.Remove(usd);
                        db.SaveChanges();

                        ViewBag.hasAds = "N";
                        ViewBag.Error = "The validity of your recent package has ended";
                    }

                    else
                    {
                        string dateString = DateTime.Now.ToString("MM/dd/yyyy");
                        List<userWatchList> nullAds = db.userWatchLists.Where(x => x.AdvertiseTbl.delFlag == "Y").ToList();
                        if (nullAds.Count > 0)
                        {
                            db.userWatchLists.RemoveRange(nullAds);
                            db.SaveChanges();

                            List<AdvertiseTbl> adsList = db.AdvertiseTbls.OrderBy(x => Guid.NewGuid()).Take(nullAds.Count).ToList();
                            foreach (var item in adsList)
                            {
                                userWatchList usw = new userWatchList();
                                usw.userId = uid;
                                usw.adId = item.AdvertiseId;
                                usw.showDate = dateString;
                                usw.isWatch = "N";
                                usw.packageId = pkg;
                                db.userWatchLists.Add(usw);
                            }
                            db.SaveChanges();


                        }
                        ViewBag.adsList = db.userWatchLists.Where(x => x.userId == uid && x.showDate == dateString && x.isWatch == "N" && x.adId != null).ToList();
                        ViewBag.hasAds = "Y";
                    }
                }
                else
                {
                    ViewBag.Error = "You are currently not subscribed to any package";
                }

                return View();
            }

            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ShowAd(int id, float inc, int uid)
        {
            if (User.Identity.IsAuthenticated)
            {


                var getAdId = db.AdvertiseTbls.Find(id);

                if (getAdId.AdsFilePath == null)
                {
                    getAdId.AdsFilePath = "~/uploads/notAvlImage.jpg";
                }
                if (getAdId.UnitPrice == null)
                {
                    getAdId.UnitPrice = "Not Specified";
                }
                return View(getAdId);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public void adResult(string adId, string increment, string uId)
        {
            if (User.Identity.IsAuthenticated)
            {
                int id = Convert.ToInt32(adId);
                float inc = float.Parse(increment);
                int uid = Convert.ToInt32(uId);
                RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserId == uid).FirstOrDefault();
                reg.balance = reg.balance + inc;

                userWatchList usd = db.userWatchLists.Where(x => x.userId == uid && x.adId == id).FirstOrDefault();
                usd.isWatch = "Y";
                db.SaveChanges();
            }
        }

        public ActionResult withdrawRec()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //For withdrawing money
        [HttpPost]
        public ActionResult withdrawRec(string phoneNo, float Amount)
        {
            if (User.Identity.IsAuthenticated)
            {

                RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                var uid = reg.UserId;

                WithdrawTbl with = new WithdrawTbl();
                with.userId = uid;
                with.phoneNo = phoneNo;
                with.Amount = Amount;
                with.reqDate = DateTime.Now.ToString("MM/dd/yyyy");
                with.TotalAmount = reg.balance;
                with.isPaid = "N";

                db.WithdrawTbls.Add(with);
                db.SaveChanges();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult packagePay(int packageId)
        {
            if (User.Identity.IsAuthenticated)
            {
                Session["packageId"] = packageId;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult packagePay(string phoneNo)
        {

            userPackage us = new userPackage();
            var uid = Convert.ToInt32(Session["userId"]);
            var pacId = Convert.ToInt32(Session["packageId"]);

            us.userId = uid;
            us.pkgId = pacId;
            us.phoneNo = phoneNo;
            us.isPaid = "N";

            db.userPackages.Add(us);
            db.SaveChanges();

            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

    }
}