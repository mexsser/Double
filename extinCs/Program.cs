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
            set
            {
                if (parents.Count == 0)
                {
                    _val = value;
                    Publisher?.Invoke();
                }
                else
                    Console.WriteLine("Current element isn't base, set operation will be omitted.");
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
            if (parents.Count != 0)
            {
                _val = Cal(parents);
                Tag = false;
                foreach (var parent in parents)
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

        private static Double Operator_builder(Double D1, Double D2, Func<List<Double>, double> Op)
        {
            Double D3 = new Double { parents = new List<Double> { D1, D2 } };
            D3.Subscribe();
            D3.Cal += new Rel(Op);
            return D3;
        }

        // implicit cast double to Double will allow the operators to handle both double- and Double-typed operands, even symmetrically.
        public static implicit operator Double(double d) 
        {
            return new Double(d);
        }

        public static Double operator +(Double D1, Double D2)
        {
            double Plus(List<Double> parents_) => parents_[0].Val + parents_[1].Val;
            return Operator_builder(D1, D2, Plus);
        }
        public static Double operator -(Double D1, Double D2)
        {
            double Minus(List<Double> parents_) => parents_[0].Val - parents_[1].Val;
            return Operator_builder(D1, D2, Minus);
        }
        public static Double operator *(Double D1, Double D2)
        {
            double Product(List<Double> parents_) => parents_[0].Val * parents_[1].Val;
            return Operator_builder(D1, D2, Product);
        }
        public static Double operator /(Double D1, Double D2)
        {
            double Divide(List<Double> parents_) => parents_[0].Val / parents_[1].Val; // if divided by zero, will return infinity instead of error.
            return Operator_builder(D1, D2, Divide);
        }

        public static void Main(string[] args)
        {
            Double D0 = new Double(0.0);
            Double D1 = new Double(1.0);
            Double D2 = new Double(2.0);
            Double D3 = new Double(3.0);
            Double D4 = D1 + 2; // "=" does a reference type assignment
            Double D5 = D3 + D4;
            Double D6 = D5 / D2;
            Double D7 = D6 * 1.6;

            Console.WriteLine("Round 1: {0}", D7.Val);
            D2.Val = 3;
            Console.WriteLine("Round 2: {0}", D7.Val);
            D5.Release();
            D3.Val = 4;
            Console.WriteLine("Round 3: {0}", D7.Val);
            Console.ReadKey();
        }

    }
}
