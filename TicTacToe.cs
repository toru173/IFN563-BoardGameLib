using System;
using System.Collections.Generic;
using BoardGameLibs;

public class TicTacToePiece : Piece
{
    public TicTacToePiece(string description, char representation) :
        base(description, representation) { }

    public TicTacToePiece(TicTacToePiece piece) : base(piece) { }

    public TicTacToePiece() : base() { }

    public override void Place(Square square)
    {
        square.piece = this;
    }
}


public class NoughtsPiece : TicTacToePiece
{
    public NoughtsPiece() :
        base(description: "Nought", representation: 'O') { }

    public NoughtsPiece(NoughtsPiece piece) : base(piece) { }
}


public class CrossesPiece : TicTacToePiece
{
    public CrossesPiece() :
        base(description: "Cross", representation: 'X') { }

    public CrossesPiece(CrossesPiece piece) : base(piece) { }

}


public class TicTacToeBoard : Board
{
    public TicTacToeBoard() : base(xDimensions: 3, yDimensions: 3) { }

    public TicTacToeBoard(TicTacToeBoard board) : base(board) { }

}


public class TicTacToe : Game
{
    public TicTacToe() : base(numberOfPlayers: 2)
    {
        board = new TicTacToeBoard();

        preamble =
            "Welcome to Tic Tac Toe, the first game implimented with the IFN563\n" +
            "board game library created in Object Oriented C#. Type \'rules\' at any\n" +
            "time to read the game rules, \'help\' for game help or type \'quit\'\n" +
            "at any time to exit.\n";

        loadSavedGameDialogue =
            "Would you like to create a new game or load saved game of Tic Tac Toe?\n" +
            "type \'new\' to start a new game or \'load\' to load a saved game, or\n" +
            "type \'quit\' to exit: ";

        humanReadableRules =
            "You can place a piece on the board at any square that doesn't already\n" +
            "have a piece in it. In Tic Tac Toe, one can only place a piece. It cannot\n" +
            "be moved once placed. The first player to make a row of three wins!\n";

        humanReadableValidMoves =
             "You can place a piece by giving the co-ordinates of the piece, such\n" +
             "as \'A 1\', \'1 a\' etc. You can save the current game with the \'save\'\n" +
             "keyword, or use \'load\' to load a saved game at any time. Moves can be\n" +
             "Undone or Redone using the \'undo\' and \'redo\' keywords at any turn.\n";

        createPlayersPreamble =
            "This is a game for two players, either of which can be a Human or\n" +
            "Computer player. Player 1 will play as Noughts while player 2 will\n" +
            "be Crosses. Please set up the types of players competing in\n" +
            "this game. Type \'h\' or \'human\' for a human player and \'c\' or\n" +
            "\'computer\' for a computer player:\n";
    }

    public override bool init()
    {
        bool createGame = base.init();

        if (createGame && players.Count == 0)
        {
            initPlayers();
        }
        return createGame;
    }


    public override void initPlayers()
    {
        Type[] computerPlayerDifficulties = { typeof(easyTicTacToeComputer),
                                typeof(hardTicTacToeComputer)};
        createPlayers(computerPlayers: computerPlayerDifficulties);

        Agent playerOne = players[0];
        Agent playerTwo = players[1];

        playerOne.pieces.Add(new NoughtsPiece());
        playerTwo.pieces.Add(new CrossesPiece());
    }


