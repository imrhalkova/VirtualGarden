using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public abstract class Flower
    {
        public string Name { get; private set; }
        public int SeedPrice { get; private set; }
        public int DaysBetweenWatering { get; private set; }
        public int GrowthDays { get; private set; }
        public int BloomDays { get; private set; }
        public int DailyBloomIncome { get; private set; }
        public abstract List<BugWeight> BugWeights { get; }

        //Maximum number of coins gained from the flower if it is not destroyed prematurely and the player collects coins from it every day
        public int MaximumIncome
        {
            get
            {
                return BloomDays * DailyBloomIncome;
            }
        }

        protected Flower(string name, int seedPrice, int daysBetweenWatering, int growthDays, int bloomDays, int dailyBloomIncome) 
        { 
            Name = name;
            SeedPrice = seedPrice;
            DaysBetweenWatering = daysBetweenWatering;
            GrowthDays = growthDays;
            BloomDays = bloomDays;
            DailyBloomIncome = dailyBloomIncome;
        }
    }

    public class Daisy : Flower
    {
        public override List<BugWeight> BugWeights { get; } = new List<BugWeight>
        { 
            new BugWeight(BugTypes.CommonBugs, 10),
            new BugWeight(BugTypes.GardenBugs, 0),
            new BugWeight(BugTypes.ExoticBugs, 0),
            new BugWeight(BugTypes.TropicalBugs, 0),
        };
        public Daisy() : base("Daisy", 5, 0, 5, 3, 5) { }
    }

    public class Marigold : Flower
    {
        public override List<BugWeight> BugWeights { get; } = new List<BugWeight>
        {
            new BugWeight(BugTypes.CommonBugs, 10),
            new BugWeight(BugTypes.GardenBugs, 0),
            new BugWeight(BugTypes.ExoticBugs, 0),
            new BugWeight(BugTypes.TropicalBugs, 0),
        };
        public Marigold() : base("Marigold", 20, 1, 6, 6, 5) { }
    }

    public class Sunflower : Flower
    {
        public override List<BugWeight> BugWeights { get; } = new List<BugWeight>
        {
            new BugWeight(BugTypes.CommonBugs, 9),
            new BugWeight(BugTypes.GardenBugs, 1),
            new BugWeight(BugTypes.ExoticBugs, 0),
            new BugWeight(BugTypes.TropicalBugs, 0),
        };
        public Sunflower() : base("Sunflower", 50, 2, 8, 10, 13) { }
    }

    public class Petunia : Flower
    {
        public override List<BugWeight> BugWeights { get; } = new List<BugWeight>
        {
            new BugWeight(BugTypes.CommonBugs, 8),
            new BugWeight(BugTypes.GardenBugs, 2),
            new BugWeight(BugTypes.ExoticBugs, 0),
            new BugWeight(BugTypes.TropicalBugs, 0),
        };
        public Petunia() : base("Petunia", 80, 1, 9, 10, 20) { }
    }

    public class Rose : Flower
    {
        public override List<BugWeight> BugWeights { get; } = new List<BugWeight>
        {
            new BugWeight(BugTypes.CommonBugs, 2),
            new BugWeight(BugTypes.GardenBugs, 6),
            new BugWeight(BugTypes.ExoticBugs, 1),
            new BugWeight(BugTypes.TropicalBugs, 1),
        };
        public Rose() : base("Rose", 300, 0, 20, 5, 180) { }
    }

    public class Tulip : Flower
    {
        public override List<BugWeight> BugWeights { get; } = new List<BugWeight>
        {
            new BugWeight(BugTypes.CommonBugs, 2),
            new BugWeight(BugTypes.GardenBugs, 6),
            new BugWeight(BugTypes.ExoticBugs, 0),
            new BugWeight(BugTypes.TropicalBugs, 2),
        };
        public Tulip() : base("Tulip", 500, 1, 12, 8, 130) { }
    }

    public static class FlowerTypes
    {
        static Flower Daisy = new Daisy();
        static Flower Marigold = new Marigold();
        static Flower Sunflower = new Sunflower();
        static Flower Petunia = new Petunia();
        static Flower Rose = new Rose();
        static Flower Peonies = new Tulip();
    }

    public class PlantedFlower
    {
        public Flower FlowerType { get; private set; }
        public int GrowthDays { get; set; }

        public int BloomDays { get; set; }

        public FlowerState State { get; set; }

        public PlantedFlower(Flower flower)
        {
            FlowerType = flower;
            GrowthDays = 0;
            BloomDays = 0;
            State = FlowerState.Growing;
        }
    }

    public enum FlowerState
    {
        Growing,
        Blooming,
        Dead
    }
}
