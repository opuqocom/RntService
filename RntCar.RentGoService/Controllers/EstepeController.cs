using RntCar.BusinessLibrary.Estepe;
using RntCar.ClassLibrary.Estepe;
using RntCar.DAL;
using RntCar.RentGoService.Authentication.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RntCar.RentGoService.Controllers
{
    public class EstepeController : ApiController
    {

        IEstepeBL _estepeBL;
        public EstepeController()
        {
            _estepeBL = new EstepeBL();
        }

        /// <summary>
        /// Araç filo listesi. (Token zorunlu)
        /// </summary>
        [JWTAuthentication]
        [HttpGet]
        public List<Filo_Takip_REG01> Filo_Takip_REG01()
        {
            return _estepeBL.Filo_Takip_REG01();
        }
    }
}