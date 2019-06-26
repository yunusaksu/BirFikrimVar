using FikrimVar.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace FikrimVar.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewData["KullanıcıAdı"] = User.Identity.Name;
            MesajimVarEntities entities = new MesajimVarEntities();
            var liste = entities.Mesaj.ToList();
            return View(liste);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult DetailMessage(int? id)
        {
            MesajimVarEntities entities = new MesajimVarEntities();
            Mesaj ms = entities.Mesaj.Find(id);
            return View(ms);
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult MesajYolla()
        {
            ViewBag.KategoriListesi = GetCategory();
            return View();
        }

        
        [HttpPost]
        public ActionResult MesajYolla(Mesajim mesaj)
        {
            Mesaj msj = new Mesaj();
            msj.MesajBaslik = mesaj.MesajBaslik;
            msj.MesajIcerik = mesaj.MesajIcerik;
            msj.KategoriId = mesaj.KategoriId;
            msj.UyeId = int.Parse(TempData.Peek("userid").ToString()); // that's problem Solution is this website(https:/stackoverflow.com/questions/21252888/tempdata-keep-vs-pee)
            using (MesajimVarEntities entities = new MesajimVarEntities())
            {
                entities.Mesaj.Add(msj);
                entities.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        private static List<SelectListItem> GetCategory()
        {
            MesajimVarEntities entities = new MesajimVarEntities();
            List<SelectListItem> categorylistesi = (from p in entities.Kategori.AsEnumerable()
                                                    select new SelectListItem
                                                    {
                                                        Text = p.KategoriAdi,
                                                        Value = p.KategoriID.ToString()
                                                    }).ToList();
            //İlk pozisyona default item'ı ekliyorum
            categorylistesi.Insert(0, new SelectListItem { Text = "--Kategori Seçiniz--", Value = "" });
            return categorylistesi;
        }
    }
}