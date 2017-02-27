using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogTimeStartStopWpf.Helpers
{
    public class Intervalls : IEnumerable<Intervall>
    {
        private List<Intervall> _Intervalls;

        public Intervalls()
        {
            _Intervalls = new List<Intervall>();
        }

        public Intervalls(string s)
        {
            _Intervalls = new List<Intervall>();
            var arr = s.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                _Intervalls.Add(new Intervall(arr[i]));
            }
        }

        public Intervall this[int i]
        {
            get
            {
                return _Intervalls[i];
            }
        }

        public int Count
        {
            get
            {
                return _Intervalls.Count;
            }
        }

        public TimeSpan Ellapsed
        {
            get
            {
                var ellapsed = new TimeSpan(0, 0, 0);
                foreach (var i in _Intervalls)
                {
                    ellapsed = ellapsed.Add(i.Ellapsed);
                }
                return ellapsed;
            }
        }

        public double EllapsedAsDouble
        {
            get
            {
                var ellapsed = Ellapsed;
                var x = (double)ellapsed.Hours + (double)ellapsed.Minutes / 60.0;
                return x;
            }
        }

        public IEnumerator<Intervall> GetEnumerator()
        {
            return _Intervalls.GetEnumerator();
        }

        public void AddRange(IEnumerable<Intervall> items)
        {
            foreach(var item in items)
            {
                Add(item);
            }
        }

        public void Add(Intervall item)
        {
            for (int i = 0; i < _Intervalls.Count; i++)
            {
                if (Intervall.Intersect(_Intervalls[i], item) != null)
                {
                    throw new Exception(string.Format("{0} und {1} überlappen!", _Intervalls[i], item));
                }
            }
            int indexToInsert = -1;
            for (int i = 0; i < _Intervalls.Count; i++)
            {
                // _Intervalls[i]
                // [----][---------]               [-----i---]        [---------]     [---------]
                //                         item          
                //                  [-------------]
                if (item.From.CompareTo(_Intervalls[i].From) < 0)
                {
                    indexToInsert = i;
                    break;
                }
            }
            if (indexToInsert == -1)
            {
                _Intervalls.Add(item);
            }
            else
            {
                _Intervalls.Insert(indexToInsert, item);
            }
            Simplify();
        }

        public override string ToString()
        {
            if (_Intervalls == null || _Intervalls.Count == 0)
            {
                return "";
            }
            var s = _Intervalls[0].ToString();
            for (var i = 1; i < _Intervalls.Count; i++)
            {
                s += string.Format(",{0}", _Intervalls[i]);
            }
            return s;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void RemoveAt(int index)
        {
            _Intervalls.RemoveAt(index);
        }

        public void CutWith(Intervalls intervals)
        {
            var bHasChanges = true;
            while (bHasChanges)
            {
                bHasChanges = Cut(this, intervals);
            }
        }

        private bool Cut(Intervalls ii, Intervalls jj)
        {
            for (var k = 0; k < ii.Count; k++)
            {
                for (var l = 0; l < jj.Count; l++)
                {
                    Intervalls sub;
                    var bHasChanges = Cut(ii[k], jj[l], out sub);
                    if (bHasChanges)
                    {
                        ii.RemoveAt(k);
                        ii.AddRange(sub);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool Cut(Intervall i, Intervall j, out Intervalls sub)
        {
            sub = new Intervalls();
            if (j.To.Value.CompareTo(i.From) <= 0 || i.To.Value.CompareTo(j.From) <= 0)
            {                                           //           [-----i-----]
                return false;                           //  [--j--]                 [--j--]
            }
            else
            {
                if (j.From.CompareTo(i.From) < 0)               //        [--------i--------] 
                {                                               //   [xxxx[----j-----]        
                    j.From = i.From;
                }
                if (i.To.Value.CompareTo(j.To.Value) < 0)       //        [--------i--------]
                {                                               //                   [---j--]xxx]
                    j.To = i.To;
                }
                if (i.From.CompareTo(j.From) < 0)               //  [xxxxxx--i--------]      [xxxxxxxxxxx--i---]
                {                                               //        [---j--]                      [---j--]
                    sub.Add(new Intervall(i.From, j.From));
                }
                if (j.To.Value.CompareTo(i.To.Value) < 0)       //  [----i-xxxxxxxxxxx]      [--------i--xxxxxx]
                {                                               //  [---j--]                      [---j--]
                    sub.Add(new Intervall(j.To.Value, i.To.Value));
                }
                return true;
            }
        }

        public void Simplify()
        {
            var repeat = false;
            for (int i = 0; i < _Intervalls.Count - 1; i++)
            {
                if (_Intervalls[i].To.HasValue && _Intervalls[i].To.Value.ToString() == _Intervalls[i + 1].From.ToString())
                {
                    repeat = true;
                    //              i  
                    // [---]      [---][---][---]   [---][---]   [---]
                    // [---]      [--------][---]   [---][---]   [---]
                    _Intervalls[i].To = _Intervalls[i + 1].To;
                    _Intervalls.RemoveAt(i + 1);
                    break;
                }
            }
            if (repeat)
            {
                Simplify();
            }
        }
    }
}
