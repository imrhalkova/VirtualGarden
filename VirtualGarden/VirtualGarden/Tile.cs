#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public class Tile
    {
        private Garden _garden;
        public PlantedFlower? Flower {  get; private set; }
        public bool HasWeed {  get; set; } = false;
        public bool HasBugs { get; set; } = false;

        public bool HasCoins { get; set; } = false;

        public Tile(Garden garden)
        {
            _garden = garden;
        }

        public void PlantFlower(Flower flower)
        {
            Flower = new PlantedFlower(flower);
        }

        public void RemoveFlower()
        {
            Flower = null;
        }

        public void WaterFlower()
        {
            if (Flower is not null)
            Flower.DaysSinceLastWatered = 0;
        }

        public void RemoveWeed()
        {
            HasWeed = false;
        }

        public void CollectCoins()
        {
            if (HasCoins)
            {
                _garden.Player.Money += Flower.FlowerType.DailyBloomIncome;
                HasCoins = false;
            }
        }

        public void UpdateFlowers()
        {
            if (Flower is not null)
            {
                if (Flower.State == FlowerState.Growing)
                {
                    if (Flower.DaysSinceLastWatered <= Flower.FlowerType.DaysBetweenWatering && !HasWeed)
                    {
                        Flower.GrowthDays += 1;
                    }
                    if (Flower.GrowthDays == Flower.FlowerType.GrowthDays)
                    {
                        Flower.State = FlowerState.Blooming;
                    }
                }
                if (Flower.State == FlowerState.Blooming)
                {
                    if (Flower.BloomDays < Flower.FlowerType.BloomDays && !HasWeed)
                    {
                        HasCoins = true;
                        Flower.BloomDays += 1;
                    }
                }
                if (Flower.BloomDays == Flower.FlowerType.BloomDays)
                {
                    Flower.State = FlowerState.Dead;
                }
            }
        }
        public void TrySpawningWeed()
        {
            if (Flower is null && _garden.rand.NextDouble() <= _garden.WeedChance)
            {
                HasWeed = true;
            }

        }
        public void TrySpreadingWeed()
        {
            if (_garden.rand.NextDouble() <= _garden.WeedSpreadChance)
            {
                HasWeed = true;
            }
        }
    }
}
