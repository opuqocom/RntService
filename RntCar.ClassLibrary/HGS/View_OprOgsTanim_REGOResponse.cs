using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RntCar.ClassLibrary.HGS
{
    public class View_OprOgsTanim_REGOResponse
    {
        public int ogskgsno { get; set; }
        public string plaka { get; set; }
        public string serino { get; set; }
        public string OgsFirmaad { get; set; }
        public string saticihesapkodu { get; set; }
        public int? durum { get; set; }
        public DateTime? takma_tarihi { get; set; }
        public DateTime? sokme_tarihi { get; set; }
        public DateTime? credate { get; set; }
        public string carihesapadi { get; set; }
        public string entegrekod { get; set; }
        public string ruhsat_tescil_belgeno { get; set; }
        public string KitTipAd { get; set; }

    }
}
