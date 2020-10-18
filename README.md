# IFN563-BoardGameLib
 A C# Library for creating BoardGames for Queensland University of Technology's IFN563 (Object Oriented Design). The Library (BoardGameLibs.cs) is designed to be reusable between a large number of two dimensional, two player board games.

All features requested in the design documentation have been implemented. The features outlined by the design brief were as follows:

- A reusable framework that accommodates Gomoku, Othello and Mills, amongst other classic board games
- The output to a 2D Unicode display
- Human vs Human play
- Human vs Computer play
- Computer vs Computer play
- Both Human and Computer Player moves are checked for validity as they are entered
- User selectable Computer Player difficulty, including a Computer Player that places a piece in a randomly selected location and a Computer Player that actively attempts to block the creation of lines by the other player
- Games can be saved and loaded from any point
- Moves can be undone and redone. Loaded games offer the same functionality; a user can play a game almost to completion, save the game, undo everything and then load the saved game
- The game provides for inline help, including information about the game, valid moves, how to use the interface and warning or error messages if the user attempts to make an illegal move

All the above features have been implemented for the board game Gomoku; however, advanced Gomoku rules (such as Swap2 or the disallowance of overlines) have not been implemented.
