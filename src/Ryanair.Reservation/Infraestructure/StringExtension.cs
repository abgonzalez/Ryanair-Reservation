using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Infraestructure
{
    public static class StringExtension
    {
        public static DateTime? StringToDate(this string s)
        {
            try { 

                if (!Regex.IsMatch(s, @"^\d{4}\-(0?[1-9]|1[012])\-(0?[1-9]|[12][0-9]|3[01])$"))
                    return null;
                string[] datesArray = s.Split("-");
                int year = Int16.Parse(datesArray[0]);
                int month = Int16.Parse(datesArray[1]);
                int day = Int16.Parse(datesArray[2]);
                return new DateTime(year, month, day); 
            }
            catch (Exception e)
            {
                throw  e;
            }

        }
    }
}
