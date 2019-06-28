using FikrimVar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FikrimVar.Controllers
{
    public class YorumController : Controller
    {
        public MesajimVarEntities entities = new MesajimVarEntities();

        public PartialViewResult GetComments(int postId)
        {
            IQueryable<Yorumum> yorumlar = entities.Yorum.Where(y => y.Mesaj.MesajID == postId)
                                                       .Select(c => new Yorumum
                                                       {
                                                           YorumID = c.YorumID,
                                                           YorumZamani = (DateTime?)c.YorumTarihi.Value ?? DateTime.Now,
                                                           YorumMessage = c.YorumIcerik,
                                                           Kullancilar = new UserModel
                                                           {
                                                               KullaniciID = c.Uye.UyeID,
                                                               KullaniciAdi = c.Uye.UyeAdi,
                                                               KullaniciSoyadi = c.Uye.UyeSoyadi
                                                           }
                                                       }).AsQueryable();

            return PartialView("~/Views/Shared/_BenimYorumlarim.cshtml", yorumlar);
        }

        [HttpPost]
        public ActionResult AddComment(Yorumum yorum, int postId)
        {
            //bool result = false;
            Yorum yorumEntity = null;
            int userId = int.Parse(TempData.Peek("userid").ToString());

            var user = entities.Uye.FirstOrDefault(u => u.UyeID == userId);
            var post = entities.Mesaj.FirstOrDefault(p => p.MesajID == postId);

            if (yorum != null)
            {

                yorumEntity = new Yorum
                {
                    YorumIcerik = yorum.YorumMessage,
                    YorumTarihi = yorum.YorumZamani,
                };


                if (user != null && post != null)
                {
                    post.Yorum.Add(yorumEntity);
                    user.Yorum.Add(yorumEntity);

                    entities.SaveChanges();
                    //result = true;
                }
            }

            return RedirectToAction("GetComments", "Yorum", new { postId = postId });
        }

        [HttpGet]
        public PartialViewResult GetAltYorum(int YorumId)
        {
            IQueryable<FikrimVar.Models.AltYorum> subComments = entities.AltYorum.Where(sc => sc.Yorum.YorumID == YorumId)
                                                              .Select(sc => new FikrimVar.Models.AltYorum
                                                              {
                                                                  AltYorumId = sc.AltYorumID,
                                                                  AltYorumMesaji = sc.AltYorumMesaji,
                                                                  YorumZamani = (DateTime?)sc.YorumZamani.Value ?? DateTime.Now,
                                                                  Userr = new UserModel
                                                                  {
                                                                      KullaniciID = sc.Uye.UyeID,
                                                                      KullaniciAdi = sc.Uye.UyeAdi,
                                                                  }
                                                              }).AsQueryable();

            return PartialView("~/Views/Shared/_BenimAltYorumlarim.cshtml", subComments);
        }

        [HttpPost]
        public ActionResult AddSubComment(FikrimVar.Models.AltYorum altYorum, int ComID)
        {
            AltYorum subCommentEntity = null;
            int userId = int.Parse(TempData.Peek("userid").ToString());

            var user = entities.Uye.FirstOrDefault(u => u.UyeID == userId);
            var comment = entities.Yorum.FirstOrDefault(p => p.YorumID == ComID);

            if (altYorum != null)
            {

                subCommentEntity = new AltYorum
                {
                    AltYorumMesaji = altYorum.AltYorumMesaji,
                    YorumZamani = altYorum.YorumZamani,
                };


                if (user != null && comment != null)
                {
                    comment.AltYorum.Add(subCommentEntity);
                    user.AltYorum.Add(subCommentEntity);

                    entities.SaveChanges();
                    //result = true;
                }
            }

            return RedirectToAction("GetAltYorum", "Yorum", new { YorumId = ComID });

        }
    }
}