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
        public BugInfestation? Bugs { get; set; }
        public bool HasCoins { get; set; } = false;

        public Tile(Garden garden, int row, int column)
        {
            _garden = garden;
            Row = row;
            Column = column;
        }

        public string PrintCoordinates()
        {
            return $"[{Row}, {Column}]";
        }

        public void PlantFlower(Flower flower)
        {
            if (Flower is not null)
            {
                throw new FlowerAlreadyPresentException($"Cannot plant flower on tile {PrintCoordinates()}. There is already a flower present on this tile.");
            }
            Flower = new PlantedFlower(flower);
        }

        public void RemoveFlower()
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot remove flower from tile {PrintCoordinates()}. There is no flower on this tile.");
            }
            Flower = null;
        }

        public void WaterTile()
        {
            if (Flower is not null)
            {
                Flower.DaysSinceLastWatered = 0;
            }
        }

        public void RemoveWeed()
        {
            if (!HasWeed)
            {
                throw new WeedNotPresentException($"Cannot remove weed on tile {PrintCoordinates()}. There is no weed on this tile.");
            }
            HasWeed = false;
        }

        public void CollectCoins()
        {
            if (!HasCoins)
            {
                throw new CoinsNotPresentException($"Cannot collect coins on tile {PrintCoordinates()}. There are no coins on this tile.");
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

        public void UpdateWateredStateOfFlower()
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot update water state of flower on tile {PrintCoordinates()}. There is no flower on this tile.");
            }
            Flower.DaysSinceLastWatered += 1;
        }

        public bool CanSpawnBugs()
        {
            return Flower is not null && Flower.State != FlowerState.Dead && this.Bugs is null;
        }

        public void SpawnBugInfestation(BugInfestation bugs)
        {
            if (Bugs is not null)
            {
                throw new BugsAlreadyPresent($"Cannot spawn new bugs on tile {PrintCoordinates()}. This tile already has bugs.");
            }
            Bugs = bugs;
        }

        public void KillNotWateredFlower()
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot set the state of flower on tile {PrintCoordinates()} to dead. There is no flower on this tile.");
            }
            if (Flower.DaysSinceLastWatered > Flower.FlowerType.DaysBetweenWatering)
            {
                Flower.State = FlowerState.Dead;
            }
        }

        public void KillFlowersNotTreatedFromInfestation()
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot set the state of flower on tile {PrintCoordinates()} to dead. There is no flower on this tile.");
            }
            if (Bugs is not null && Bugs.DaysUntilFlowerDies == 0)
            {
                Flower.State = FlowerState.Dead;
                Bugs = null;
            }
        }

        public void UpdateBugInfestation()
        {
            if (Bugs is null)
            {
                throw new BugsNotPresent($"Cannot update bug infestation on tile {PrintCoordinates()}. This tile has no bugs.");
            }
            Bugs.Update();
        }
    }
}
