using FikrimVar.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace FikrimVar.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel user, string ReturnUrl = "/")
        {
            if (IsValid(user))
            {
                FormsAuthentication.SetAuthCookie(user.KullaniciAdi, false);
                return Redirect(ReturnUrl);
            }
            else
            {
                return View(user);
            }
        }

        private bool IsValid(UserModel user)
        {
            using (MesajimVarEntities entities = new MesajimVarEntities())
            {
                var us = entities.Uye.FirstOrDefault(u => u.UyeAdi == user.KullaniciAdi && u.UyeSoyadi == user.KullaniciSoyadi && u.UyeSifre == user.Sifre);
                TempData["userId"] = us.UyeID;
                TempData.Keep("userId");
                return us != null ? true : false;
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/Home/About");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Uye uye)
        {
            if (ModelState.IsValid)
            {
                MesajimVarEntities entities = new MesajimVarEntities();
                entities.Uye.Add(uye);
                entities.SaveChanges();
            }
            return RedirectToAction("Login","Login");
        }
    }
}