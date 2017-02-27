using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DialogTimeStartStopWpf.Helpers
{
    public struct Time : IComparable<Time>
    {
        private int _h, _m;
         
        public Time(int h, int m)
        {
            _h = h;
            _m = m;
        }
        public Time(string s)
        {
            var arr = s.Split(':'); // arr1[0] = "09" arr1[1] = "10"
            _h = int.Parse(arr[0]);
            _m = int.Parse(arr[1]);
        }

        public int Hour
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
            }
        }

        public int Minute
        {
            get
            {
                return _m;
            }
            set
            {
                _m = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}", _h, _m);
        }

        public int CompareTo(Time other)
        {
            if (this.Hour < other.Hour)
            {
                return -1;
            }
            if (this.Hour > other.Hour)
            {
                return 1;
            }
            if (this.Minute < other.Minute)
            {
                return -1;
            }
            if (this.Minute > other.Minute)
            {
                return 1;
            }
            return 0;
        }
    }
}
