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
        public string ImageFilename { get; set; }
        public Bugs(string name, int sprayPrice, int daysUntilFlowerDies, string imageFilename)
        {
            Name = name;
            SprayPrice = sprayPrice;
            DaysUntilFlowerDies = daysUntilFlowerDies;
            ImageFilename = imageFilename;
        }
    }

    public class CommonBugs : Bugs
    {
        public CommonBugs() : base("Common bugs", 5, 3, "bugs.png") { }
    }

    public class GardenBugs : Bugs
    {
        public GardenBugs() : base("Garden bugs", 90, 3, "bugs.png") { }
    }

    public class ExoticBugs : Bugs
    {
        public ExoticBugs() : base("Exotic bugs", 250, 1, "bugs.png") { }
    }

    public class TropicalBugs : Bugs
    {
        public TropicalBugs() : base("Tropical Bugs", 350, 3, "bugs.png") { }
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

        public bool IsEqual(BugInfestation other)
        {
            if (other is null)
            {
                return false;
            }
            return Bugs == other.Bugs &&
                DaysUntilFlowerDies == other.DaysUntilFlowerDies;
        }

        public static bool IsEqual(BugInfestation bugs1, BugInfestation bugs2)
        {
            if (bugs1 is null &&  bugs2 is null)
            {
                return true;
            }
            if (bugs1 is null || bugs2 is null)
            {
                return false;
            }
            return bugs1.Equals(bugs2);
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
