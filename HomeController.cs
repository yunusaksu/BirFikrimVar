using FikrimVar.Models;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FikrimVar.Controllers
{
    public class HomeController : Controller
    {
        MesajimVarEntities entities = new MesajimVarEntities();
        [Authorize]
        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var mesajlar = entities.Mesaj.OrderBy(x => x.MesajBaslik).ToPagedList(pageNumber, pageSize);
            return View(mesajlar);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewData["KullanıcıAdı"] = User.Identity.Name;
            return View();
        }

        public ActionResult DetailMessage(int? id)
        {
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