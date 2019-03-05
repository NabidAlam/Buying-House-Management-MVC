using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHMS.Helpers
{
    public static class NullHelpers
    {
        public static string DateToString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            if ((DateTime)obj == DateTime.MinValue)
            {
                return "";
            }

            return ((DateTime)obj).ToString("dd/MM/yyyy");
        }

   


    }
}