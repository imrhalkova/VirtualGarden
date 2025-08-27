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
                KillNotWateredFlower();
                KillFlowersNotTreatedFromInfestation();
                switch (Flower.State)
                {
                    case FlowerState.Growing:
                        if (!HasWeed && Bugs is null)
                        {
                            Flower.GrowthDays += 1;
                        }
                        break;
                    case FlowerState.Blooming:
                        if (Flower.BloomDays < Flower.FlowerType.BloomDays && !HasWeed && Bugs is null)
                        {
                            HasCoins = true;
                            Flower.BloomDays += 1;
                        }
                        break;
                    case FlowerState.Dead:
                        break;
                }
            }
        }

        public void UpdateFlowerState()
        {
            if (Flower is not null)
            {
                if (Flower.State == FlowerState.Growing && Flower.GrowthDays == Flower.FlowerType.GrowthDays)
                {
                    Flower.State = FlowerState.Blooming;
                }
                else if (Flower.State == FlowerState.Blooming && Flower.BloomDays == Flower.FlowerType.BloomDays)
                {
                    Flower.State = FlowerState.Dead;
                }
            }
        }
        public void TrySpawningWeed()
        {
            if (Flower is null && _garden.Rand.NextDouble() <= _garden.WeedChance)
            {
                HasWeed = true;
            }

        }
        public void TrySpreadingWeed()
        {
            if (_garden.Rand.NextDouble() <= _garden.WeedSpreadChance)
            {
                HasWeed = true;
            }
        }

        public void IncrementDaysSinceLastWatered()
        {
            DaysSinceLastWatered += 1;
        }

        public void TrySpawningBugInfestation()
        {
            if (Flower is not null && Flower.State != FlowerState.Dead && this.Bugs is null && _garden.Rand.NextDouble() <= _garden.BugsChance)
            {
                SpawnABugInfestation();
            }
        }

        public void SpawnABugInfestation()
        {
            //TO DO
        }

        public void KillNotWateredFlower()
        {
            if (DaysSinceLastWatered > Flower.FlowerType.DaysBetweenWatering)
            {
                Flower.State = FlowerState.Dead;
            }
        }

        public void KillFlowersNotTreatedFromInfestation()
        {
            if (Bugs is not null && Bugs.DaysUntilFlowerDies == 0)
            {
                Flower.State = FlowerState.Dead;
                Bugs = null;
            }
        }

        public void UpdateBugInfestation()
        {
            if (Bugs is not null)
            {
                Bugs.DaysUntilFlowerDies -= 1;
            }
        }
    }
}
