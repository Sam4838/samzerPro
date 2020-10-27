using GTM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace GTM.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: Account
        makemoneyEntities db = new makemoneyEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(RegistrationTbl registration)
        {


            var count = db.RegistrationTbls.Where(x => x.UserName == registration.UserName && x.Password == registration.Password).Count();
            if (count != 0)
            {
                registration = db.RegistrationTbls.Where(x => x.UserName == registration.UserName && x.Password == registration.Password).FirstOrDefault();
                if (registration.ProfileImgPath == null)
                {
                    registration.ProfileImgPath = "~/uploads/notAvlImage.jpg";
                }
                Session["userId"] = registration.UserId;
                Session["img"] = registration.ProfileImgPath;
                Session["role"] = registration.Perspective;
                Session["Fname"] = registration.FName;
                Session["email"] = registration.UserName;
                Session["blnc"] = registration.balance;

                //Session["balance"] = registration.balance;


                var usp = db.userPackages.Where(x => x.userId == registration.UserId && x.isPaid == "Y").FirstOrDefault();

                var val = db.userPackages.Where(x => x.userId == registration.UserId && x.isPaid == "Y").Count();

                if (val != 0)
                {
                    string dateString = DateTime.Now.ToString("MM/dd/yyyy");
                    var wlist = db.userWatchLists.Where(x => x.userId == registration.UserId && x.showDate == dateString).Count();
                    if (wlist == 0 || wlist == null)
                    {
                        List<AdvertiseTbl> adsList = db.AdvertiseTbls.OrderBy(x => Guid.NewGuid()).Take(20).ToList();
                        foreach (var item in adsList)
                        {
                            userWatchList usw = new userWatchList();
                            usw.userId = registration.UserId;
                            usw.adId = item.AdvertiseId;
                            usw.showDate = dateString;
                            usw.isWatch = "N";
                            usw.packageId = usp.pkgId;
                            db.userWatchLists.Add(usw);
                        }
                    }

                    db.SaveChanges();
                    string dateToday = DateTime.Now.ToString("MM/dd/yyyy");
                    List<userWatchList> oldList = db.userWatchLists.Where(x => x.userId == registration.UserId && x.showDate != dateToday).ToList();
                    db.userWatchLists.RemoveRange(oldList);
                    db.SaveChanges();
                }


                FormsAuthentication.SetAuthCookie(registration.UserName, false);
                return RedirectToAction("Index", "Home");
            }

            else
            {
                TempData["InvalidMsg"] = "Email or Password is incorrect! Try again..";
                return View();

            }

        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");

        }

        public ActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(RegistrationTbl registration, string IsAgreeTerms/*, HttpPostedFileBase UserImageFile*/)

        {

            if (registration.ProfileImgPath == null)
            {
                registration.ProfileImgPath = null;
            }
            //else
            //{

            //    string fileName = Path.GetFileNameWithoutExtension(UserImageFile.FileName);
            //    string extension = Path.GetExtension(UserImageFile.FileName);
            //    fileName = fileName + DateTime.Now.ToString("mmyyssff") + extension;
            //    registration.ProfileImgPath = "~/uploads/" + fileName;
            //    fileName = Path.Combine(Server.MapPath("~/uploads/"), fileName);
            //    UserImageFile.SaveAs(fileName);

            //}
            if (registration.Gender == null)
            {
                registration.Gender = "Not Specified";
            }
            if (registration.Perspective == null)
            {
                registration.Perspective = "Customer";
            }

            //if (IsAgreeTerms == "true" || IsAgreeTerms == "false")
            if (IsAgreeTerms == "on" || IsAgreeTerms == "off")
            {
                registration.IsAgreeTerms = true;


            }

            var userBlnc = registration.balance;
            int absBlnc = Convert.ToInt32(userBlnc);
            var accBlnc = Math.Abs(absBlnc);
            registration.balance = accBlnc;
            accBlnc = 0;
            //if (ModelState.IsValid)
            //{
            string usrpwd = registration.Password;
            string confpwd = registration.ConfirmPassword;

            if (usrpwd != confpwd)
            {
                ViewBag.Matchpwd = "Both passwords must be matched!";

                return View();

            }
            var countUsr = db.RegistrationTbls.Where(x => x.UserName == registration.UserName).Count();
            if (countUsr != 0 || countUsr > 1)
            {
                ViewBag.UserExist = "This email is already exist! Try again..";
                return View();

            }
            else
            {
                db.RegistrationTbls.Add(registration);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

        }

        public ActionResult UserProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var uid = Convert.ToInt32(Session["userId"]);
                if (uid == 0 || uid == null)
                {
                    RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    uid = reg.UserId;
                }

                //var userId = Convert.ToInt32(Session["userId"]);
                RegistrationTbl regstr = db.RegistrationTbls.Where(x => x.UserId == uid).FirstOrDefault();
                if (regstr.ProfileImgPath == null)
                {
                    regstr.ProfileImgPath = "~/uploads/notAvlImage.jpg";
                }

                userPackage usrpkg = db.userPackages.Where(x => x.userId == uid && x.isPaid == "Y").FirstOrDefault();
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
                return View(regstr);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult UpdateProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var uid = Convert.ToInt32(Session["userId"]);
                if (uid == 0 || uid == null)
                {
                    RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    uid = reg.UserId;
                }

                //var userId = Convert.ToInt32(Session["userId"]);
                RegistrationTbl regstr = db.RegistrationTbls.Where(x => x.UserId == uid).FirstOrDefault();
                if (regstr.ProfileImgPath == null)
                {
                    regstr.ProfileImgPath = "~/uploads/notAvlImage.jpg";
                }
                return View(regstr);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpPost]
        public ActionResult UpdateProfile(RegistrationTbl reg, string IsAgreeTerms, HttpPostedFileBase UserImageFile)
        {
            if (UserImageFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(UserImageFile.FileName);
                string extension = Path.GetExtension(UserImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("mmyyssff") + extension;
                reg.ProfileImgPath = "~/uploads/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/uploads/"), fileName);
                UserImageFile.SaveAs(fileName);

            }
            if (reg.ProfileImgPath == "~/uploads/notAvlImage.jpg")
            {
                reg.ProfileImgPath = null;
            }
            reg.ConfirmPassword = reg.Password;
            if (ModelState.IsValid)
            {
                db.Entry(reg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserProfile");
            }
            //return View();
            return RedirectToAction("UserProfile");

        }


        //public ActionResult UpdateProfile()
        //{


        //    var userId = Convert.ToInt32(Session["userId"]);
        //    RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserId == userId).FirstOrDefault();
        //    if (reg.ProfileImgPath == null)
        //    {
        //        reg.ProfileImgPath = "~/uploads/notAvlImage.jpg";
        //    }
        //    return View(reg);
        //}

        //[HttpPost]
        //public ActionResult UpdateProfile(RegistrationTbl reg, string IsAgreeTerms)
        //{
        //    if (IsAgreeTerms == "true")
        //    {
        //        reg.IsAgreeTerms = true;
        //    }
        //    else
        //    {
        //        reg.IsAgreeTerms = false;

        //    }

        //    if (reg.UserImageFile != null)
        //    {
        //        string fileName = Path.GetFileNameWithoutExtension(reg.UserImageFile.FileName);
        //        string extension = Path.GetExtension(reg.UserImageFile.FileName);
        //        fileName = fileName + DateTime.Now.ToString("mmyyssff") + extension;
        //        reg.ProfileImgPath = "~/uploads/" + fileName;
        //        fileName = Path.Combine(Server.MapPath("~/uploads/"), fileName);
        //        reg.UserImageFile.SaveAs(fileName);

        //    }

        //    if (reg.ProfileImgPath == "/uploads/notAvlImage.jpg")
        //    {
        //        reg.ProfileImgPath = null;
        //    }

        //    return View();
        //}
        public JsonResult IsUserExists(string userdata)
        {

            System.Threading.Thread.Sleep(200);
            var searchData = db.RegistrationTbls.Where(x => x.UserName == userdata).SingleOrDefault();
            if (searchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            //return Json(!db.RegistrationTbls.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult CheckUser(string userName)
        //{
        //    bool result = !db.RegistrationTbls.ToList().Exists(model => model.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
        //    return Json(result);
        //}

        public ActionResult UserBalance()
        {

            if (User.Identity.IsAuthenticated)
            {
                var uid = Convert.ToInt32(Session["userId"]);
                if (uid == 0 || uid == null)
                {
                    RegistrationTbl reg = db.RegistrationTbls.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                    uid = reg.UserId;

                }

                RegistrationTbl regstr = db.RegistrationTbls.Where(x => x.UserId == uid).FirstOrDefault();
                var userBlnc = regstr.balance;
                int absBlnc = Convert.ToInt32(userBlnc);
                var accBlnc = Math.Abs(absBlnc);
                ViewBag.usrbalance = accBlnc;

                return View(regstr);

            }
            else
            {
                return RedirectToAction("Account", "Login");
            }


        }


        public ActionResult RecoverPswd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecoverPswd(RegistrationTbl reg)

        {
            var getemail = db.RegistrationTbls.Where(x => x.UserName == reg.UserName).FirstOrDefault();
            //var pswrd = db.RegistrationTbls.Where(x => x.Password == reg.Password).FirstOrDefault();
            if (getemail.UserName == reg.UserName)
            {
                string pwd = getemail.Password;

                MailMessage mm = new MailMessage("sameen048380@gmail.com", getemail.UserName);
                mm.Subject = "Recover Password";
                mm.Body = string.Format("<h1 style='color:dodgerblue'>Assalam-o-Alaikum!</h1><h3 style='font-size:15px;font-family:Times New Roman;'>Hope you are fine and doing well.Since we have received a request to recover your password that has been processed!</3></br><h4>Below is your Password:</h4><h2 style='color:green'>{0}</h2><b>Regards:</b><br><p>Samzer Team</p><a href='http://samzer.pk/'>samzer.pk</a>", pwd);
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                //smtp.Host = "makemoney-pk.somee.com"; 

                smtp.EnableSsl = true;
                NetworkCredential nc = new NetworkCredential();
                nc.UserName = "sameen048380@gmail.com";
                nc.Password = "AllaH143";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = nc;
                smtp.Port = 587;

                smtp.Send(mm);

                ViewBag.MailSuccess = "Your Password has been sent to " + getemail.UserName;
            }


            return View();
        }

    }



}