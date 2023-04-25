using RntCar.ClassLibrary.Estepe;
using RntCar.ClassLibrary.HGS;
using RntCar.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RntCar.BusinessLibrary.Estepe
{
    public class EstepeBL:IEstepeBL
    {

        IDB _db;
        public EstepeBL()
        { 
            this._db = new DAL.DAL();
        }

        public List<Filo_Takip_REG01> Filo_Takip_REG01()
        {
            return (List<Filo_Takip_REG01>)_db.Cmd<Filo_Takip_REG01>(
                                "SELECT plakano as PlakaNo, plaka as Plaka, markano as MarkaNo, MarkaAd, tipno as TipNo," +
                                        "TipAd, versiyonno as VersiyonNo, VersiyonAd, opsiyonadx as OpsiyonAd, " +
                                        "tversiyonno2 as TVersiyonNo2, model as Model, sasino as SasiNo, kod1 as Kod1, Kategori1 " +
                                "FROM dbo.view_Opr_Filo_Takip_REGO1 with(NOLOCK) "+
                                "where Kod1 in('2799','2800')");


        }

        public List<View_OprOgsTanim_REGOResponse> GetHGSDefineList(short type)
        {
            var result = _db.Cmd<View_OprOgsTanim_REGOResponse>($"otofilo_entegre.dbo.SP_RNTGO_HGS_ETIKETLERI {type}").ToList();
            return result;
        }

        public void InsertHgsResult(HGS_INTEGRATION_RESULT HGS_INTEGRATION_RESULT)
        {
            _db.CmdExec($"otofilo_entegre.dbo.SP_RNTGO_HGS_ETIKET_RESULT_EKLE {HGS_INTEGRATION_RESULT.OGS_KGS_NO}, '{HGS_INTEGRATION_RESULT.PROCCESS_TYPE}', {HGS_INTEGRATION_RESULT.RESULT}, '{HGS_INTEGRATION_RESULT.EXCEPTION_DETAIL}' ");
        }
    }
}
