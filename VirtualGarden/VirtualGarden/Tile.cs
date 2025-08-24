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
        public int Row { get; private set; }
        public int Column { get; private set; }
        public PlantedFlower? Flower {  get; private set; }
        public bool HasWeed {  get; set; } = false;
        public Bugs? Bugs { get; set; }
        public bool HasCoins { get; set; } = false;
        public int DaysSinceLastWatered { get; set; }

        public Tile(Garden garden, int row, int column)
        {
            _garden = garden;
            DaysSinceLastWatered = -1;
            Row = row;
            Column = column;
        }

        public void PlantFlower(Flower flower)
        {
            if (Flower is not null)
            {
                throw new FlowerAlreadyPresentException();
            }
            Flower = new PlantedFlower(flower);
        }

        public void RemoveFlower()
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException();
            }
            Flower = null;
        }

        public void WaterTile()
        {
            if (Flower is not null)
            this.DaysSinceLastWatered = 0;
        }

        public void RemoveWeed()
        {
            if (!HasWeed)
            {
                throw new WeedNotPresentException();
            }
            HasWeed = false;
        }

        public void CollectCoins()
        {
            if (!HasCoins)
            {
                throw new CoinsNotPresentException();
            }
            _garden.Player.Money += Flower.FlowerType.DailyBloomIncome;
            HasCoins = false;
        }

        public void UpdateFlower()
        {
            if (Flower is not null)
            {
                switch (Flower.State)
                {
                    case FlowerState.Growing:
                        if (this.DaysSinceLastWatered <= Flower.FlowerType.DaysBetweenWatering && !HasWeed)
                        {
                            Flower.GrowthDays += 1;
                        }
                        if (Flower.GrowthDays == Flower.FlowerType.GrowthDays)
                        {
                            Flower.State = FlowerState.Blooming;
                        }
                        break;
                    case FlowerState.Blooming:
                        if (Flower.BloomDays < Flower.FlowerType.BloomDays && !HasWeed)
                        {
                            HasCoins = true;
                            Flower.BloomDays += 1;
                        }
                        if (Flower.BloomDays == Flower.FlowerType.BloomDays)
                        {
                            Flower.State = FlowerState.Dead;
                        }
                        break;
                    case FlowerState.Dead:
                        break;
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
