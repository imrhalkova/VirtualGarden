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
    /// The FlowerType classes consist of information common for all instances of said flower.
    /// </summary>
    public abstract class FlowerType
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
        public abstract List<BugsWeight> BugWeights { get; }

        /// <summary>
        /// The filename of a file containing an image of this flower blooming.
        /// </summary>
        public string BloomingImageFilename { get; private set; }

        /// <summary>
        /// The filename of a file containing an image of this flower growing.
        /// </summary>
        public string GrowingImageFilename { get; private set; } = "growing_flower.png";

        /// <summary>
        /// The filename of a file containing an image of this flower dead.
        /// </summary>
        public string DeadImageFilename { get; private set; } = "dead_flower.png";

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

        protected FlowerType(string name, int seedPrice, int daysBetweenWatering, int growthDays, int bloomDays, int dailyBloomIncome, string imageFilename) 
        { 
            Name = name;
            SeedPrice = seedPrice;
            DaysBetweenWatering = daysBetweenWatering;
            GrowthDays = growthDays;
            BloomDays = bloomDays;
            DailyBloomIncome = dailyBloomIncome;
            BloomingImageFilename = imageFilename;
        }
    }

    public class Daisy : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        { 
            new BugsWeight(BugTypes.CommonBugs, 10),
            new BugsWeight(BugTypes.GardenBugs, 0),
            new BugsWeight(BugTypes.ExoticBugs, 0),
            new BugsWeight(BugTypes.TropicalBugs, 0),
        };
        public Daisy() : base("Daisy", 5, 0, 3, 5, 3, "daisy.png") { }
    }

    public class Marigold : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 10),
            new BugsWeight(BugTypes.GardenBugs, 0),
            new BugsWeight(BugTypes.ExoticBugs, 0),
            new BugsWeight(BugTypes.TropicalBugs, 0),
        };
        public Marigold() : base("Marigold", 20, 1, 6, 6, 5, "marigold.png") { }
    }

    public class Sunflower : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 9),
            new BugsWeight(BugTypes.GardenBugs, 1),
            new BugsWeight(BugTypes.ExoticBugs, 0),
            new BugsWeight(BugTypes.TropicalBugs, 0),
        };
        public Sunflower() : base("Sunflower", 50, 2, 6, 10, 15, "sunflower.png") { }
    }

    public class Petunia : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 8),
            new BugsWeight(BugTypes.GardenBugs, 2),
            new BugsWeight(BugTypes.ExoticBugs, 0),
            new BugsWeight(BugTypes.TropicalBugs, 0),
        };
        public Petunia() : base("Petunia", 80, 1, 7, 10, 20, "petunia.png") { }
    }

    public class Rose : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 2),
            new BugsWeight(BugTypes.GardenBugs, 6),
            new BugsWeight(BugTypes.ExoticBugs, 1),
            new BugsWeight(BugTypes.TropicalBugs, 1),
        };
        public Rose() : base("Rose", 300, 0, 10, 15, 70, "rose.png") { }
    }

    public class Tulip : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 2),
            new BugsWeight(BugTypes.GardenBugs, 6),
            new BugsWeight(BugTypes.ExoticBugs, 0),
            new BugsWeight(BugTypes.TropicalBugs, 2),
        };
        public Tulip() : base("Tulip", 500, 1, 8, 12, 90, "tulip.png") { }
    }

    public class Hydrangea : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 0),
            new BugsWeight(BugTypes.GardenBugs, 4),
            new BugsWeight(BugTypes.ExoticBugs, 3),
            new BugsWeight(BugTypes.TropicalBugs, 3),
        };
        public Hydrangea() : base("Hydrangea", 800, 4, 7, 10, 250, "hydrangea.png") { }
    }

    public class Daffodil : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 0),
            new BugsWeight(BugTypes.GardenBugs, 4),
            new BugsWeight(BugTypes.ExoticBugs, 3),
            new BugsWeight(BugTypes.TropicalBugs, 3),
        };
        public Daffodil() : base("Daffodil", 1000, 2, 4, 7, 250, "daffodil.png") { }
    }

    public class ForgetMeNot : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 0),
            new BugsWeight(BugTypes.GardenBugs, 1),
            new BugsWeight(BugTypes.ExoticBugs, 4),
            new BugsWeight(BugTypes.TropicalBugs, 5),
        };
        public ForgetMeNot() : base("Forget-me-not", 1500, 0, 8, 19, 315, "forget_me_not.png") { }
    }

    public class ArumLily : FlowerType
    {
        public override List<BugsWeight> BugWeights { get; } = new List<BugsWeight>
        {
            new BugsWeight(BugTypes.CommonBugs, 0),
            new BugsWeight(BugTypes.GardenBugs, 0),
            new BugsWeight(BugTypes.ExoticBugs, 8),
            new BugsWeight(BugTypes.TropicalBugs, 2),
        };
        public ArumLily() : base("Arum Lily", 5000, 1, 20, 5, 2500, "arum_lily.png") { }
    }

    //The FlowerTypes class contains instances of all flowers that inherit from Flower
    public static class FlowerTypes
    {
        public static FlowerType Daisy = new Daisy();
        public static FlowerType Marigold = new Marigold();
        public static FlowerType Sunflower = new Sunflower();
        public static FlowerType Petunia = new Petunia();
        public static FlowerType Rose = new Rose();
        public static FlowerType Tulip = new Tulip();
        public static FlowerType Hydrangea = new Hydrangea();
        public static FlowerType Daffodil = new Daffodil();
        public static FlowerType ForgetMeNot = new ForgetMeNot();
        public static FlowerType ArumLily = new ArumLily();
    }

    //The PlantedFlower class contains info specific to instances of a flower planted
    public class PlantedFlower
    {
        //The type of which this flower is (e.g. Daisy, Rose), contains info that is the same for every flower of this type
        public FlowerType FlowerType { get; private set; }

        /// <summary>
        /// The name of the flower type of this planted flower.
        /// </summary>
        [Save, NotLoad]
        public string? FlowerTypeName => FlowerType.Name;

        //The number of days this flower has grown
        [Save]
        public int GrowthDays { get; set; }

        //The number of days this flower has already bloomed and produced income
        [Save]
        public int BloomDays { get; set; }

        //The current state of this planted flower
        [Save]
        public FlowerState State { get; set; }

        /*Number of days the flower has not been watered. Starts at 1 after being planted because
        DaysSinceLastWatered is updated after the flower states so that flowers are properly killed.*/
        [Save]
        public int DaysSinceLastWatered { get; set;}

        private PlantedFlower() { }

        public PlantedFlower(FlowerType flower)
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
