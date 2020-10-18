using System;
using System.Collections.Generic;
using BoardGameLib;

public class GomokuPiece : Piece
{
    // Classes must have their normal constructor, a constructor
    // containing just their Type to use to copy objects, and
    // an Empty constructor for the XML Serialiser used to save
    // and load the objects
    public GomokuPiece(string description, char representation) :
        base(description, representation)
    { }

    public GomokuPiece(GomokuPiece piece) : base(piece) { }

    public GomokuPiece() : base() { }

    // Any valid moves for a piece should be listed here. All moves
    // are considered invalid by default. In Gomoku, a piece can
    // only be placed; all other moves throw an InvalidMoveException
    public override void Place(Square square)
    {
        square.piece = this;
    }
}


public class WhitePiece : GomokuPiece
{
    public WhitePiece() :
        base(description: "White", representation: '\x25CB') { }

    public WhitePiece(WhitePiece piece) : base(piece) { }
}


public class BlackPiece : GomokuPiece
{
    public BlackPiece() :
        base(description: "Black", representation: '\x25CF') { }

    public BlackPiece(BlackPiece piece) : base(piece) { }

}


public class GomokuBoard : Board
{
    public GomokuBoard(GomokuBoard board) : base(board) { }

    public GomokuBoard() : base(xDimensions: 15, yDimensions: 15) { }
}


public class Gomoku : Game
{
    // Here game-specific text and logic is set out. In the constructor
    // the help, rules and player creation text is set.
    public Gomoku() : base(numberOfPlayers: 2)
    {
        board = new GomokuBoard();

        preamble =
            "Welcome to Gomoku, a game implimented to demonstrate the IFN563\n" +
            "Board Game library created in Object Oriented C#. Type \'rules\' at any\n" +
            "time to read the game rules, \'help\' for game help or type \'quit\'\n" +
            "at any time to exit.\n";

        loadSavedGameDialogue =
            "Would you like to create a new game or load saved game of Gomoku?\n" +
            "type \'new\' to start a new game or \'load\' to load a saved game, or\n" +
            "type \'quit\' to exit: ";

        humanReadableRules =
            "You can place a piece on the board at any square that doesn't already\n" +
            "have a piece in it. In Gomoku, one can only place a piece. It cannot\n" +
            "be moved once placed. The first player to make a row of 5 wins!\n";

        humanReadableValidMoves =
             "You can place a piece by giving the co-ordinates of the piece, such\n" +
             "as \'A 1\', \'1 a\' etc. You can save the current game with the \'save\'\n" +
             "keyword, or use \'load\' to load a saved game at any time. Moves can be\n" +
             "Undone or Redone using the \'undo\' and \'redo\' keywords at any turn.\n";

        createPlayersPreamble =
            "This is a game for two players, either of which can be a Human or\n" +
            "Computer player. Player 1 will play as Black while player 2 will\n" +
            "be White. Please set up the types of players competing in\n" +
            "this game. Type \'h\' or \'human\' for a human player and \'c\' or\n" +
            "\'computer\' for a computer player:\n";
    }

    // Perform any game-specific initiation logic here
    public override bool init()
    {
        bool createGame = base.init();

        if (createGame && players.Count == 0)
        {
            initPlayers();
        }
        return createGame;
    }

    // Perform game specific player creation here
    public override void initPlayers()
    {
        Type[] computerPlayerDifficulties = { typeof(easyGomokuComputer),
                                              typeof(hardGomokuComputer) };

        createPlayers(computerPlayers: computerPlayerDifficulties);

        Agent playerOne = players[0];
        Agent playerTwo = players[1];

        playerOne.pieces.Add(new BlackPiece());
        playerTwo.pieces.Add(new WhitePiece());
    }

    public bool checkForLine()
    {
        return true;
    }

