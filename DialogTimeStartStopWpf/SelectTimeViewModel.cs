using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogTimeStartStopWpf
{
    public class SelectTimeViewModel : ViewModelBase
    {
        private int _Hour;
        private int _Minute;
        private DateTime _Datum;

        public DateTime Datum
        {
            get { return _Datum; }
            set
            {
                _Datum = value;
                RaisePropertyChanged("Datum");
            }
        }

        public int Hour
        {
            get { return _Hour; }
            set
            {
                _Hour = value;
                RaisePropertyChanged("Hour");
            }
        }
        public int Minute
        {
            get { return _Minute; }
            set
            {
                _Minute = value;
                RaisePropertyChanged("Minute");
            }
        }

        internal bool IsValid()
        {
            return 0 <= _Hour && _Hour < 24 &&
                   0 <= _Minute && _Minute < 60;
        }
    }
}
