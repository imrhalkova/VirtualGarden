
# User documentation

## Introduction

The game is a virtual flower garden. The goal of the game is to get all types of flowers to bloom in the garden. The player must water the flowers, protect them from bugs and weed and earn money to buy new flower seeds.

## Game controls

### The message box

Whenever a handleded exception occurs during the game the error message is displayed for the user at the bottom of the screen in a white message box. The player can make the message box invisible again by clicking the 'clear' button on the right of the message box.
<img width="563" height="38" alt="image" src="https://github.com/user-attachments/assets/64f55b98-f77e-4eb6-a978-4326d0f63c76" />

### Main menu

Right after opening the game the user sees the main menu. It consists of the following 3 buttons:
<img width="709" height="628" alt="image" src="https://github.com/user-attachments/assets/6252f67f-ef8c-4b0b-afa0-ad37c6adf148" />

The button 'New game' creates a completely new game. If there was a previously saved game, it will be overwritten without asking the user for confirmation.
The button 'Load game' loads a previously saved game. If there is no saved game the game will write an error message into the message box.
The button 'Exit' immediately ends the application.


### The garden

After creating a new game or loading a saved one the user sees a screen with the flower garden. A possible look of the garden after playing a bit:
<img width="2551" height="1315" alt="Screenshot 2025-09-26 224631" src="https://github.com/user-attachments/assets/0533ab95-6488-468d-a419-ea3aebc3f790" />

On the right top side there are two buttons. The button 'menu' will bring the player into the game menu where the player can save or exit the game.
The 'flower catalogue' button will bring it user to the flower catalogue which contains information about flower types in the game which the user needs to take care of the flowers.

Bellow that there is a blue box containing the amount of money the player has and the number of the current day.

Bellow the blue box there is a pink box that contains the counts of flowers that have bloomed for each type of flower.

In the top centre of the screen there is a yellow 'New day' button which is used to transition to a new day.

Below the new day button there are the tiles of the garden. The screen with the info of a specific tile is accessed by clicking said tile.

the left side of the screen is used for displaying the info about the current active event in a light blue box. The event info includes name, description, number of days left plus additional info for specific events. If no event is currently active the blue box is not visible at all.

### A tile

By clicking on a specific tile the user is shown the following screen with tile information:
<img width="2555" height="1363" alt="image" src="https://github.com/user-attachments/assets/a559699e-c99e-466c-9cce-baf6e1c224b5" />

At the left top of the screen there is the 'Back to garden' button which will return the user to the garden screen.

At the top right of the screen, the amount of money the user has is shown.

In the middle of the screen there are two columns with information. The left column displays information about the selected tile: the row and column of the tile (starting at 0), if there is a flower planted on this tile and which one, if there is weed on this tile, if there are bugs on this tile and if they are the, the amount of money needed to get rid of them and the number of days until they kill the flower on this tile, and lastly the number of money/coins and player has.

The right column displays information about the flower planted on this tile. If there is no flower planted on it, the column is not visible at all. The information displayed is the number of days this flower has already grown, the number of days this flower has bloomed, the number of days since this flower has been watered and the state of the flower (growing, blooming or dead).

At the bottom of the screen there are six buttons used to interact with the tile and the flower on it. The 'Water' button waters and flower planted on this tile. The 'Remove weed' button removes weed from this tile if there is any. The 'Remove bugs' button removes bugs from this tile and subtracts the amount of money needed for it from the player. There must be bugs on the tile and the player must have sufficient funds for the bug spray. The 'Collect coins' button collects coins from this tile that were produced by a blooming flower. The coins are added to the player's funds. The 'Plant flower' button will take the user to the flower catalogue from where they can choose a flower to plant on this tile provided there is currently no flower planted on this tile. The 'Remove flower' button will remove the flower planted on this tile without asking the user for confirmation.

### The flower catalogue

The user can get into the flower catalogue by either clicking the 'Flower catalogue' button in the garden screen or the 'Plant flower' button in the tile information screen. This screen displays information for each type of flower.
<img width="2550" height="1306" alt="image" src="https://github.com/user-attachments/assets/a9a5a93d-edf6-41d6-8dbc-4fac9ad968f5" />

At the left top of the screen there is the 'Back to garden' button, which will return the user to the garden screen. At the right top of the screen, the amount of money the player has is displayed.

The rest of the screen is filled with information for each flower type. The information includes the name of the flower, the number of days it needs to grow and start blooming, the number of days it blooms, the maximum number of days the flower can survive without being watered, the amount of money this flower produces during one day of blooming, the maximum amount of money the player can collect from this flower and the price of the flower's seeds.

If the player entered the screen from the tile screen there is also a 'Buy seeds and plant' button visible under each flower information. Clicking the button will plant the said flower on the chosen tile and return the player to the garden screen. If the player entered the flower catalogue from the garden screen this button is not visible.

### The game menu

The game menu, which is accessed by clicking the 'menu' button on the garden screen, consists of 3 buttons:

<img width="564" height="567" alt="image" src="https://github.com/user-attachments/assets/d6c138be-3f00-4bf6-bb0e-9f2d0ba5be74" />

The 'Resume' button will return the user to the garden screen.

The 'Save game' button will save the current state of the game and overwrite the previous save of the game without asking the user.

The 'Exit' button will exit the game without saving it and without asking the user's permission.

## Game mechanics

### Money

Every new day the player is awarded 20 coins. The player can also collect money from blooming flowers.

### Flowers

To plant a flower the player has to pay for flower seeds. The flower goes through 3 states. First the flower is in growing state which lasts the number of growth days for said flower. After that the flower enters blooming state during which it produces money that can be collected every day. The flower remains blooming for the flower's number of bloom days. After the flower fades or if its killed due to bugs or not being watered often enough, the flower is in dead state.

Maximum days between watering signifies the maximum number of days this flower will survive not being watered.

### Weed

Weed spawns on tiles without flowers. With new day it can spread to adjacent tiles, including the ones with flowers. When weed is on the same tile as a planted flower, if the flower is in growing state than the growth is stopped by the weed, if the flower is in blooming state, the blooming is paused and the flower does not produce money.

### Bugs

Bugs spawn on tiles with flowers. They kill the flower after a few days. To get rid of the bugs the player has to pay for a bug spray. There are different kind of bugs with different prices of bug sprays and different number of days until the flower is killed. If the flower is in the growing state, growth is paused as long as the bugs are there. If the flower is in blooming state. blooming is paused as long as the bugs are there.

### Events

There is a chance with each new day that an event will be triggered. If it is triggered, one of the possible events will be chosen.
