using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public abstract class Bugs
    {
        public string Name { get; private set; }
        public int SprayPrice { get; set; }
        public int DaysUntilFlowerDies { get; set; }
        public Bugs(string name, int sprayPrice, int daysUntilFlowerDies)
        {
            Name = name;
            SprayPrice = sprayPrice;
            DaysUntilFlowerDies = daysUntilFlowerDies;
        }
    }

    public class CommonBugs : Bugs
    {
        public CommonBugs() : base("Common bugs", 5, 3) { }
    }

    public class GardenBugs : Bugs
    {
        public GardenBugs() : base("Garden bugs", 90, 3) { }
    }

    public class ExoticBugs : Bugs
    {
        public ExoticBugs() : base("Exotic bugs", 250, 1) { }
    }

    public class TropicalBugs : Bugs
    {
        public TropicalBugs() : base("Tropical Bugs", 350, 3) { }
    }

    public static class BugTypes
    {
        public static Bugs CommonBugs = new CommonBugs();
        public static Bugs GardenBugs = new GardenBugs();
        public static Bugs ExoticBugs = new ExoticBugs();
        public static Bugs TropicalBugs = new TropicalBugs();
    }

    public class BugInfestation
    {
        public Bugs Bugs { get; private set; }
        public int DaysUntilFlowerDies { get; private set; }

        public BugInfestation(Bugs bugs)
        {
            Bugs = bugs;
            DaysUntilFlowerDies = bugs.DaysUntilFlowerDies;
        }

        public void Update()
        {
            DaysUntilFlowerDies -= 1;
        }
    }
    
    public class BugsWeight<Bugs> : IWeightedItem<Bugs>
    {
        public Bugs Item { get; private set; }
        public int Weight { get; private set; }

        public BugsWeight(Bugs bugs, int weight)
        {
            Item = bugs;
            Weight = weight;
        }
    }
}
