using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualGarden
{
    /// <summary>
    /// The abstract class from which all Flowers inherit.
    /// The Flower classes consist of information common for all instances of said flower.
    /// </summary>
    public abstract class Flower
    {
        /// <summary>
        /// The name of this type of flower
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The amount of money needed to buy seeds of this flower
        /// </summary>
        public int SeedPrice { get; private set; }

        /// <summary>
        /// The number of days this flower can go without being watered.
        /// </summary>
        public int DaysBetweenWatering { get; private set; }

        /// <summary>
        /// The number of days this flower takes to get into the blooming state when conditions for growth are met each day.
        /// Including the day the flower was planted.
        /// </summary>
        public int GrowthDays { get; private set; }

        /// <summary>
        /// The number of days this flower blooms and produces coins when conditions for blooming are met each day.
        /// </summary>
        public int BloomDays { get; private set; }

        /// <summary>
        /// The number of coins this flower produces in one day while blooming.
        /// </summary>
        public int DailyBloomIncome { get; private set; }

        /// <summary>
        /// A list of weights for each bug type signifying how likely is that bugtype to be in a bug infestation on this flower.
        /// </summary>
        public abstract List<BugsWeight<Bugs>> BugWeights { get; }

        /// <summary>
        /// The filename of a file containing an image of this flower.
        /// </summary>
        public string ImageFilename { get; private set; }

        /// <summary>
        /// Maximum number of coins gained from the flower if it is not destroyed prematurely and the player collects coins from it every day.
        /// </summary>
        public int MaximumIncome
        {
            get
            {
                return BloomDays * DailyBloomIncome;
            }
        }

        protected Flower(string name, int seedPrice, int daysBetweenWatering, int growthDays, int bloomDays, int dailyBloomIncome, string imageFilename) 
        { 
            Name = name;
            SeedPrice = seedPrice;
            DaysBetweenWatering = daysBetweenWatering;
            GrowthDays = growthDays;
            BloomDays = bloomDays;
            DailyBloomIncome = dailyBloomIncome;
            ImageFilename = imageFilename;
        }
    }

    public class Daisy : Flower
    {
        public override List<BugsWeight<Bugs>> BugWeights { get; } = new List<BugsWeight<Bugs>>
        { 
            new BugsWeight<Bugs>(BugTypes.CommonBugs, 10),
            new BugsWeight<Bugs>(BugTypes.GardenBugs, 0),
            new BugsWeight<Bugs>(BugTypes.ExoticBugs, 0),
            new BugsWeight<Bugs>(BugTypes.TropicalBugs, 0),
        };
        public Daisy() : base("Daisy", 5, 0, 5, 3, 5, "daisy.png") { }
    }

    public class Marigold : Flower
    {
        public override List<BugsWeight<Bugs>> BugWeights { get; } = new List<BugsWeight<Bugs>>
        {
            new BugsWeight<Bugs>(BugTypes.CommonBugs, 10),
            new BugsWeight<Bugs>(BugTypes.GardenBugs, 0),
            new BugsWeight<Bugs>(BugTypes.ExoticBugs, 0),
            new BugsWeight<Bugs>(BugTypes.TropicalBugs, 0),
        };
        public Marigold() : base("Marigold", 20, 1, 6, 6, 5, "marigold.png") { }
    }

    public class Sunflower : Flower
    {
        public override List<BugsWeight<Bugs>> BugWeights { get; } = new List<BugsWeight<Bugs>>
        {
            new BugsWeight<Bugs>(BugTypes.CommonBugs, 9),
            new BugsWeight<Bugs>(BugTypes.GardenBugs, 1),
            new BugsWeight<Bugs>(BugTypes.ExoticBugs, 0),
            new BugsWeight<Bugs>(BugTypes.TropicalBugs, 0),
        };
        public Sunflower() : base("Sunflower", 50, 2, 8, 10, 13, "sunflower.png") { }
    }

    public class Petunia : Flower
    {
        public override List<BugsWeight<Bugs>> BugWeights { get; } = new List<BugsWeight<Bugs>>
        {
            new BugsWeight<Bugs>(BugTypes.CommonBugs, 8),
            new BugsWeight<Bugs>(BugTypes.GardenBugs, 2),
            new BugsWeight<Bugs>(BugTypes.ExoticBugs, 0),
            new BugsWeight<Bugs>(BugTypes.TropicalBugs, 0),
        };
        public Petunia() : base("Petunia", 80, 1, 9, 10, 20, "petunia.png") { }
    }

    public class Rose : Flower
    {
        public override List<BugsWeight<Bugs>> BugWeights { get; } = new List<BugsWeight<Bugs>>
        {
            new BugsWeight<Bugs>(BugTypes.CommonBugs, 2),
            new BugsWeight<Bugs>(BugTypes.GardenBugs, 6),
            new BugsWeight<Bugs>(BugTypes.ExoticBugs, 1),
            new BugsWeight<Bugs>(BugTypes.TropicalBugs, 1),
        };
        public Rose() : base("Rose", 300, 0, 20, 5, 180, "rose.png") { }
    }

    public class Tulip : Flower
    {
        public override List<BugsWeight<Bugs>> BugWeights { get; } = new List<BugsWeight<Bugs>>
        {
            new BugsWeight<Bugs>(BugTypes.CommonBugs, 2),
            new BugsWeight<Bugs>(BugTypes.GardenBugs, 6),
            new BugsWeight<Bugs>(BugTypes.ExoticBugs, 0),
            new BugsWeight<Bugs>(BugTypes.TropicalBugs, 2),
        };
        public Tulip() : base("Tulip", 500, 1, 12, 8, 130, "tulip.png") { }
    }

    //The FlowerTypes class contains instances of all flowers that inherit from Flower
    public static class FlowerTypes
    {
        public static Flower Daisy = new Daisy();
        public static Flower Marigold = new Marigold();
        public static Flower Sunflower = new Sunflower();
        public static Flower Petunia = new Petunia();
        public static Flower Rose = new Rose();
        public static Flower Peonies = new Tulip();
    }

    //The PlantedFlower class contains info specific to instances of a flower planted
    public class PlantedFlower
    {
        //The type of which this flower is (e.g. Daisy, Rose), contains info that is the same for every flower of this type
        public Flower FlowerType { get; private set; }

        //The number of days this flower has grown
        public int GrowthDays { get; set; }

        //The number of days this flower has already bloomed and produced income
        public int BloomDays { get; set; }

        //The current state of this planted flower
        public FlowerState State { get; set; }

        /*Number of days the flower has not been watered. Starts at 1 after being planted because
        DaysSinceLastWatered is updated after the flower states so that flowers are properly killed.*/
        public int DaysSinceLastWatered { get; set;}

        public PlantedFlower(Flower flower)
        {
            FlowerType = flower;
            GrowthDays = 0;
            BloomDays = 0;
            State = FlowerState.Growing;
            DaysSinceLastWatered = 1;
        }

        public bool IsEqual(PlantedFlower other)
        {
            if (other is null)
            {
                return false;
            }
            return FlowerType == other.FlowerType &&
                GrowthDays == other.GrowthDays &&
                BloomDays == other.BloomDays &&
                State == other.State &&
                DaysSinceLastWatered == other.DaysSinceLastWatered;
        }

        public static bool IsEqual(PlantedFlower flower1, PlantedFlower flower2)
        {
            if (flower1 is null && flower2 is null)
            {
                return true;
            }
            if (flower1 is null || flower2 is null)
            {
                return false;
            }
            return flower1.IsEqual(flower2);
        }
    }

    //The possible states a planted flower can be in
    public enum FlowerState
    {
        Growing,
        Blooming,
        Dead
    }
}
