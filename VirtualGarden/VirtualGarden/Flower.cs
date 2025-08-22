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
        public Daisy() : base("Daisy", 5, 0, 5, 3, 5) { }
    }

    public static class FlowerTypes
    {
        static Flower Daisy = new Daisy();
    }

    public class PlantedFlower
    {
        public Flower FlowerType { get; private set; }
        public int GrowthDays { get; set; }

        public int BloomDays { get; set; }

        public int DaysSinceLastWatered { get; set; }

        public FlowerState State { get; set; }

        public PlantedFlower(Flower flower)
        {
            FlowerType = flower;
            GrowthDays = 0;
            BloomDays = 0;
            DaysSinceLastWatered = -1;
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
