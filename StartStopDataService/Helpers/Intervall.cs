using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartStopDataService.Helpers
{
    public class Intervall
    {
        private Time _from;
        private Time? _to;

        public Intervall(Time from, Time to)
        {
            _from = from;
            _to = to;
        }

        public Intervall(Time from)
        {
            _from = from;
            _to = null;
        }

        public Intervall(string s)
        {
            var arr = s.Split('-', 'p'); // arr[0] = "09:10" arr[1] = "14:45"
            _from = new Helpers.Time(arr[0]);
            _to = null;
            if (!string.IsNullOrEmpty(arr[1]))
            {
                _to = new Helpers.Time(arr[1]);
            }
        }

        public Time From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        public Time? To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }

        public TimeSpan Ellapsed
        {
            get
            {
                if (_to.HasValue)
                {
                    var h = _to.Value.Hour - _from.Hour;
                    var m = _to.Value.Minute - _from.Minute;
                    if (m < 0)
                    {
                        h--;
                        m += 60;
                    }
                    return new TimeSpan(h, m, 0);
                }
                return new TimeSpan(0, 0, 0);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", _from, _to == null ? "" : _to.Value.ToString());
        }

        public static Intervall Intersect(Intervall i1, Intervall i2)
        {
            Intervall result = null;

            if (i1.To == null)
            {
                if (i2.To == null)
                {
                    if (i2.From.CompareTo(i1.From) < 0)
                    {
                        //                              i1
                        //                    [-------------------------------------------  
                        //      i2
                        // [--------------------------------------------------------------
                        result = new Intervall(i1.From);
                    }
                    else
                    {
                        //                              i1
                        //                    [-------------------------------------------  
                        //                                       i2
                        //                             [----------------------------------
                        result = new Intervall(i2.From);
                    }
                }
                else
                {
                    //                              i1
                    //                    [-------------------------------------------  
                    //      i2
                    // [--------------]
                    if (i2.To.Value.CompareTo(i1.From) <= 0)
                    {
                        result = null;
                    }
                    else
                    {
                        if (i2.From.CompareTo(i1.From) < 0)
                        {
                            //                          i1
                            //                [-------------------------------------------  
                            //      i2
                            // [--------------=====================]
                            result = new Intervall(i1.From, i2.To.Value);
                        }
                        else
                        {
                            //                              i1
                            //                    [-------------------------------------------  
                            //                                i2
                            //                        [================]
                            result = new Intervall(i2.From, i2.To.Value);
                        }
                    }
                }
            }
            else
            {
                if (i2.To == null)
                {
                    if (i2.From.CompareTo(i1.From) < 0)
                    {
                        //                            i1
                        //                    [----------------]  
                        //      i2
                        // [--------------------------------------------------------------
                        result = new Intervall(i1.From, i1.To.Value);
                    }
                    else 
                    {
                        if (i2.From.CompareTo(i1.To.Value) < 0)
                        {
                            //           i1
                            // [---------------]  
                            //                    i2
                            //          [----------------------------------
                            result = new Intervall(i2.From, i1.To.Value);
                        }
                        else
                        {
                            //           i1
                            // [---------------]  
                            //                                    i2
                            //                       [----------------------------------
                            return null;
                        }
                    }
                }
                else
                {
                    var from = i1.From.CompareTo(i2.From) < 0 ? i2.From : i1.From;
                    var to = i1.To.Value.CompareTo(i2.To.Value) < 0 ? i1.To.Value : i2.To.Value;

                    //               >>   <<
                    //                              i1
                    //                    [-------------------]  
                    //      i2
                    // [--------------]

                    //                    << >>
                    //                              i1
                    //                    [-------------------]  
                    //      i2
                    // [----------------------]

                    //                    <<                 >>
                    //                    [-------------------]  
                    //       i2
                    // [------------------------------------------------]

                    //          <<    >>
                    //            i1
                    // [-------------------]  
                    //             i2
                    //          [-------]

                    //          <<        >>
                    //           i1
                    // [-------------------]  
                    //                     i2
                    //          [----------------------]

                    //                    >>   <<
                    //          i1
                    // [-------------------]  
                    //                                  i2
                    //                         [-----------------]

                    if (from.CompareTo(to) < 0)
                    {
                        result = new Intervall(from, to);
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
            return result;
        }
    }
}
