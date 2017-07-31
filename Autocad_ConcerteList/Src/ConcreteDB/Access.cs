using System;
using System.Collections.Generic;
using System.Linq;

namespace Autocad_ConcerteList.ConcreteDB
{
    public static class Access
    {
        //"kuznetsov_av"
        private static readonly List<string> _accessUsers = new List<string> { "LukashovaTS"};

        public static bool Success()
        {
	        return true;
            //return _accessUsers.Contains(Environment.UserName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
