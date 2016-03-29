using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentariumTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            DateTime nowPlus60h = now.AddHours(60);

            double diff1 = GetDifferenceInDays(now, nowPlus60h); // rezultat je 2.5
            double diff2 = GetDifferenceInDays(nowPlus60h, now); // rezultat je 2.5

            int[] elements = new int[] { 2, 2, 1, 3, 4, 5, 6, 5, 7, 8, 6 };
            int[] duplicates = Duplicates(elements);
        }

        // - nije specificirano kako točno ovo treba računati pa sa odabrao najjednostavniju opciju,
        //   ako treba napraviti izračun bez korištenja 'TimeOffset' klase ili uz neka druga ograničanja, molim vas navedite ih
        // - 'DateTime' je struktura i ne može biti null, tako da nema niti provjere null vrijednosti
        //   - kad bi 'dt1' i 'dt2' mogli biti null na početak funkcije bih dodao ovo:
        //     if (dt1 == null || dt2 == null) {
        //         return 0.0;
        //     }
        private static double GetDifferenceInDays(DateTime dt1, DateTime dt2)
        {
            return Math.Abs((dt2 - dt1).TotalDays);
        }

        public static int[] Duplicates(int[] original)
        {
            return original
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .Distinct()
                .ToArray();
        }
    }
}
