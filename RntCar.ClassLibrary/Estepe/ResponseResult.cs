using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RntCar.ClassLibrary.Estepe
{
    public class ResponseResult
    {
        public bool Result { get; set; }
         
        public String ExceptionDetail { get; set; }


        public string Id { get; set; }

        public static ResponseResult ReturnSuccess()
        {
            return new ResponseResult { Result = true };
        }

        public static ResponseResult ReturnError(String Detail)
        {
            return new ResponseResult { Result = false, ExceptionDetail = Detail };
        }

        public static ResponseResult ReturnSuccessWithId(string id)
        {
            return new ResponseResult { Result = true, Id = id };
        }
    }
}
