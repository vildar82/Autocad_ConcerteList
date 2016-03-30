using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Model.ConcreteDB
{
   public static class Access
   {
      private static List<string> _accessUsers = new List<string>() { "LukashovaTS", "khisyametdinovvt", "kuznetsov_av" };

      public static bool Success()
      {
         return _accessUsers.Contains(Environment.UserName, StringComparer.OrdinalIgnoreCase);
      }
   }
}
