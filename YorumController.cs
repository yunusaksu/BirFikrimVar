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

        public JsonResult Like(int postId, int userId)
        {
            var dahaOnceDislikeVarMi = entities.Dislike.Where(x => x.PostId == postId).Any(x => x.UserId == userId);
            var dahaOnceLikeVarMi = entities.Like.Where(x => x.PostId == postId).Any(x => x.UserId == userId);
            if (dahaOnceLikeVarMi == false)
            {
                Like like = new Like { PostId = postId, UserId = userId };
                entities.Like.Add(like);
                entities.SaveChanges();
                var sum = entities.Like.Where(x => x.PostId == postId).Count(); // postId'sı bu olan mesajı kaç farklı kullanıcı beğenmiş demektir bu kullanım.
                if (dahaOnceDislikeVarMi == true)
                {
                    var dahaOncekiDislikenDislikeIdsi = entities.Dislike.FirstOrDefault(x => x.UserId == userId && x.PostId == postId).DislikeID;
                    var eskidislike = entities.Dislike.Where(x => x.DislikeID == dahaOncekiDislikenDislikeIdsi).Single(); // dislike entitisini sectik
                    entities.Dislike.Remove(eskidislike);
                    entities.SaveChanges();
                    var sum2 = entities.Dislike.Where(x => x.PostId == postId).Count();
                    return Json(new { result = sum, result2 = sum2 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var sum2 = entities.Dislike.Where(x => x.PostId == postId).Count();
                    return Json(new { result = sum, result2 = sum2 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var sum2 = entities.Dislike.Where(x => x.PostId == postId).Count();
                var sum = entities.Like.Where(x => x.PostId == postId).Count();
                return Json(new { result = sum, result2 = sum2, result3 = "Daha önce begendin!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Dislike(int postId, int userId)
        {
            var dahaOnceDislikeVarMi = entities.Dislike.Where(x => x.PostId == postId).Any(x => x.UserId == userId); // boolean değer döndürüyor bize
            var dahaOnceLikeVarMi = entities.Like.Where(x => x.PostId == postId).Any(x => x.UserId == userId);

            if (dahaOnceDislikeVarMi == false)
            {
                Dislike dislike = new Dislike { PostId = postId, UserId = userId };
                entities.Dislike.Add(dislike);
                entities.SaveChanges();
                var sum2 = entities.Dislike.Where(x => x.PostId == postId).Count();
                if (dahaOnceLikeVarMi == true)
                {
                    var dahaOncekiLikenLikeIdsi = entities.Like.FirstOrDefault(x => x.UserId == userId && x.PostId == postId).LikeID;
                    var eskiLike = entities.Like.Where(x => x.LikeID == dahaOncekiLikenLikeIdsi).Single();
                    entities.Like.Remove(eskiLike);
                    entities.SaveChanges();
                    var sum = entities.Like.Where(x => x.PostId == postId).Count();
                    return Json(new { result = sum, result2 = sum2 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var sum = entities.Like.Where(x => x.PostId == postId).Count();
                    return Json(new { result = sum, result2 = sum2 }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var sum2 = entities.Dislike.Where(x => x.PostId == postId).Count();
                var sum = entities.Like.Where(x => x.PostId == postId).Count();
                return Json(new { result = sum, result2 = sum2, result3 = "Daha önce dislike yaptın!" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
