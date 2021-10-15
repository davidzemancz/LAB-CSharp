using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class Assert
    {
        public static void IsTrue(bool expr)
        {
            if (!expr) throw new Exception("Expression is false");
        }
    }
}
