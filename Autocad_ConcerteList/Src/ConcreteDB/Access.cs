using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB
{
    public static class Access
    {
        //"kuznetsov_av"
        private static readonly List<string> _accessUsers = new List<string> { "LukashovaTS", AutoCAD_PIK_Manager.Env.CadManLogin };

        public static bool Success()
        {
            return _accessUsers.Contains(Environment.UserName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
