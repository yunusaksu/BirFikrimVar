using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FikrimVar.Models
{
    public class Mesajim
    {
        public string MesajBaslik { get; set; }
        [Required(ErrorMessage = "Bu alan boş geçilemez")]
        [MinLength(20, ErrorMessage = "İçerik az")]
        public string MesajIcerik { get; set; }
        public int KategoriId { get; set; }
    }
}