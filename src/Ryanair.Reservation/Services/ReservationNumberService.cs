using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ryanair.Reservation.Services
{
    public class ReservationNumberService : IReservationNumberService
    {
        string _letters;

        int _numbers;
        public string Value
        {
            get
            {
                return _letters + _numbers.ToString("000");
            }
        }
        public ReservationNumberService()
        {
            _letters = "AAA";
            _numbers = 0;
        }
        public string Next()
        {
            if (_numbers == 999)
            {
                _numbers = 001;
                _letters=IncreaseLetters();
            }
            else
            {
                ++_numbers;
            }
            return _letters + _numbers.ToString("000");
        }
        public string IncreaseLetters()
        {
            char[] charLetters = _letters.ToArray();
            for (int i = charLetters.Length-1; i >= 0; i--)
            {
                if (charLetters[i] >= 65 && charLetters[i] < 90)
                {
                    ++charLetters[i];
                    return new string(charLetters);
                }
            }
            throw new OverflowException("Overflow on Reservation Number");
        }
    }
}
