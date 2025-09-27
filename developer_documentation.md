
# Developer documentation

## Introduction

The project is a virtual garden game written in C# with user interface made in Winforms.

## Project structure

- `VirtualGarden`
  - `pictures\`: contains all images used in the UI
  - `BugType.cs`: contains all classes dealing with bugs
  - `Event.cs`: contains the abstract `Event` class and the concrete events inheriting from it
  - `Exception.cs`: contains the game's own exceptions
  - `FileHandler.cs`: contains the `FileHandler` class, `SaveAttribute` and `NotLoadAttribute` used for saving and loading the garden and for accessing images used in UI
  - `FlowerType.cs`: contains all classes dealing with flowers
  - `Form1.cs`: contains the code of the graphic UI
  - `Garden.cs`: contains the `Garden` class
  - `Player.cs`: contains the `Player` class
  - `Program.cs`: contains `Main()`
  - `Random.cs`: contains the `IRandomNumberGenerator` interface used for generating random numbers in the game, `IWeightedItem<T>` interface and the `WeightedRandom` class used for chosing weighted items randomly
  - `Tile.cs`: contains the `Tile` class
- `VirtualGardenTests`
  - `TestHelpers.cs`: contains classes and fuctions used for testing
  - `VirtualGardenTests.cs`: contains unit tests for the Virtual Garden 

## Important classes

### `Garden`

The `Garden` class contains the whole game including the grid of tiles, the player and the active event.

### `Tile`

The `Tile` class represents one tile of the garden grid. It includes informations about the tile's location in the grid,the flower planted on it, the bugs, weed and coins on it.

### `Player`

The `Player` class represents the player. It includes the members `Money`, `NewDayIncome`, `NumberOfDay` and `FlowerTypesBloomCount` to keep track of the number of flowers that have bloomed of each flower type.

### `Event`

The `Event` is an abstract class from which all individual events inherit. It has the abstract members `Name`, `Description`, `Update()` and the non-abstract `DaysLeft`.
The `Update()` function is used to update the event for a new day.

### `FlowerType`

The `FlowerType` class is the abstract class from which all types of flowers (Daisy, Rose, Tulip, ...) inherit.

### `PlantedFlower`

The `PlantedFlower` class contains information associated with a concrete planted flower in the garden.

### `BugType`

The `BugType` class is the abstract class from which all types of bugs inherit.

### `BugInfestation`

The `BugInfestation` class contains information about a specific bug infestation on a flower.

## Transition between days

The `Garden.NewDay()` function is used to transition between days in the game. It does the following:
- updates ongoing events and ends those that have run out
- updates previously spawned infestations in the garden
- updates the growth and bloom days of flowers and their state
- updates the `DaysSinceLastWatered` for flowers
- updates weed in the garden (both tries spreading existing weed and spawning new weed)
- tries spawning new bug infestations
- updates the player
- if there is no active event, it tries starting a new event
- if the active event is raining, it waters all flowers

## Saving/Loading

The saving a loading of the game is done by the `FileHandler` class. Its `SaveGardenToFile(Garden garden)` function saves the passed instance of `Garden` into a `.JSON` file so that it can be loaded later. The fields and properties needed for reconstructing the `Garden` are first, with the use of reflection, converted to a
`Dictionary<string, object?>` dictionary which is then saved into a JSON file `save.JSON` inside the `saves` folder that is created in the same directory as the application's `.exe` file.

The game is loaded back with the `LoadGardenFromFile()` which returns an instance of the garden that is saved in the `save.JSON` file using reflection.

### The structure of the JSON save file

- `Grid`: the garden's grid of tiles.
  - `rows`: the number of rows of the grid
  - `columns`: the number of columns of the grid
  - Each tile indexed `(row,column)`
    - `Row`: the number of row in which the tile is located
    - `Column`: the number of column in which the tile is located
    - `Flower`: the info of the instance of `PlantedFlower` that is planted on this tile or `null`
      - `FlowerTypeName` the name of the flower type
      - `GrowthDays`: the number of days this flower has grown
      - `BloomDays`: the number of days this flower has bloomed
      - `State`: the state of the flower (`Growing`, `Blooming` or `Dead`)
      - `DaysSinceLastWatered`: the number of days since the flower has been last watered
    - `HasWeed`: `true` if there is weed on this tile, otherwise `false`
    - `Bugs`: the info of the instance of `BugInfestation` on this tile or `null`
      - `BugsTypeName`: the name of the bugs type
      - `DaysUntilFlowerDies`: the number of days until the flower dies if the bugs are not removed
    - `Coins`: the number of coins on this tile
- `Player`
  - `Money`: the amount of money the player has
  - `NewDayIncome`: the amount of money the player gets every new day
  - `NumberOfDay`: the number of day
  - `FlowerTypesBloomCount`: counts of flowers that have bloomed for each flower type
- `Event`: the info of the current active event or `null`
  - `DaysLeft`
  - Info specific to the event
- `EventTypeName`: the full type name of the active event
- `EventChance`: the chance of a new event starting on a new day with no active event
- `_normalWeedChance`: the chance of weed spawning on one empty tile with no event modifying the weed chances
- `_normalBugsChance`: the chance of bugs spawning on one tile with flower with no event modifying the bugs chances
- `_eventsEnabled`: `true` if events are enabled, otherwise `false`
