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
        List<Filo_Takip_REG01> Filo_Takip_REG01();
        List<View_OprOgsTanim_REGOResponse> GetHGSDefineList(short type);
        void InsertHgsResult(HGS_INTEGRATION_RESULT HGS_INTEGRATION_RESULT);
    }
}
