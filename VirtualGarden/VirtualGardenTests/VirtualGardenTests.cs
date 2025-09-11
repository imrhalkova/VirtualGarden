using System;
using VirtualGarden;

namespace VirtualGardenTests
{
    public class VirtualGardenNoWeedBugsAndEventsTests
    {
        [Fact]
        public void CreateGardenGridTest()
        {
            Garden garden = new Garden(2, 3, new Player(), 0, 0, 0, false);

            Tile[,] correctGrid = new Tile[2, 3];
            for (int i = 0; i < correctGrid.GetLength(0); i++)
            {
                for (int j = 0; j < correctGrid.GetLength(1); j++)
                {
                    correctGrid[i, j] = new Tile(i, j);
                }
            }
            Assert.True(TestHelpers.AreGridsEqual(correctGrid, garden.Grid));
        }

        [Fact]
        public void DailyIncomeTest()
        {
            int dailyIncome = 20;
            int startingMoney = 0;

            Garden garden = new Garden(1, 1, new Player(startingMoney, dailyIncome), 0, 0, dailyIncome, false);

            garden.NewDay();

            Assert.Equal(startingMoney + dailyIncome, garden.Player.Coins);
        }

        [Fact]
        public void PlantFlowerTest()
        {
            Garden garden = new Garden(1, 1, new Player(), 0, 0, 0, false);

            garden.PlantFlower(0, 0, FlowerTypes.Daisy);

            Garden correctGarden = new Garden(1, 1, new Player());

            correctGarden.GetTile(0, 0).PlantFlower(FlowerTypes.Daisy);

            Assert.True(TestHelpers.AreGridsEqual(garden.Grid, correctGarden.Grid));
        }

        [Fact]
        public void FlowerStateFromGrowingToBloomingTest()
        {
            Garden garden = new Garden(2, 2, new Player(), 0, 0, 20, false);

            garden.PlantFlower(0, 0, new TestFlower1());

            Assert.Equal(FlowerState.Growing, garden.GetTile(0, 0).Flower.State);
            garden.WaterFlower(garden.GetTile(0, 0));
            garden.NewDay();

            Assert.Equal(FlowerState.Blooming, garden.GetTile(0, 0).Flower.State);
        }

        [Fact]
        public void FlowerDiesFromNotBeingWateredTest()
        {
            Garden garden = new Garden(2, 2, new Player(), 0, 0, 20, false);

            garden.PlantFlower(0, 0, new TestFlower1());

            garden.NewDay();

            Assert.Equal(FlowerState.Dead, garden.GetTile(0, 0).Flower.State);
        }
    }

    public class VirtualGardenWeedTests
    {
        [Fact]
        public void FlowerNotGrowingWhenOnATileWithWeed()
        {
            Garden garden = new Garden(1, 1, new Player(), 0, 0, 0, false);

            garden.GetTile(0, 0).SpawnWeed();
            garden.PlantFlower(0, 0, new TestFlower1());
            garden.WaterTile(0, 0);
            garden.NewDay();

            Assert.Equal(FlowerState.Growing, garden.GetTile(0, 0).Flower.State);
        }

        [Fact]
        public void FlowerNotBloomingAndNotProducingMoneyOnATileWithWeed()
        {
            Garden garden = new Garden(1, 1, new Player(), 0, 0, 0, false);
            
            garden.PlantFlower(0, 0, FlowerTypes.Daisy);
            for (int i = 0; i < FlowerTypes.Daisy.GrowthDays; i++)
            {
                garden.WaterTile(0, 0);
                garden.NewDay();
            }
            int previousBloomDays = garden.GetTile(0, 0).Flower.BloomDays;
            garden.GetTile(0, 0).SpawnWeed();
            garden.WaterTile(0, 0);
            garden.NewDay();

            Assert.Equal(previousBloomDays, garden.GetTile(0, 0).Flower.BloomDays);
            Assert.Equal(0, garden.GetTile(0, 0).Coins);
        }

        [Fact]
        public void SpawnWeedNoBugsAndEventsTest()
        {
            double weedSpawnChance = 0.2;
            List<double> doublesToBeGenerated = new List<double>() { weedSpawnChance - 0.1 };
            IRandomNumberGenerator random = new FakeRandomGenerator(doublesToBeGenerated);
            Garden garden = new Garden(1, 1, new Player(), weedSpawnChance, 0, 0, false, random);
            garden.NewDay();

            Assert.Equal(true, garden.GetTile(0, 0).HasWeed);
        }

        [Fact]
        public void NoSpawnWeedNoBugsAndEventsTest()
        {
            double weedSpawnChance = 0.2;
            List<double> doublesToBeGenerated = new List<double>() { weedSpawnChance + 0.1 };
            IRandomNumberGenerator random = new FakeRandomGenerator(doublesToBeGenerated);
            Garden garden = new Garden(1, 1, new Player(), weedSpawnChance, 0, 0, false, random);
            garden.NewDay();

            Assert.Equal(false, garden.GetTile(0, 0).HasWeed);
        }

        [Fact]
        public void NoSpawnWeedOnATileWithFlowerTest()
        {
            double weedSpawnChance = 0.2;
            List<double> doublesToBeGenerated = new List<double>() { weedSpawnChance - 0.1 };
            IRandomNumberGenerator random = new FakeRandomGenerator(doublesToBeGenerated);
            Garden garden = new Garden(1, 1, new Player(), weedSpawnChance, 0, 0, false, random);
            garden.GetTile(0, 0).PlantFlower(FlowerTypes.Petunia);
            garden.NewDay();

            Assert.Equal(false, garden.GetTile(0, 0).HasWeed);
        }

        [Fact]
        public void WeedSpreadingToAdjacentTileTest()
        {
            double weedSpawnChance = 0;
            List<double> doublesToBeGenerated = new List<double>() { 0.3 };
            IRandomNumberGenerator random = new FakeRandomGenerator(doublesToBeGenerated);
            Garden garden = new Garden(1, 2, new Player(), weedSpawnChance, 0, 0, false, random);
            garden.GetTile(0, 0).SpawnWeed();
            garden.NewDay();

            Assert.Equal(true, garden.GetTile(0, 1).HasWeed);
        }

        [Fact]
        public void WeedNotSpreadingToNonAdjacentTileTest()
        {
            double weedSpawnChance = 0;
            List<double> doublesToBeGenerated = new List<double>() { 0.3 };
            IRandomNumberGenerator random = new FakeRandomGenerator(doublesToBeGenerated);
            Garden garden = new Garden(1, 3, new Player(), weedSpawnChance, 0, 0, false, random);
            garden.GetTile(0, 0).SpawnWeed();
            garden.NewDay();

            Assert.Equal(false, garden.GetTile(0, 2).HasWeed);
        }

    }
}