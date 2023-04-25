using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RntCar.DAL
{
    public interface IDB
    {
        IDbConnection ConnectionGet();
        IEnumerable<T> Cmd<T>(string query);
        int CmdExec(string query);
    }
}