    // Outline game-specific legal moves and state-checking here
    public override void move(int[] sourceCoordinates,
                              int[] destinationCoordinates,
                              Piece piece,
                              Agent player)
    {

        // Declare game-specific variables at the top of the move function
        int maxMoves = 15 * 15;
        string _onePiecePerSquareError = "Placing a piece on top of another is" +
                                         " not valid a valid move in Gomoku";

        int lineLength = 5;

        // Next, include any helper functions required to facilitate a move
        // in this game, or used to check game states
        bool checkForLine(Board board, int xCoordinate, int yCoordinate,
                          string direction = null, int depth = 1, int maxDepth = 5)
        {
            int xDimensions = board.xDimensions;
            int yDimensions = board.yDimensions;

            int i = xCoordinate;
            int j = yCoordinate;

            Square currentSquare = board.squares[(i * yDimensions) + j];
            Piece currentPiece = board.squares[(i * yDimensions) + j].piece;

            if (depth == maxDepth)
                return true;

            if (currentPiece != null)
            {
                switch (direction)
                {
                    case "N":
                        if (currentSquare.neighbours[0] &&
                            (currentPiece.Equals(board.squares[(i * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate, yCoordinate - 1, "N", depth + 1, maxDepth);
                        return false;
                    case "NE":
                        if (currentSquare.neighbours[0] && currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate - 1, "NE", depth + 1, maxDepth);
                        return false;
                    case "E":
                        if (currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + j].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate, "E", depth + 1, maxDepth);
                        return false;
                    case "SE":
                        if (currentSquare.neighbours[1] && currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate + 1, "SE", depth + 1, maxDepth);
                        return false;
                    case "S":
                        if (currentSquare.neighbours[1] &&
                            (currentPiece.Equals(board.squares[(i * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate, yCoordinate + 1, "S", depth + 1, maxDepth);
                        return false;
                    case "SW":
                        if (currentSquare.neighbours[1] && currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate + 1, "SW", depth + 1, maxDepth);
                        return false;
                    case "W":
                        if (currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + j].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate, "W", depth + 1, maxDepth);
                        return false;
                    case "NW":
                        if (currentSquare.neighbours[0] && currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate - 1, "NW", depth + 1, maxDepth);
                        return false;
                    default:
                        throw new ArgumentException();
                }
            }
            else
                return false;
        }

        // Check for illegal game states or moves before the call to the base class
        foreach (Square square in currentGameState.squares)
        {
            if (square.coordinates[0] == destinationCoordinates[0] &&
                square.coordinates[1] == destinationCoordinates[1] &&
                square.piece != null)
                throw new InvalidMoveException(_onePiecePerSquareError);
        }

        // Call to base class to perform the move
        base.move(sourceCoordinates, destinationCoordinates, piece, player);

        // Check for win/draw/loss after the call to the base class
        foreach (Square square in currentGameState.squares)
        {
            string[] directions = new string[] {"N", "NE", "E", "SE",
                                                "S", "SW", "W", "NW"};

            int xCoordinate = square.coordinates[0];
            int yCoordinate = square.coordinates[1];

            foreach (string direction in directions)
            {
                // You win in Gomoku by making a line of five. This checks
                // a line of the same Piece that is five pieces long either
                // virtically, horizontally or on the diagonal
                if (checkForLine(currentGameState, xCoordinate, yCoordinate, direction, maxDepth: lineLength))
                    // At this point we've already moved into a game state after
                    // the winning move has already been played. Set the winner to
                    // the current player in the previous state
                    winner = gameStates[gameStates.IndexOf(currentGameState) - 1].currentPlayer;
            }
        }

        if (gameLength > maxMoves)
            draw = true;
    }
}

// Extend the basic (empty) computer player here
// with game-specific logic. This player will only attempt
// to place a piece in a random square
public class easyGomokuComputer : Computer
{
    public easyGomokuComputer(string name) : base(name) { }

    public easyGomokuComputer() : base() { }

    public override Agent nextMove(Game game)
    {
        var rand = new Random();
        int xDimensions = game.initialGameState.xDimensions;
        int yDimensions = game.initialGameState.yDimensions;

        while (true)
        {
            // Get co-ordinates for a random next move
            int[] coordinates = new int[2];
            coordinates[0] = rand.Next(xDimensions);
            coordinates[1] = rand.Next(yDimensions);

            try
            {
                // Attempt to make the move. If the move was succesfull,
                // yield to the next player
                game.move(sourceCoordinates: null,
                          destinationCoordinates: coordinates,
                          piece: this.pieces[0]);
                return game.NextPlayer();
            }
            catch (InvalidMoveException)
            {
                // If the move was an invalid move, catch the exception
                // and attempt to make a move in a different location
                continue;
            }
        }
    }
}

// Here we define a second computer player that is more difficult to beat
// This player is highly defensive: it will attempt to block the creation
// of any lines of 2, 3 or 4 pieces. However, it will not attempt to prevent
// the creation of a line of 5 pieces out of a 2-piece line and another 2-piece
// line, for example. It will only attempt to win if it has already
// accidentally created a line with 4 pieces 
public class hardGomokuComputer : easyGomokuComputer
{
    public hardGomokuComputer(string name) : base(name) { }

    public hardGomokuComputer() : base() { }

    List<Object[]> winningMoves = new List<Object[]>();
    List<Object[]> blockingMoves = new List<Object[]>();

    public override Agent nextMove(Game game)
    {
        Board currentGameState = game.currentGameState;
        string[] directions = new string[] {"N", "NE", "E", "SE",
                                            "S", "SW", "W", "NW"};
        int winningLineLength = 5;
        int[] blockingLineLengths = { 2, 3, 4 };

        // Very similar to the above
        bool checkForLine(Board board, int xCoordinate, int yCoordinate,
                      string direction = null, int depth = 1, int maxDepth = 1)
        {
            int xDimensions = board.xDimensions;
            int yDimensions = board.yDimensions;

            int i = xCoordinate;
            int j = yCoordinate;

            Square currentSquare = board.squares[(i * yDimensions) + j];
            Piece currentPiece = board.squares[(i * yDimensions) + j].piece;

            if (depth == maxDepth)
                return true;

            if (currentPiece != null)
            {
                switch (direction)
                {
                    case "N":
                        if (currentSquare.neighbours[0] &&
                            (currentPiece.Equals(board.squares[(i * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate, yCoordinate - 1, "N", depth + 1, maxDepth);
                        return false;
                    case "NE":
                        if (currentSquare.neighbours[0] && currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate - 1, "NE", depth + 1, maxDepth);
                        return false;
                    case "E":
                        if (currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + j].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate, "E", depth + 1, maxDepth);
                        return false;
                    case "SE":
                        if (currentSquare.neighbours[1] && currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate + 1, "SE", depth + 1, maxDepth);
                        return false;
                    case "S":
                        if (currentSquare.neighbours[1] &&
                            (currentPiece.Equals(board.squares[(i * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate, yCoordinate + 1, "S", depth + 1, maxDepth);
                        return false;
                    case "SW":
                        if (currentSquare.neighbours[1] && currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate + 1, "SW", depth + 1, maxDepth);
                        return false;
                    case "W":
                        if (currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + j].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate, "W", depth + 1, maxDepth);
                        return false;
                    case "NW":
                        if (currentSquare.neighbours[0] && currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate - 1, "NW", depth + 1, maxDepth);
                        return false;
                    default:
                        throw new ArgumentException();
                }
            }
            else
                return false;
        }

        // Attempt to play a list of moves in order presented. Move
        // on to the next move in the list if any move is considered
        // invalid
        Agent playMove(List<Object[]> moves)
        {
            foreach (var nextMove in moves)
            {
                int newXCoordinate = (int)nextMove[0];
                int newYCoordinate = (int)nextMove[1];
                string direction = (string)nextMove[2];
                int lineLength = (int)nextMove[3];

                switch (direction)
                {
                    case "N":
                        newYCoordinate -= lineLength;
                        break;
                    case "NE":
                        newXCoordinate += lineLength;
                        newYCoordinate -= lineLength;
                        break;
                    case "E":
                        newXCoordinate += lineLength;
                        break;
                    case "SE":
                        newXCoordinate += lineLength;
                        newYCoordinate += lineLength;
                        break;
                    case "S":
                        newYCoordinate += lineLength;
                        break;
                    case "SW":
                        newXCoordinate -= lineLength;
                        newYCoordinate += lineLength;
                        break;
                    case "W":
                        newXCoordinate -= lineLength;
                        break;
                    case "NW":
                        newXCoordinate -= lineLength;
                        newYCoordinate -= lineLength;
                        break;
                    default:
                        throw new ArgumentException();
                }

                int[] newDestinationCoordinates = new int[] { newXCoordinate, newYCoordinate };

                try
                {
                    game.move(sourceCoordinates: null,
                              destinationCoordinates: newDestinationCoordinates,
                              piece: this.pieces[0]);
                    return game.NextPlayer();
                }
                catch (InvalidMoveException)
                {
                    continue;
                }
            }

            return null;
        }

        // Iterate through the squares. Try to find one where the opponent has
        // already made a line of two, three or four, or that we have already
        // made a line of four
        foreach (Square square in currentGameState.squares)
        {
            int xCoordinate = square.coordinates[0];
            int yCoordinate = square.coordinates[1];

            foreach (string direction in directions)
            {
                if (checkForLine(currentGameState, xCoordinate, yCoordinate, direction,
                                  maxDepth: winningLineLength - 1))

                {
                    if (this.pieces.Contains(square.piece))
                    {
                        // Found a string of four that this player has already formed. Add to the
                        // list of possible next moves
                        winningMoves.Add(new Object[] { xCoordinate, yCoordinate, direction, winningLineLength - 1});
                    }
                }
                foreach (int lineLength in blockingLineLengths)
                {
                    if (checkForLine(currentGameState, xCoordinate, yCoordinate, direction,
                                     maxDepth: lineLength))
                    {
                        if (!this.pieces.Contains(square.piece))
                        {
                            // Found a line of two, three or four that another player
                            // has already formed. Block a line by placing a piece
                            // at the end, if possible
                            if (blockingMoves.Count != 0 && (int)blockingMoves[0][3] <= lineLength)
                            {
                                // Ensure longer lines are treated as more dangerous than
                                // shorter lines
                                blockingMoves.Insert(0, new Object[] { xCoordinate, yCoordinate,
                                                                       direction, lineLength });
                            }
                            else
                            {
                                blockingMoves.Add(new Object[] { xCoordinate, yCoordinate,
                                                                       direction, lineLength });
                            }

                        }
                    }
                }
            }
        }

        // Here we have a list of possible next move candidates. Attempt
        // to play a winning move first (if available), then play a
        // blocking move when no winning moves available. If we have
        // exhausted our available winning and blocking moves, make a
        // random move instead.
        
        Agent winningMovePlay = playMove(winningMoves);
        Agent blockingMovePlay = null;

        if (winningMovePlay == null && game.winner == null)
        {
            // We haven't been able to play a winning move. Attempt a blocking
            // move instead
            blockingMovePlay = playMove(blockingMoves);
        }

        if (winningMovePlay != null || game.winner != null)
        {
            // We've won! Let the game progression logic know
            return winningMovePlay;
        }

        // If control has fallen through to here, we haven't been able to
        // play a winning move. Attempt to play a blocking move instead
        if (blockingMovePlay != null)
        {
            return blockingMovePlay;
        }

        // If unable to identify a winning or blocking move, just return
        // a random next move as per the easy computer player
        return base.nextMove(game);
    }
}


public class PlayGomoku
{
    static void Main()
    {
        Console.Clear();

        Gomoku game = new Gomoku();

        bool beginGame = game.init();

        if (beginGame)
        {
            Agent nextPlayer = game.firstPlayer;

            while (nextPlayer != null)
            {
                Console.WriteLine();
                Console.WriteLine(game.currentGameState.ToString());
                nextPlayer = game.currentPlayer.nextMove(game);
            }

            // If we're exiting the main game loop.
            if (game.winner != null || game.draw == true)
            {    // Paint the board one final time
                Console.WriteLine();
                Console.WriteLine(game.currentGameState.ToString());
            }
        }

        if (game.winner != null)
            Console.WriteLine(game.winner.name + " is the winner!");

        if (game.winner == null && game.draw == true)
            Console.WriteLine("It was a draw!");

        if (game.winner == null && game.draw == false)
        {
            Console.WriteLine("Press any key to Quit");
            Console.ReadKey();
        }
    }
}