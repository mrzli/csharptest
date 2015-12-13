using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClosureTest01
{
    class Program
    {
        public class Closure1
        {
            public int i;
            public void Anonymous()
            {
                Console.WriteLine(i);
            }
        }

        private static Action lambda;

        static void Main(string[] args)
        {
            // closure
            {
                int i = 42;
                lambda = () => { Console.WriteLine(i); };

                lambda();
                Method();
                i = 11;
                lambda();
                Method();
            }

            lambda();

            // equivalent to
            {
                Closure1 closure = new Closure1();
                closure.i = 42;
                lambda = closure.Anonymous;
                lambda();
                Method();
                closure.i = 11;
                lambda();
                Method();
            }

            lambda();

            Console.ReadKey();
        }

        private static void Method()
        {
            int i = 15;
            lambda();
        }

        // another example
        public Action Counter()
        {
            int count = 0;
            Action counter = () =>
            {
                count++;
            };

            return counter;
        }

        public class Local
        {
            public int count;
            public void Anonymous()
            {
                count++;
            }
        }

        public Action Counter2()
        {
            Local local = new Local();
            local.count = 0;
            Action counter = local.Anonymous;
            return counter;
        }
    }
}
