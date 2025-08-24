using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public abstract class Bugs
    {
        public int SprayPrice { get; set; }
        public int DaysUntilFlowerDies { get; set; }
        public Bugs(int sprayPrice, int daysUntilFlowerDies)
        {
            SprayPrice = sprayPrice;
            DaysUntilFlowerDies = daysUntilFlowerDies;
        }
    }

    public class CommonBugs : Bugs
    {
        public CommonBugs() : base(5, 3) { }
    }
}
