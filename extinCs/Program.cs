using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datetype
{
    public delegate void SetTag();
    public delegate double Rel(List<Double> parents_);
    public class Double
    {
        public event SetTag Publisher;  // as a list of Refresh() foctors of children
        private event Rel Cal;
        private List<Double> parents = new List<Double>();
        private bool Tag = true;
        private double _val = 0.0;
        public double Val
        {
            get
            {
                if (Tag && Cal != null)
                {
                    _val = Cal(parents);
                    Tag = false;
                }
                return _val;
            }
            private set
            {
                _val = value;
                Publisher?.Invoke();
            }
        }

        public Double() { }
        public Double(double d1) 
        {
            Val = d1;
            Tag = false;
        }
        public void Settag()
        {
            Tag = true;
            Publisher?.Invoke();
        }
        public void Release()
        {
            Val = Val;
            if(parents.Count != 0)
            { 
                foreach(var parent in parents)
                    parent.Publisher -= new SetTag(this.Settag);
                parents = new List<Double>();
            }
        }

        private void Subscribe()
        {
            if(parents.Count != 0)
                foreach (var parent in parents)
                    parent.Publisher += new SetTag(this.Settag);
        }
        public static Double operator +(Double D1, Double D2)
        {
            Double D3 = new Double { parents = new List<Double> { D1, D2 } };
            D3.Subscribe();

            double Plus(List<Double> parents_)
            {
                return parents_[0].Val + parents_[1].Val;
            }

            D3.Cal += new Rel(Plus);
            return D3;
        }

        public static void Main(string[] args)
        {
            Double D1 = new Double(1.0);
            Double D2 = new Double(2.0);
            Double D3 = new Double(3.0);
            Double D4 = D1 + D2;
            Double D5 = D3 + D4;

            Console.WriteLine("Round 1: {0}", D5.Val);
            D2.Val = 3;
            Console.WriteLine("Round 2: {0}", D5.Val);
            D5.Release();
            D2.Val = 4;
            Console.WriteLine("Round 3: {0}", D5.Val);
            Console.ReadKey();
        }




    }
}