    public override void move(int[] sourceCoordinates,
                              int[] destinationCoordinates,
                              Piece piece,
                              Agent player)
    {

        // Declare game-specific variables at the top of the move function
        int maxMoves = 9;
        string _onePiecePerSquareError = "Placing a piece on top of another is" +
                                         " not valid a valid move in Tic Tac Toe";

        // Next, include any helper functions required to facilitate a move
        // in this game, or used to check game states
        bool checkForLine(Board board, int xCoordinate, int yCoordinate,
                          string direction = null, int depth = 1, int maxDepth = 3)
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
                            return checkForLine(board, xCoordinate, yCoordinate - 1, "N", depth + 1);
                        return false;
                    case "NE":
                        if (currentSquare.neighbours[0] && currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate - 1, "NE", depth + 1);
                        return false;
                    case "E":
                        if (currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + j].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate, "E", depth + 1);
                        return false;
                    case "SE":
                        if (currentSquare.neighbours[1] && currentSquare.neighbours[2] &&
                            (currentPiece.Equals(board.squares[((i + 1) * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate + 1, yCoordinate + 1, "SE", depth + 1);
                        return false;
                    case "S":
                        if (currentSquare.neighbours[1] &&
                            (currentPiece.Equals(board.squares[(i * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate, yCoordinate + 1, "S", depth + 1);
                        return false;
                    case "SW":
                        if (currentSquare.neighbours[1] && currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + (j + 1)].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate + 1, "SW", depth + 1);
                        return false;
                    case "W":
                        if (currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + j].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate, "W", depth + 1);
                        return false;
                    case "NW":
                        if (currentSquare.neighbours[0] && currentSquare.neighbours[3] &&
                            (currentPiece.Equals(board.squares[((i - 1) * yDimensions) + (j - 1)].piece)))
                            return checkForLine(board, xCoordinate - 1, yCoordinate - 1, "NW", depth + 1);
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

        // Call to base class to do the move
        base.move(sourceCoordinates, destinationCoordinates, piece, player);

        // Check for win/draw/loss after the call to the base class
        foreach (Square square in currentGameState.squares)
        {
            string[] directions = new string[] {"N", "NE", "E", "SE",
                                                "S", "SW", "W", "NW"};

            int xCoordinate = square.coordinates[0];
            int yCoordinate = square.coordinates[1];

            foreach(string direction in directions)
            {
                // You win in Tic Tac Toe by making a line of three. This checks
                // a line of the same Piece that is three pieces long either
                // virtically, horizontally or on the diagonal
                if (checkForLine(currentGameState, xCoordinate, yCoordinate, direction))
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


public class easyTicTacToeComputer : Computer
{
    public easyTicTacToeComputer(string name) : base(name) { }

    public easyTicTacToeComputer() : base() { }

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
                // Attempt to make the move. If the move was sucesfull,
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

public class hardTicTacToeComputer : easyTicTacToeComputer
{
    public hardTicTacToeComputer(string name) : base(name) { }

    public hardTicTacToeComputer() : base() { }

    List<Object[]> winningMoves = new List<Object[]>();
    List<Object[]> blockingMoves = new List<Object[]>();

    public override Agent nextMove(Game game)
    {
        Board currentGameState = game.currentGameState;
        string[] directions = new string[] {"N", "NE", "E", "SE",
                                            "S", "SW", "W", "NW"};
        int lineLength = 2;


        bool checkForLine(Board board, int xCoordinate, int yCoordinate,
                      string direction = null, int depth = 1, int maxDepth = 2)
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

        Agent playMove(List<Object[]> moves)
        {
            foreach (var nextMove in moves)
            {
                int newXCoordinate = (int)nextMove[0];
                int newYCoordinate = (int)nextMove[1];
                string direction = (string)nextMove[2];

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
                    Console.Write("Attempting move (x, y): ");
                    Console.Write(newDestinationCoordinates[0].ToString() + ", ");
                    Console.WriteLine(newDestinationCoordinates[1].ToString());

                    game.move(sourceCoordinates: null,
                              destinationCoordinates: newDestinationCoordinates,
                              piece: this.pieces[0]);

                    Console.Write("Succesful Move (x,y): ");
                    Console.Write(newDestinationCoordinates[0].ToString() + ", ");
                    Console.WriteLine(newDestinationCoordinates[1].ToString());
                    return game.NextPlayer();
                }
                catch (InvalidMoveException)
                {
                    Console.Write("Invalid Move (x,y): ");
                    Console.Write(newDestinationCoordinates[0].ToString() + ", ");
                    Console.WriteLine(newDestinationCoordinates[1].ToString());
                    continue;
                }
            }

            return null;
        }

        // Iterate through the squares. Try to find one where the opponent has
        // already made a line of two, or that we have already made a line of two
        foreach (Square square in currentGameState.squares)
        {
            int xCoordinate = square.coordinates[0];
            int yCoordinate = square.coordinates[1];

            foreach (string direction in directions)
            {
                if (checkForLine(currentGameState, xCoordinate, yCoordinate, direction,
                                  maxDepth: lineLength))

                {
                    if (this.pieces.Contains(square.piece))
                    {
                        // Found a string of two that this player has already formed. Add to the
                        // list of possible next moves
                        winningMoves.Add(new Object[] { xCoordinate, yCoordinate, direction });
                    }
                    else
                    {
                        // Found a list of two that another player has already formed.
                        // Block a third placed move, if possible
                        blockingMoves.Add(new Object[] { xCoordinate, yCoordinate, direction });
                    }
                }
            }
        }

        Console.WriteLine("Possible winning Moves:");
        foreach(Object[] move in winningMoves)
        {
            Console.Write("(x, y, direction): ");
            Console.Write(move[0].ToString() + ", ");
            Console.Write(move[1].ToString() + ", ");
            Console.WriteLine(move[2].ToString());
        }

        Console.WriteLine("Possible blocking Moves:");
        foreach (Object[] move in blockingMoves)
        {
            Console.Write("(x, y, direction): ");
            Console.Write(move[0].ToString() + ", ");
            Console.Write(move[1].ToString() + ", ");
            Console.WriteLine(move[2].ToString());
        }

        // Here we have a list of possible next move candidates. Attempt
        // to play a winning move first (if available), then play a
        // blocking move when no winning moves available. If we have
        // exhausted our available winning and blocking moves, make a
        // random move instead.

        Console.WriteLine("Attempting winning moves:");
        Agent winningMovePlay = playMove(winningMoves);
        Agent blockingMovePlay = null;

        if (winningMovePlay == null && game.winner == null)
        {
            // We haven't been able to play a winning move. Attempt a blocking
            // move instead
            Console.WriteLine("Attempting blocking moves:");
            blockingMovePlay = playMove(blockingMoves);
        }

        if (winningMovePlay != null || game.winner != null)
        {
            Console.WriteLine("Played a winning move");
            return winningMovePlay;
        }

        // If control has fallen through to here, we haven't been able to
        // play a winning move. Attempt to play a blocking move instead
        if (blockingMovePlay != null)
        {
            Console.WriteLine("Played a blocking move");
            return blockingMovePlay;
        }

        // If unable to identify a winning or blocking move, just return
        // a random next move as per the easy computer player
        Console.WriteLine("Played a random move");
        return base.nextMove(game);
    }
}


public class PlayTicTacToe
{
    static void playGame()
    {
        Console.Clear();

        TicTacToe game = new TicTacToe();

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

            // If we're not quitting
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
            Console.WriteLine("Quitting...");
    }
}