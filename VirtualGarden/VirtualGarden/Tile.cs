#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    /// <summary>
    /// Represents one tile of the garden grid in which the user can plant a flower and take care of it.
    /// </summary>
    public class Tile
    {
        //The number of the row this tile is in in the garden grid
        public int Row { get; private set; }

        //The number of the column this tile is in in the garden grid
        public int Column { get; private set; }
        public PlantedFlower? Flower {  get; private set; }

        //An indicator whether there is weed on this tile
        public bool HasWeed {  get; set; } = false;
        public BugInfestation? Bugs { get; set; }

        //The number of coins that can be collected from this tile during the current day
        public int Coins { get; set; }

        public Tile(int row, int column)
        {
            Row = row;
            Column = column;
            Coins = 0;
        }

        public string PrintCoordinates()
        {
            return $"[{Row}, {Column}]";
        }

        public void PlantFlower(FlowerType flower)
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
            Bugs = null;
        }

        public void WaterFlower()
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

        public void RemoveBugs()
        {
            if (Bugs is null)
            {
                throw new BugsNotPresentException($"Cannot remove bugs from tile {PrintCoordinates()}. There are no bugs on this tile.");
            }
            Bugs = null;
        }

        public void CollectCoins()
        {
            if (Coins == 0)
            {
                throw new CoinsNotPresentException($"Cannot collect coins on tile {PrintCoordinates()}. There are no coins on this tile.");
            }
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Coins present on a tile without a flower.");
            }
            Coins = 0;
        }

        public void UpdateFlower(Player player)
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot update flower on tile [{Row}, {Column}]. There is no flower on this tile");
            }
            SetDeadFlowerStatesToDead();
            Coins = 0;
            if (Flower.State == FlowerState.Growing && !HasWeed && Bugs is null)
            {
                Flower.GrowthDays += 1;
                if (Flower.GrowthDays == Flower.FlowerType.GrowthDays)
                {
                    Flower.State = FlowerState.Blooming;
                    player.FlowerTypesBloomCount[Flower.FlowerType] += 1;
                }
            }
            if (Flower.State == FlowerState.Blooming && Flower.BloomDays < Flower.FlowerType.BloomDays && !HasWeed && Bugs is null)
            {
                Coins = Flower.FlowerType.DailyBloomIncome;
                Flower.BloomDays += 1;
            }
        }

        public void SetDeadFlowerStatesToDead()
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot set the state of flower on tile {PrintCoordinates()} to dead. There is no flower on this tile.");
            }
            KillFadedFlower();
            KillNotWateredFlower();
            KillFlowersNotTreatedFromInfestation();
        }

        public void SpawnWeed()
        {
            HasWeed = true;
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
                throw new BugsAlreadyPresentException($"Cannot spawn new bugs on tile {PrintCoordinates()}. This tile already has bugs.");
            }
            Bugs = bugs;
        }

        public void KillFlower()
        {
            if (Flower is null)
            {
                throw new FlowerNotPresentException($"Cannot kill flower on tile {PrintCoordinates()}. There is no flower on this tile.");
            }
            Flower.State = FlowerState.Dead;
            Bugs = null;
        }

        private void KillFadedFlower()
        {
            if (Flower.BloomDays == Flower.FlowerType.BloomDays)
            {
                KillFlower();
            }
        }

        private void KillNotWateredFlower()
        {
            if (Flower.DaysSinceLastWatered > Flower.FlowerType.DaysBetweenWatering)
            {
                KillFlower();
            }
        }

        private void KillFlowersNotTreatedFromInfestation()
        {
            if (Bugs is not null && Bugs.DaysUntilFlowerDies == 0)
            {
                KillFlower();
                Bugs = null;
            }
        }

        public void UpdateBugInfestation()
        {
            if (Bugs is null)
            {
                throw new BugsNotPresentException($"Cannot update bug infestation on tile {PrintCoordinates()}. This tile has no bugs.");
            }
            Bugs.Update();
        }

        public bool IsEqual(Tile other)
        {
            if (other is null)
            {
                return false;
            }
            return Row == other.Row &&
                Column == other.Column &&
                PlantedFlower.IsEqual(Flower, other.Flower) &&
                HasWeed == other.HasWeed &&
                BugInfestation.IsEqual(Bugs, other.Bugs) &&
                Coins == other.Coins;
        }
    }
}
