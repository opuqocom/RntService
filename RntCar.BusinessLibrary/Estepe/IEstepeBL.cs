using RntCar.ClassLibrary.Estepe;
using RntCar.ClassLibrary.HGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RntCar.BusinessLibrary.Estepe
{
    public interface IEstepeBL
    {
        List<Filo_Takip_REG01> Filo_Takip_REG01();//veritabanından belirli bir sorgu çalıştırarak bir liste döndürür. Bu liste, Filo_Takip_REG01 adlı bir sınıftan nesneler içerir.
        List<View_OprOgsTanim_REGOResponse> GetHGSDefineList(short type);////veritabanındaki bir saklı prosedürü çağırarak belirli bir tipte HGS tanımları listesi döndürür.
        void InsertHgsResult(HGS_INTEGRATION_RESULT HGS_INTEGRATION_RESULT);//InsertHgsResult metodu ise, HGS entegrasyonu sonucunda elde edilen sonuçları veritabanına kaydetmek için kullanılır.
    }
}
