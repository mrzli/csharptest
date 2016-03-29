using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeDisplay
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            //DateTime endTime = new DateTime(2058, 10, 27, 17, 40, 0);
            //DateTime endTime = new DateTime(2044, 8, 23, 9, 30, 0);
            DateTime endTime = new DateTime(2016, 3, 29, 15, 33, 0);

            double totalMilliseconds = (endTime - startTime).TotalMilliseconds;

            while (true)
            {
                DateTime currentTime = DateTime.Now;
                double elapsedMilliseconds = (currentTime - startTime).TotalMilliseconds;
                double remainingSeconds = Math.Max((endTime - currentTime).TotalSeconds, 0.0);
                double percent = Math.Min(elapsedMilliseconds / totalMilliseconds * 100.0, 100.0);
                Console.Write("\rRemaining: {0:0.0} s; Since '{1:yyyy.MM.dd-HH:mm:ss.fff}': {2:##0.00000000}%     ", remainingSeconds, startTime, percent);
            }
        }
    }
}
