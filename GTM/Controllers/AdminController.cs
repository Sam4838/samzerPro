using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTM.Models;
namespace GTM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        makemoneyEntities db = new makemoneyEntities();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.totUsers = db.RegistrationTbls.Count();
            ViewBag.sales = db.userPackages.Where(x => x.isPaid == "Y").Count();
            ViewBag.withdraws = db.WithdrawTbls.Where(x => x.isPaid == "Y").Count();
            return View();
        }

        public ActionResult GetUsersList()
        {
            var getUsers = db.RegistrationTbls.ToList();

            //var getEmpUser = db.RegistrationTbls.FirstOrDefault();
            //if (getEmpUser.Perspective == "Admin")
            //{
            //    getEmpUser.ProfileImgPath = "Visitor";
            //}
            return View(getUsers);
        }

        public ActionResult GetUsersDetails(int? id)
        {

            var getUsersDtlId = db.RegistrationTbls.Find(id);
            if (getUsersDtlId.ProfileImgPath == null)
            {
                getUsersDtlId.ProfileImgPath = "~/uploads/notAvlImage.jpg";
            }
            //userPackage usrpkg = db.userPackages.Where(x => x.userId == uid && x.isPaid == "Y").FirstOrDefault();
            //var uid = Convert.ToInt32(getUsersDtlId);
            //if (getUsersDtlId.UserId == 0 || getUsersDtlId.UserId == null)
            //{
            //    RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //    getUsersDtlId = reg.UserId;
            //}
            userPackage usrpkg = db.userPackages.Where(x => x.userId == id && x.isPaid == "Y").FirstOrDefault();

            if (usrpkg == null)
            {
                ViewBag.CurrentPkg = "N";
            }
            else
            {
                PackagesTbl pkg = db.PackagesTbls.Where(x => x.packageId == usrpkg.pkgId).FirstOrDefault();
                ViewBag.CurrentPkg = pkg.Title;
                //ViewBag.CurrentPkg = db.PackagesTbls.Where(x=>x.packageId==usrpkg.pkgId).FirstOrDefault();

            }
            return View(getUsersDtlId);
        }


        public ActionResult DeleteUser(int id)
        {
            var getUsrId = db.RegistrationTbls.Find(id);
            db.RegistrationTbls.Remove(getUsrId);
            db.SaveChanges();
            return RedirectToAction("GetUsersList");
        }

        public ActionResult GetUserSms()
        {
            var getUserTextMsg = db.MessageTbls.ToList();

            return View(getUserTextMsg);
        }

        public ActionResult GetAdsList(AdvertiseTbl advertise)
        {

            //var getAdvertiseList = db.AdvertiseTbls.ToList();

            if (advertise.AdsFilePath == null)
            {
                advertise.AdsFilePath = "~/uploads/notAvlImage.jpg";
            }
            if (advertise.UnitPrice == null)
            {
                advertise.UnitPrice = "Not Specified";
            }
            return View(db.AdvertiseTbls.ToList());
        }
        public ActionResult CreateAds()
        {
            return View();
        }

        [HttpPost]

        public ActionResult CreateAds(AdvertiseTbl advertise, HttpPostedFileBase AdsImageFile)
        {

            if (AdsImageFile != null)
            {

                string fileName = Path.GetFileNameWithoutExtension(AdsImageFile.FileName);
                string extension = Path.GetExtension(AdsImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("mmyyssff") + extension;
                advertise.AdsFilePath = "~/uploads/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/uploads/"), fileName);
                AdsImageFile.SaveAs(fileName);

            }
            else
            {

                advertise.AdsFilePath = null;

            }

            if (advertise.AdsFilePath == "~/uploads/notAvlImage.jpg")
            {
                advertise.AdsFilePath = null;
            }
            if (advertise.UnitPrice == null)
            {
                advertise.UnitPrice = null;
            }
            advertise.delFlag = "N";
            db.AdvertiseTbls.Add(advertise);
            db.SaveChanges();

            return RedirectToAction("GetAdsList");
        }

        public ActionResult EditAds(int id)
        {
            var getAdsId = db.AdvertiseTbls.Find(id);

            if (getAdsId.AdsFilePath == null)
            {
                getAdsId.AdsFilePath = "~/uploads/notAvlImage.jpg";
            }
            if (getAdsId.UnitPrice == null)
            {
                getAdsId.UnitPrice = "Not specified";
            }
            return View(getAdsId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAds(AdvertiseTbl advertise, HttpPostedFileBase AdsImageFile)
        {

            if (AdsImageFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(AdsImageFile.FileName);
                string extension = Path.GetExtension(AdsImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("mmyyssff") + extension;
                advertise.AdsFilePath = "~/uploads/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/uploads/"), fileName);
                AdsImageFile.SaveAs(fileName);

            }

            if (advertise.AdsFilePath == "/uploads/notAvlImage.jpg")
            {
                advertise.AdsFilePath = null;
            }
            advertise.delFlag = "N";

            db.Entry(advertise).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("GetAdsList");
        }


        public ActionResult GetAdsDetails(int id)
        {
            var getAdsDtlId = db.AdvertiseTbls.Find(id);

            if (getAdsDtlId.AdsFilePath == null)
            {
                getAdsDtlId.AdsFilePath = "~/uploads/notAvlImage.jpg";
            }
            if (getAdsDtlId.UnitPrice == null)
            {
                getAdsDtlId.UnitPrice = "Not specified";
            }
            return View(getAdsDtlId);
        }

        public ActionResult ShowAd(int id)
        {
            var getAdId = db.AdvertiseTbls.Find(id);

            if (getAdId.AdsFilePath == null)
            {
                getAdId.AdsFilePath = "~/uploads/notAvlImage.jpg";
            }
            return View(getAdId);
        }


        //public ActionResult DeleteAds(int id)
        //{
        //    var getadsId = db.AdvertiseTbls.Find(id);
        //    db.AdvertiseTbls.Remove(getadsId);
        //    db.SaveChanges();
        //    return RedirectToAction("GetAdsList");
        //}

        public ActionResult UserPackages()
        {
            ViewBag.usp = db.userPackages.ToList();
            return View();
        }
        //public ActionResult allowUserPac(int uid)
        //{
        //    userPackage usd = db.userPackages.Where(x => x.userId == uid).FirstOrDefault();

        //    var pkg = usd.pkgId;
        //    PackagesTbl pkgRow = db.PackagesTbls.Where(x => x.packageId == pkg).FirstOrDefault();

        //    var days = Convert.ToInt32(pkgRow.validity);
        //    days = days * 30;

        //    DateTime aDay = DateTime.Now;
        //    TimeSpan aMonth = new System.TimeSpan(days, 0, 0, 0);
        //    DateTime aDayAfterAMonth = aDay.Add(aMonth);
        //    var endDate = aDayAfterAMonth.ToString("MM/dd/yyyy HH:mm:ss");

        //    usd.isPaid = "Y";
        //    usd.endDate = endDate;

        //    db.SaveChanges();
        //    return RedirectToAction("UserPackages", "Admin");
        //}
        public ActionResult allowUserPac(int uid, int id)
        {
            userPackage usd = db.userPackages.Where(x => x.userId == uid && x.id == id).FirstOrDefault();

            if (usd.isPaid == "N")
            {
                var pkg = usd.pkgId;
                PackagesTbl pkgRow = db.PackagesTbls.Where(x => x.packageId == pkg).FirstOrDefault();

                var days = Convert.ToInt32(pkgRow.validity);
                days = days * 30;

                DateTime aDay = DateTime.Now;
                TimeSpan aMonth = new System.TimeSpan(days, 0, 0, 0);
                DateTime aDayAfterAMonth = aDay.Add(aMonth);
                var endDate = aDayAfterAMonth.ToString("MM/dd/yyyy HH:mm:ss");

                usd.isPaid = "Y";
                usd.endDate = endDate;

                //DateTime cnlDate = Convert.ToDateTime(usd.endDate);

                //if (aDay < cnlDate) {
                //    var str = "Greater";
                //}
                db.SaveChanges();
                return RedirectToAction("UserPackages", "Admin");
            }
            else
            {
                return RedirectToAction("UserPackages", "Admin");
            }

        }


        //public ActionResult disUserPac(int uid)
        //{
        //    userPackage usd = db.userPackages.Where(x => x.userId == uid).FirstOrDefault();
        //    usd.isPaid = "N";
        //    db.SaveChanges();
        //    return RedirectToAction("UserPackages", "Admin");
        //}

        //public ActionResult delUserPac(int uid)
        //{
        //    userPackage usd = db.userPackages.Where(x => x.userId == uid).FirstOrDefault();
        //    db.userPackages.Remove(usd);
        //    db.SaveChanges();
        //    return RedirectToAction("UserPackages", "Admin");
        //}
        public ActionResult disUserPac(int uid, int id)
        {
            userPackage usd = db.userPackages.Where(x => x.userId == uid && x.id == id).FirstOrDefault();
            usd.isPaid = "N";
            db.SaveChanges();
            return RedirectToAction("UserPackages", "Admin");
        }

        public ActionResult delUserPac(int uid, int id)
        {
            userPackage usd = db.userPackages.Where(x => x.userId == uid && x.id == id).FirstOrDefault();
            db.userPackages.Remove(usd);
            db.SaveChanges();
            return RedirectToAction("UserPackages", "Admin");
        }

        public ActionResult DeleteAds(int id)
        {
            AdvertiseTbl getadsId = db.AdvertiseTbls.Find(id);
            getadsId.delFlag = "Y";
            db.SaveChanges();
            return RedirectToAction("GetAdsList");
        }

        public ActionResult GetWithdrawDetails()
        {

            ViewBag.withdraw = db.WithdrawTbls.ToList();

            return View();
        }

        //public ActionResult allowWithdraw(int userId)
        //{

        //    WithdrawTbl with = db.WithdrawTbls.Where(x => x.userId == userId).FirstOrDefault();
        //    with.isPaid = "Y";
        //    with.payDate = DateTime.Now.ToString("MM/dd/yyyy");

        //    RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserId == userId).FirstOrDefault();
        //    reg.balance = reg.balance - with.Amount;

        //    db.SaveChanges();

        //    return RedirectToAction("GetWithdrawDetails", "Admin");
        //}

        //public ActionResult delWithdraw(int userId)
        //{
        //    WithdrawTbl with = db.WithdrawTbls.Where(x => x.userId == userId).FirstOrDefault();
        //    db.WithdrawTbls.Remove(with);
        //    db.SaveChanges();
        //    return RedirectToAction("GetWithdrawDetails", "Admin");
        //}


        public ActionResult allowWithdraw(int userId, int id)
        {

            WithdrawTbl with = db.WithdrawTbls.Where(x => x.userId == userId && x.id == id).FirstOrDefault();
            if (with.isPaid == "N")
            {
                with.isPaid = "Y";
                with.payDate = DateTime.Now.ToString("MM/dd/yyyy");

                RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserId == userId).FirstOrDefault();
                reg.balance = reg.balance - with.Amount;

                db.SaveChanges();
                return RedirectToAction("GetWithdrawDetails", "Admin");
            }
            else
            {
                return RedirectToAction("GetWithdrawDetails", "Admin");
            }


        }

        public ActionResult delWithdraw(int userId, int id)
        {
            WithdrawTbl with = db.WithdrawTbls.Where(x => x.userId == userId && x.id == id).FirstOrDefault();
            db.WithdrawTbls.Remove(with);
            db.SaveChanges();
            return RedirectToAction("GetWithdrawDetails", "Admin");
        }

        public ActionResult UserManualPkg()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserManualPkg(userPackage usrpkg/*,string isPaid*/)
        {

            //if (usrpkg.isPaid=="Y")
            //{
            //    usrpkg.isPaid = "Y";
            //}
            //else
            //{
            //    TempData["ispaidval"] = "IsPaid must be checked!";
            //}
            db.userPackages.Add(usrpkg);
            db.SaveChanges();
            return View();
        }

    }
}