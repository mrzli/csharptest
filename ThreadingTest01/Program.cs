using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingTest01
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;

            //Parallel.For(0, 100000, new ParallelOptions { MaxDegreeOfParallelism = 100 }, i => count++); // BAD: count++ is not atomic
            Parallel.For(0, 100000, new ParallelOptions { MaxDegreeOfParallelism = 100 }, i => Interlocked.Increment(ref count));

            Console.WriteLine(count);
            Console.ReadKey();
        }
    }
}
