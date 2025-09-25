using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public abstract class BugType
    {
        public string Name { get; private set; }
        public int SprayPrice { get; set; }
        public int DaysUntilFlowerDies { get; set; }
        public string ImageFilename { get; set; }
        public BugType(string name, int sprayPrice, int daysUntilFlowerDies, string imageFilename)
        {
            Name = name;
            SprayPrice = sprayPrice;
            DaysUntilFlowerDies = daysUntilFlowerDies;
            ImageFilename = imageFilename;
        }
    }

    public class CommonBugs : BugType
    {
        public CommonBugs() : base("Common bugs", 5, 3, "bugs.png") { }
    }

    public class GardenBugs : BugType
    {
        public GardenBugs() : base("Garden bugs", 90, 3, "bugs.png") { }
    }

    public class ExoticBugs : BugType
    {
        public ExoticBugs() : base("Exotic bugs", 250, 1, "bugs.png") { }
    }

    public class TropicalBugs : BugType
    {
        public TropicalBugs() : base("Tropical Bugs", 350, 3, "bugs.png") { }
    }

    public static class BugTypes
    {
        public static BugType CommonBugs = new CommonBugs();
        public static BugType GardenBugs = new GardenBugs();
        public static BugType ExoticBugs = new ExoticBugs();
        public static BugType TropicalBugs = new TropicalBugs();
    }

    public class BugInfestation
    {
        public BugType Bugs { get; private set; }
        /// <summary>
        /// The name of the flower type of this planted flower.
        /// </summary>
        [Save, NotLoad]
        public string? BugsTypeName => Bugs.Name;
        [Save]
        public int DaysUntilFlowerDies { get; private set; }

        private BugInfestation() { }
        public BugInfestation(BugType bugs)
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
    
    public class BugsWeight : IWeightedItem<BugType>
    {
        public BugType Item { get; private set; }
        public int Weight { get; private set; }

        public BugsWeight(BugType bugs, int weight)
        {
            Item = bugs;
            Weight = weight;
        }
    }
}
