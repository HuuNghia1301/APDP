using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Caculator
    {
        public int Add(int a, int b) { return a + b; }
        public int Sub(int a, int b) { return a - b; }
        public int Mul(int a, int b) { return a * b; }
        public double Div(int a, int b)
            {
            if (b == 0)
            
                throw new DivideByZeroException("Cannot divide by zero");
                return a / b;
            
            }
        }
    }
