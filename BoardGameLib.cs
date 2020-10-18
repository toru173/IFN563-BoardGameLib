using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;


namespace BoardGameLibs
{
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() { }
        public InvalidMoveException(string message) : base(message) { }
        public InvalidMoveException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class Piece
    {
        public char representation;
        public string description;

        public Piece() { }

        public Piece(string description, char representation)
        {
            this.description = description;
            this.representation = representation;

        }

        public Piece(Piece other)
        {
            this.description = other.description;
            this.representation = other.representation;
        }

        public virtual void Place(Square square = null)
        {
            // Placing a piece is game-specific. Raise an exception if
            // this method isn't overridden
            throw new InvalidMoveException("Placing a piece is not supported in this game");
        }

        public virtual void Move(Square oldSquare = null, Square newSquare = null)
        {
            // Moving a piece is game-specific. Raise an exception if
            // this method isn't overridden
            throw new InvalidMoveException("Moving a piece is not supported in this game");
        }

        public virtual void Remove(Square square = null)
        {
            // Removing a piece is game-specific. Raise an exception if
            // this method isn't overridden
            throw new InvalidMoveException("Removing a piece is not supported in this game");
        }

        public virtual void Replace(Square square, Piece newPiece = null)
        {
            // Replacing a piece is game-specific. Raise an exception if
            // this method isn't overridden
            throw new InvalidMoveException("Replacing a piece is not supported in this game");
        }

        public override string ToString()
        {
            return representation.ToString();
        }

        public override bool Equals(Object other)
        {
            if (other == null)
                return false;

            if (other.GetType() != this.GetType())
                return false;

            var otherPiece = other as Piece;

            if (otherPiece.description != this.description)
                return false;
            if (otherPiece.representation != this.representation)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Square
    {
        // Unicode Box Drawing characters from
        // https://www.unicode.org/charts/PDF/U2500.pdf
        // Variables are protected rather than private in the case
        // of a class inheriting Square and wishing to override the default
        // stringRepresentation sting
        protected char upperLeftCorner = '\x250C';
        protected char upperRightCorner = '\x2510';
        protected char lowerLeftCorner = '\x2514';
        protected char lowerRightCorner = '\x2518';

        protected char leftSide = '\x2502';
        protected char rightSide = '\x2502';
        protected char topSide = '\x2500';
        protected char bottomSide = '\x2500';

        protected char northPointingT = '\x2534';
        protected char southPointingT = '\x252C';
        protected char eastPointingT = '\x251C';
        protected char westPointingT = '\x2524';

        protected char cross = '\x253C';

        protected char newline = '\n';

        // Neighbours is an array of four booleans stating whether
        // the current square has neighbours North, South, East or West
        public bool[] neighbours = new bool[] { false, false, false, false };
        public int[] coordinates;

        public virtual string gridStringRepresentation
        {
            // This string representations assumes that the square is used in a
            // grid pattern, similar to tic-tac-toe or chess. This ought to be
            // overridden if the string representation of a square is different,
            // such as that in Nine Men’s Morris.

            get
            {
                // Drawing the edges is the responsibility
                // of the West-most or north-most square
                string returnString = "";

                // If no neighbours to the North or West, draw the corner
                if (neighbours[0] == false && neighbours[3] == false)
                    returnString += upperLeftCorner;

                // If no neighbours to the North, draw the top
                if (neighbours[0] == false)
                {
                    returnString += topSide;
                    returnString += topSide;
                    returnString += topSide;
                }

                // If no neighbours to the north or East, draw the corner
                if (neighbours[0] == false && neighbours[2] == false)
                    returnString += upperRightCorner;

                // If no neighbours to the north and neighbours to the east
                // draw the south pointing T
                if (neighbours[0] == false && neighbours[2] == true)
                    returnString += southPointingT;

                // If no neighbours to the north we have a first line. Add a newline
                if (neighbours[0] == false)
                    returnString += newline;

                // If no neighbours to the West, draw the left hand side
                if (neighbours[3] == false)
                    returnString += leftSide;

                // Add the string representation of the piece on this square if it
                // exists
                if (piece != null)
                    returnString += " " + piece.ToString() + " ";
                else
                    returnString += "   ";

                // Square is always responsible for drawing the eastern side
                returnString += rightSide;
                returnString += newline;

                // If no neighbours to the south and west draw the corner
                if (neighbours[1] == false && neighbours[3] == false)
                    returnString += lowerLeftCorner;

                // If neighbours to the south and no neighbours
                // to the west draw the east pointing T
                if (neighbours[1] == true && neighbours[3] == false)
                    returnString += eastPointingT;

                // Square always responsible for drawing the bottom
                returnString += bottomSide;
                returnString += bottomSide;
                returnString += bottomSide;

                // If no neighbours to the south or east draw the corner
                if (neighbours[1] == false && neighbours[2] == false)
                    returnString += lowerRightCorner;

                // If no neighbours to the south but neighbours to the east
                // draw the north pointing T
                if (neighbours[1] == false && neighbours[2] == true)
                    returnString += northPointingT;

                // If neighbours to the south and no neighbours to the east
                // draw the west pointing T
                if (neighbours[1] == true && neighbours[2] == false)
                    returnString += westPointingT;

                // If neighbours to the south and the east draw the cross
                if (neighbours[1] == true && neighbours[2] == true)
                    returnString += cross;

                return returnString;
            }
        }

        public Square() { }

        public Square(bool[] neighbours, int[] coordinates)
        {
            this.neighbours = neighbours;
            this.coordinates = coordinates;
        }

        public Square(Square other)
        {
            this.neighbours = other.neighbours;
            this.coordinates = other.coordinates;
            if (other.piece != null)
            {
                Type newPieceType = other.piece.GetType();
                Piece newPiece = Activator.CreateInstance(newPieceType, other.piece) as Piece;
                this.piece = newPiece;
            }
        }

        public Piece piece { get; set; }

        public override string ToString()
        {
            return gridStringRepresentation;
        }

        public override bool Equals(Object other)
        {
            if (other == null)
                return false;

            if (other.GetType() != this.GetType())
                return false;

            var otherSquare = other as Square;

            for(int i = 0; i < this.neighbours.Length; i++)
                if (this.neighbours[i] != otherSquare.neighbours[i])
                    return false;

            for (int i = 0; i < this.coordinates.Length; i++)
                if (this.coordinates[i] != otherSquare.coordinates[i])
                    return false;

            if (this.piece != null || otherSquare.piece != null)
            {
                if ((this.piece == null && otherSquare.piece != null) ||
                    (this.piece != null && otherSquare.piece == null))
                    return false;
                if (!otherSquare.piece.Equals(this.piece))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Board
    {
        // Currently allows for the creation of a max 26x26 sized board
        // The largest popular board game viable would be Go, with a 19x19
        // sized board
        public int xDimensions;
        public int yDimensions;

        // Serializing using XMLSerialiser does not support multidimensional
        // arrays! As such, array is a single dimensional array that is packed
        // as a 2D array. It needs to be made public or isn't serialised
        public Square[] squares;

        // Storing the current player in the board so it gets added to the
        // Game linked list as part of the current game state

        public Agent currentPlayer { get; set; }

        public Board() { }

        public Board(int xDimensions, int yDimensions)
        {
            if (xDimensions < 1 || xDimensions > 26)
                throw new ArgumentOutOfRangeException(xDimensions.ToString());

            if (yDimensions < 1 || yDimensions > 26)
                throw new ArgumentOutOfRangeException(yDimensions.ToString());

            this.xDimensions = xDimensions;
            this.yDimensions = yDimensions;

            this.squares = new Square[xDimensions * yDimensions];

            for (int j = 0; j < yDimensions; j++)
            {
                bool northernNeighbour = true;
                bool southernNeighbour = true;

                if (j == 0)
                    northernNeighbour = false;
                if (j == yDimensions - 1)
                    southernNeighbour = false;

                for (int i = 0; i < xDimensions; i++)
                {
                    bool easternNeighbour = true;
                    bool westernNeighbour = true;

                    if (i == xDimensions - 1)
                        easternNeighbour = false;
                    if (i == 0)
                        westernNeighbour = false;

                    bool[] neighbours = { northernNeighbour, southernNeighbour,
                                          easternNeighbour, westernNeighbour };

                    int[] coordinates = new int[] { i, j };

                    squares[(i * yDimensions) + j] = new Square(neighbours, coordinates);
                }
            }
        }

        public Board(Board other)
        {
            this.xDimensions = other.xDimensions;
            this.yDimensions = other.yDimensions;
            this.squares = new Square[xDimensions * yDimensions];

            for (int j = 0; j < other.yDimensions; j++)
            {
                for (int i = 0; i < other.xDimensions; i++)
                {
                    this.squares[(i * yDimensions) + j] =
                        new Square(other.squares[(i * yDimensions) + j]);
                }
            }
        }

        public override string ToString()
        {
            string xSquareNumbering = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string returnString = "      ";
            string returnLine;
            string squareString;
            int numLines;

            // We need to slice up the ToString() object returned by the squares
            // as we need to write the board line-by-line, not square-by-square.
            // The Square ToString() object is either a 4x3, 5x3, 4x2 or 5x2
            // string

            for (int i = 0; i < xDimensions; i++)
            {
                returnString += xSquareNumbering[i] + "   ";
            }

            returnString += "\n";


            for (int j = 0; j < yDimensions; j++)
            {
                if (j == 0)
                    numLines = 3;
                else
                    numLines = 2;

                for (int line = 0; line < numLines; line++)
                {
                    if ((line == 0 && j > 0) || (line == 1 && j == 0))
                    {
                        if (j + 1 < 10)
                            returnLine = "  " + (j + 1).ToString() + " ";
                        else
                            returnLine = " " + (j + 1).ToString() + " ";
                    }
                    else
                        returnLine = "    ";

                    for (int i = 0; i < xDimensions; i++)
                    {
                        squareString = squares[(i * yDimensions) + j].ToString();
                        String[] strings = squareString.Split('\n');
                        returnLine += strings[line];
                    }
                    returnString += returnLine + '\n';
                }
            }
            return returnString;
        }

        public override bool Equals(Object other)
        {
            if (other == null)
                return false;

            if (other.GetType() != this.GetType())
                return false;

            var otherBoard = other as Board;

            if (otherBoard.xDimensions != this.xDimensions)
                return false;
            if (otherBoard.yDimensions != this.yDimensions)
                return false;
            for (int j = 0; j < yDimensions; j++)
            {
                for (int i = 0; i < xDimensions; i++)
                {
                    if (!otherBoard.squares[(i * yDimensions) + j].Equals(
                        this.squares[(i * yDimensions) + j]))
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Game
    {
        private string _preamble = null;
        private string _loadSavedGameDialogue = null;
        private string _humanReadableRules = null;
        private string _humanReadableValidMoves = null;
        private string _createPlayersPreamble = null;

        private Type _gameType;

        public GameIO io;

        public int numberOfPlayers;
        public List<Agent> players = new List<Agent>();

        public Board initialGameState;
        public Board currentGameState;
        public List<Board> gameStates = new List<Board>();

        public Agent winner = null;
        public bool draw = false;


        // Parameterless constructor for serialisation
        public Game()
        {
            io = new GameIO(this);
        }

        public Game(int numberOfPlayers)
        {
            if (numberOfPlayers < 1)
                throw new ArgumentOutOfRangeException();
            this.numberOfPlayers = numberOfPlayers;
            io = new GameIO(this);
        }

        public Board board
        {
            get
            {
                return initialGameState;
            }
            set
            {
                if (initialGameState == null)
                {
                    initialGameState = value;
                    _gameType = value.GetType();
                    currentGameState = value;
                    gameStates.Add(initialGameState);
                }
            }
        }

        public string preamble
        {
            get
            {
                return _preamble;
            }
            set
            {
                if (_preamble == null)
                    _preamble = value;
            }
        }

        public string loadSavedGameDialogue
        {
            get
            {
                return _loadSavedGameDialogue;
            }
            set
            {
                if (_loadSavedGameDialogue == null)
                    _loadSavedGameDialogue = value;
            }
        }

        public string humanReadableRules
        {
            get
            {
                return _humanReadableRules;
            }
            set
            {
                if (_humanReadableRules == null)
                    _humanReadableRules = value;
            }
        }

        public string humanReadableValidMoves
        {
            get
            {
                return _humanReadableValidMoves;
            }
            set
            {
                if (_humanReadableValidMoves == null)
                    _humanReadableValidMoves = value;
            }
        }

        public string createPlayersPreamble
        {
            get
            {
                return _createPlayersPreamble;
            }
            set
            {
                if (_createPlayersPreamble == null)
                    _createPlayersPreamble = value;
            }
        }

        public virtual Agent firstPlayer
        {
            get
            {
                return players[0];
            }
        }

        public Agent currentPlayer
        {
            get
            {
                return currentGameState.currentPlayer;
            }
        }

        public int gameLength
        {
            get
            {
                return gameStates.Count;
            }
        }

        public void addGameState(Board board)
        {
            if (gameStates.IndexOf(currentGameState) != gameLength - 1)
            {
                // An Agent has triggered one or more undo moves. By then attempting
                // to add a new state, all those states considered to be 'in the
                // future' (ie, further along the LinkedList of game states than
                // the current state) must be trimmed. Undoing triggers Back-To-
                // The-Future style time travel - a new timeline is ALWAYS created
                // if you make any changes in the past. This can be dissapointing
                // to the user if they undo a move, then the AI decides to do a
                // different move. Such is life!

                while (gameStates.IndexOf(currentGameState) != gameLength - 1)
                {
                    // Trim the future timeline of moves to the current move
                    gameStates.Remove(gameStates[gameLength - 1]);
                }
            }
            gameStates.Add(board);
            currentGameState = gameStates[gameLength - 1];
        }

        public bool undoGameState()
        {
            if (!gameStates[0].Equals(currentGameState))
            {
                // Not at the beginning of our list of moves. Move one further up the list
                currentGameState = gameStates[gameStates.IndexOf(currentGameState) - 1];
                return true;
            }
            // Cannot undo the first move
            return false;
        }

        public bool redoGameState()
        {
            if (gameLength == 1)
                return false;
            if (!gameStates[gameLength - 1].Equals(currentGameState))
            {
                // Not at the end of our list of moves. Move one further down the list
                currentGameState = gameStates[gameStates.IndexOf(currentGameState) + 1];
                return true;
            }
            // Cannot redo the last move
            return false;
        }

        public virtual bool init()
        {
            string input = null;

            Console.Write(preamble);
            Console.WriteLine();
            Console.Write(loadSavedGameDialogue);

            while (input == null)
            {
                input = Console.ReadLine();

                if (input.ToUpper() == "NEW")
                    return true;

                if (input.ToUpper() == "LOAD")
                {
                    Game loadedGame = null;

                    try
                    {
                        while (loadedGame == null)
                            loadedGame = this.io.Load();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return false;
                    }

                    this.players = loadedGame.players;
                    this.gameStates = loadedGame.gameStates;
                    this.initialGameState = loadedGame.initialGameState;
                    this.currentGameState = loadedGame.currentGameState;
                    return true;
                }

                if (input.ToUpper() == "QUIT")
                    return false;

                Console.Write("Sorry, " + input + " is not a recognised option\n");
                input = null;
            }
            return false;
        }

        public void createPlayers(Type[] computerPlayers = null)
        {
            string playerTypeInput;

            Console.WriteLine();
            Console.Write(_createPlayersPreamble);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                bool validPlayer = false;

                while (!validPlayer)
                {
                    Console.Write("Player " + (i + 1).ToString() + ": ");
                    playerTypeInput = Console.ReadLine();

                    if (playerTypeInput.ToUpper() == "H" || playerTypeInput.ToUpper() == "HUMAN")
                    {
                        Agent newPlayer = new Human("Player " + (i + 1).ToString());
                        players.Add(newPlayer);
                        validPlayer = true;
                    }

                        if (playerTypeInput.ToUpper() == "C" ||
                        playerTypeInput.ToUpper() == "COMPUTER")
                    {
                        if (computerPlayers != null)
                        {
                            string difficulty = "";
                            Console.Write("Difficulty level (easy or hard): ");
                            difficulty = Console.ReadLine();
                            while (difficulty.ToUpper() != "EASY" && difficulty.ToUpper() != "HARD")
                            {
                                Console.WriteLine("Unrecognised input: " + difficulty);
                                Console.Write("Difficulty level (easy or hard): ");
                                difficulty = Console.ReadLine();
                            }

                            if(difficulty.ToUpper() == "EASY")
                            {
                                Agent newPlayer = Activator.CreateInstance(computerPlayers[0]) as Agent;
                                newPlayer.name = "Player " + (i + 1).ToString();
                                players.Add(newPlayer);
                                validPlayer = true;
                            }

                            if(difficulty.ToUpper() == "HARD")
                            {
                                Agent newPlayer = Activator.CreateInstance(computerPlayers[1]) as Agent;
                                newPlayer.name = "Player " + (i + 1).ToString();
                                players.Add(newPlayer);
                                validPlayer = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Computer players not allowed");
                            continue;
                        }

                    }
                    if (!validPlayer)
                    {
                        Console.WriteLine("Sorry, " + playerTypeInput + " is not a recognised player type");
                        continue;
                    }
                }
            }
            // Once players are instantiated, add the first player into the game states
            initialGameState.currentPlayer = this.firstPlayer;
            currentGameState.currentPlayer = this.firstPlayer;
        }

        public virtual void initPlayers()
        {
            // Initialising players is Game implimentation specific
            throw new NotImplementedException("Player Initialisation is Game specific");
        }

        public virtual Agent NextPlayer()
        {
            // Impliments a loop around the List of players. Can be overriden
            // for games that do not simply alternate between players. Returns null
            // if there is a winner or no move moves available: will be interpreted
            // as the end of the game

            if (winner != null)
                return null;
            if (draw == true)
                return null;
            if (currentPlayer.Equals(players[players.Count - 1]) || players.Count == 1)
                return players[0];
            int nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % players.Count;
            return players[nextPlayerIndex];
        }

        public virtual void move(int[] sourceCoordinates,
                                 int[] destinationCoordinates,
                                 Piece piece = null,
                                 Agent player = null)
        {
            // Override and extend this method for Game implimentation specific rules.
            // For anything that is an illegal move, throw a new InvalidMoveException
            // as per the examples below.

            int xDimensions = initialGameState.xDimensions;
            int yDimensions = initialGameState.yDimensions;

            string _coordinatesOutOfRangeError = "Source or Destination Co-ordinates " +
                                                 "outside of board bounds";

            if (player == null)
                player = this.currentPlayer;

            int sourceX = -1;
            int sourceY = -1;

            int destinationX = -1;
            int destinationY = -1;

            // Check if we've been given dimensions outside of the current board dimensions

            if (sourceCoordinates != null)
            {
                if (sourceCoordinates[0] < 0 || sourceCoordinates[0] > (xDimensions - 1))
                    throw new InvalidMoveException(_coordinatesOutOfRangeError);
                if (sourceCoordinates[1] < 0 || sourceCoordinates[1] > (yDimensions - 1))
                    throw new InvalidMoveException(_coordinatesOutOfRangeError);
                sourceX = sourceCoordinates[0];
                sourceY = sourceCoordinates[1];
            }

            if (destinationCoordinates != null)
            {
                if (destinationCoordinates[0] < 0 || destinationCoordinates[0] > (xDimensions - 1))
                    throw new InvalidMoveException(_coordinatesOutOfRangeError);
                if (destinationCoordinates[1] < 0 || destinationCoordinates[1] > (yDimensions - 1))
                    throw new InvalidMoveException(_coordinatesOutOfRangeError);
                destinationX = destinationCoordinates[0];
                destinationY = destinationCoordinates[1];
            }

            Board newGameState = Activator.CreateInstance(_gameType, currentGameState) as Board;
            newGameState.currentPlayer = NextPlayer();

            if (sourceCoordinates != null)
            {
                // The Move is to move a piece from one square to another
                Square sourceSquare = newGameState.squares[sourceX * xDimensions + sourceY];
                Square destinationSquare = newGameState.squares[destinationX * yDimensions + destinationY];

                if (sourceSquare.piece != null)
                    sourceSquare.piece.Move(sourceSquare, destinationSquare);
                else
                    throw new InvalidMoveException("There is no piece to move from that square");
            }
            else
            {
                Square destinationSquare = newGameState.squares[destinationX * yDimensions + destinationY];
                piece.Place(destinationSquare);
            }

            addGameState(newGameState);
        }
    }

    public class Agent
    {
        public List<Piece> pieces = new List<Piece>();
        public string name { get; set; }

        public Agent() { }

        public Agent(string name)
        {
            this.name = name;
        }

        public virtual Agent nextMove(Game game)
        {
            //nextMove is Agent-specific, but returns the next player according
            // to the game if the move was valid and successul, or null if there
            // is no next user (IE, game has ended)
            throw new NotImplementedException("Agent Move Logic is Agent Specific");
        }

        public override bool Equals(Object other)
        {
            if (other == null)
                return false;

            if (other.GetType() != this.GetType())
                return false;

            var otherAgent = other as Agent;

            if (otherAgent.name != this.name)
                return false;

            foreach (Piece piece in this.pieces)
            {
                bool foundPiece = false;
                foreach (Piece otherPiece in otherAgent.pieces)
                {
                    if (piece.Equals(otherPiece))
                        foundPiece = true;
                }
                return foundPiece;
            }

            foreach (Piece otherPiece in otherAgent.pieces)
            {
                bool foundPiece = false;
                foreach (Piece piece in this.pieces)
                {
                    if (piece.Equals(otherPiece))
                        foundPiece = true;
                }
                return foundPiece;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Human : Agent
    {
        private bool parseInput(string input, out int[] sourceCoordinates,
                                              out int[] destinationCoordinates,
                                              out Piece piece)
        {
            // Attempt to extract a valid set of co-ordinates from (fairly) natural
            // language user input. This is not Game implimentation specific and
            // strives to be as generalised as possible.

            // If the word 'to' is found, assume this is a Move move. Otherwise it
            // is a move that Places, Replaces or Removes a piece. In all cases,
            // attempt to get co-ordinates from the input. Input can be in the
            // form of "a1", "1A", "1 a" etc and include unnecessary verbose
            // natural language. 'Move the knight in B3 to 4 a' is valid input,
            // though may not be a valid move.

            // Return null if the input does not contain a valid set of co-ordinate
            // pairs, otherwise return the co-ordinates found. Note: the move
            // may not be valid or legal in the context of the Game. Detecting a 
            // legal move is implimentation specific.

            string alphaCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string[] tokens = input.ToUpper().Split(' ');
            string[] destinationTokens;
            string[] sourceTokens;

            int toIndex = 0;

            char tokenToAlphaCoordinate(string token)
            {
                // Returns a sole alpha character if found in token, otherwise
                // returns Unicode null character
                bool foundAlpha = false;
                char alphaCharacter = '\0';

                for (int i = 0; i < token.Length; i++)
                {
                    foreach (char character in alphaCharacters)
                    {
                        if (token[i] == character)
                        {
                            // We've already found an alpha character in this token
                            // So the input is null
                            if (foundAlpha == true)
                                return '\0';
                            else
                            {
                                foundAlpha = true;
                                alphaCharacter = token[i];
                            }
                        }
                    }
                }
                return alphaCharacter;
            }

            int tokenToNumericalCoordinate(string token)
            {
                // Returns a single or two digit number in a given token.
                // If none found, returns -1
                string numbers = "0123456789";
                string numberString = "";
                int previousNumberIndex = -1;
                int number = 0;

                for (int i = 0; i < token.Length; i++)
                {
                    foreach (char numberCharacter in numbers)
                    {
                        if (token[i] == numberCharacter)
                        {
                            if (previousNumberIndex == -1)
                            {
                                // This is the first number we're found in
                                // the token. Add it to our number string
                                numberString += token[i];
                                previousNumberIndex = i;
                            }
                            else
                            {
                                // We've found a number previously. Check to see
                                // if it was the last character we just checked
                                // implying this is part of a multi-digit number
                                if (i == previousNumberIndex + 1)
                                {
                                    numberString += token[i];
                                    previousNumberIndex = i;
                                }
                                else
                                {
                                    // Previous character was NOT a number
                                    // Return our error status (0)
                                    return 0;
                                }
                            }
                        }

                    }
                }

                if (numberString != "")
                {
                    // Will ensure number is converted to a valid int, or stays at
                    // the default value of -1 if numberString is invalid or out of range
                    int.TryParse(numberString, out number);
                    return number;
                }
                else
                    return 0;
            }

            int[] tokensToCoordinates(string[] tokens)
            {
                // A valid co-ordinate pair is made up of a single character (A-Z)
                // and an int in the range of 0-26. A Game will recognise an
                // implimentation-specific subset of these as valid co-ordinates;
                // at this point we just want to make sure the user has input a
                // valid co-ordinate pair.

                // Note: co-ordinate pairs may be made up of two seperate tokens.
                // As such, iterate through the array of tokens until a valid
                // co-ordinate pair has been built. If an error is ever encountered
                // return null.

                int[] returnCoordinate = new int[2];

                bool gotValidAlphaCoordinate = false;
                bool gotValidNumericalCoordinate = false;

                char chosenAlphaCoordinate = '\0';
                int chosenNumericalCoordinate = 0;

                foreach (string token in tokens)
                {
                    // Iterate over the string and check for valid alpha and
                    // numerical co-ordinates.

                    char alphaCoordinate = tokenToAlphaCoordinate(token);
                    int numericalCoordinate = tokenToNumericalCoordinate(token);

                    if (alphaCoordinate != '\0')
                    {
                        if (gotValidAlphaCoordinate == false)
                        {
                            // We don't yet have a valid alpha co-ordinate
                            gotValidAlphaCoordinate = true;
                            chosenAlphaCoordinate = alphaCoordinate;
                        }
                        else
                        {
                            // If we find a SECOND valid alpha co-ordinate,
                            // return null. This will be interpreted as invalid input
                            chosenAlphaCoordinate = '\0';
                        }
                    }

                    if (numericalCoordinate > 0)
                    {
                        if (gotValidNumericalCoordinate == false)
                        {
                            // We don't yet have a valid numerical co-ordinate
                            gotValidNumericalCoordinate = true;
                            chosenNumericalCoordinate = numericalCoordinate;
                        }
                        else
                        {
                            // If we find a SECOND valid numerical co-ordinate,
                            // return null. This will be interpreted as invalid input
                            chosenNumericalCoordinate = 0;
                        }
                    }
                }

                // We have a valid alpha co-ordinate and a valid numerical
                // co-ordinate. Convert to an [x,y] co-ordinate pair and return,
                // or return null if we have any of the defaults, indicating
                // invalid input was found
                int x = alphaCharacters.IndexOf(chosenAlphaCoordinate);
                int y = chosenNumericalCoordinate - 1;

                returnCoordinate[0] = x;
                returnCoordinate[1] = y;

                if (x < 0 || y < 0)
                    return null;
                else
                    return returnCoordinate;
            }

            // Check for the keyword "to" in the input
            for (int i = 0; i < tokens.Length; i++)
            {
                // If we do, find the location
                if (tokens[i] == "TO")
                    toIndex = i;
            }

            // Now, we have to extract source and destination co-ordinates out of
            // the tokenised user input. If there is a 'to' verb, assume
            // whatever is on the left is the source and whatever is on the right
            // is a destination. If there is no 'to' statement, assume it is
            // all destination.

            // Split source and destination tokens around 'to' verb
            destinationTokens = new string[tokens.Length - (toIndex)];
            Array.Copy(tokens, toIndex, destinationTokens, 0, destinationTokens.Length);

            sourceTokens = new string[toIndex];
            Array.Copy(tokens, 0, sourceTokens, 0, sourceTokens.Length);

            // Pull a valid co-ordinate pair from each set of tokens. Note:
            // the function will return null if no valid co-ordinates can be found
            sourceCoordinates = tokensToCoordinates(sourceTokens);
            destinationCoordinates = tokensToCoordinates(destinationTokens);
            piece = null;

            // If neither source co-ordinate or destination co-ordinates found,
            // there was no valid user input
            if (sourceCoordinates == null && destinationCoordinates == null)
                return false;

            // If we have source co-ordinates but no destination co-ordinates,
            // the user input was malformed. Return null
            if (sourceCoordinates != null && destinationCoordinates == null)
                return false;

            return true;
        }
        public Human() : base() { }


        public Human(string name) : base(name) { }


        public override Agent nextMove(Game game)
        {
            string _inputError = " is not a valid input. Please see the help for valid input";
            string _moveError = " is not a valid move in this game. Please see the rules for valid moves";

            Console.Write(name + "\'s Move: ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "HELP")
            {
                Console.Write(game.humanReadableValidMoves);
                return this.nextMove(game);
            }

            if (input.ToUpper() == "RULES")
            {
                Console.Write(game.humanReadableRules);
                return this.nextMove(game);
            }

            if (input.ToUpper() == "UNDO")
            {
                // Cannot undo the first move, or if the first
                // move was done by a computer player
                if ((game.initialGameState.currentPlayer.GetType() != this.GetType() &&
                    game.gameStates.IndexOf(game.currentGameState) == 1) ||
                    !game.undoGameState())
                {
                    Console.WriteLine("Cannot undo the first move");
                    return this.nextMove(game);
                }

                while (game.currentPlayer.GetType() != this.GetType())
                    // We probably undid to a computer player. Undo
                    // some more until we get back to a Human player
                    if ((game.initialGameState.currentPlayer.GetType() != this.GetType() &&
                         game.gameStates.IndexOf(game.currentGameState) == 1) ||
                         !game.undoGameState())
                        // Cannot undo the first move, or if the first
                        // move was done by a computer player
                        break;

                return game.NextPlayer();
            }

            if (input.ToUpper() == "REDO")
            {
                if (!game.redoGameState())
                {
                    Console.WriteLine("No moves to redo");
                    return this.nextMove(game);
                }
                return game.NextPlayer();
            }

            if (input.ToUpper() == "SAVE")
            {
                game.io.Save();
                return this.nextMove(game);
            }

            if (input.ToUpper() == "LOAD")
            {
                Game loadedGame = null;

                try
                {
                    while (loadedGame == null)
                        loadedGame = game.io.Load();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return game.NextPlayer();
                }

                game.players = loadedGame.players;
                game.gameStates = loadedGame.gameStates;
                game.initialGameState = loadedGame.initialGameState;
                game.currentGameState = loadedGame.currentGameState;
                return game.NextPlayer();

            }

            if (input.ToUpper() == "QUIT" || input.ToUpper() == "END"
                || input.ToUpper() == "EXIT")
            {
                return null;
            }

            // Parse Input. Note that this is moderately generic and can
            // cope with some natural language:
            // TODO: add logic to detect the use of 'a' in natural language
            // and exclude it from being grabbed as a co-ordinate
            // TODO: deal with multi-piece games by allowing the selection
            // of a piece by name

            bool validInput = parseInput(input, out int[] sourceCoordinates,
                                                out int[] destinationCoordinates,
                                                out Piece piece);

            if (!validInput)
            {
                Console.WriteLine("\'" + input + "\'" + _inputError);
                return this.nextMove(game);
            }

            // If no piece specific by user input, simply choose the first piece
            // in the list of pieces

            if (piece == null)
                piece = this.pieces[0];

            try
            {
                game.move(sourceCoordinates, destinationCoordinates, piece, this);
            }
            catch (InvalidMoveException e)
            {
                Console.WriteLine("\'" + input + "\'" + _moveError);
                Console.WriteLine(e.Message);
                return this.nextMove(game);
            }

            return game.NextPlayer();
        }
    }

    public class Computer : Agent
    {
        public Computer(string name) : base(name) { }

        public Computer() : base() { }
    }

    public class GameIO
    {
        private static string _pathToFileSaveDialogue = "File name and path: ";
        private static string _succesfulFileSaveDialogue = "File Saved";

        private static string _previousLoadDialogue = "A previously saved game has been found.\n" +
                                                      "Press Return to load this file, or enter a\n" +
                                                      "different path from which to load a\n" +
                                                      "file (\'.sav\' will be appended automatically): ";

        private static string _pathToFileLoadDialogue = "File to load (\'.sav\' will be appended automatically): ";

        private static string _cancelLoadDialogue = "Type \'cancel\' at any time to cancel game loading";

        private static string _fileExtension = ".sav";

        private static string _overwriteErrorDialogue = "File Exists. Overwrite? ";
        private static string _invalidDirectoryError = "Error: Directory Not Found";
        private static string _invalidFileError = "Error: File not Found";
        private static string _incorrectFileTypeError = "Error: File does not appear to be a saved game";

        private string _filePath;

        private Type[] _gameTypes;
        private Game _game;

        public GameIO() { }

        public GameIO(Game game)
        {
            _gameTypes = Assembly.GetExecutingAssembly().GetTypes();

            static bool exclusions(Type type)
            {
                // Prevent certain types being passed to the XML serialiser
                // when saving the Game
                Type[] excludedTypes = { typeof(InvalidMoveException) };

                foreach(Type excludedType in excludedTypes)
                {
                    // Exclude auto generated types and those in the
                    // exclusedTypes list
                    if(type == excludedType ||
                       type.ToString().Contains('<') ||
                       type.ToString().Contains('>'))
                        return false;
                }
                return true;
            }
            _gameTypes = Array.FindAll(_gameTypes, exclusions);
            _game = game;
        }

        // Inspired by https://stackoverflow.com/a/14663848 but modified to accept
        // an array of valid Types built from the current assembly
        public void Save()
        {
            Console.Write(_pathToFileSaveDialogue);
            _filePath = Console.ReadLine();

            _filePath += _fileExtension; // Append file extensions

            if (File.Exists(_filePath))
            {
                Console.Write(_overwriteErrorDialogue);
                string input = Console.ReadLine();
                if (input.ToUpper() == "NO" || input.ToUpper() == "N")
                    return;
            }

            try
            {
                using (var writer = new System.IO.StreamWriter(_filePath))
                {
                    var serializer = new XmlSerializer(typeof(Game), _gameTypes);
                    serializer.Serialize(writer, _game);
                    writer.Flush();
                    Console.WriteLine(_succesfulFileSaveDialogue);
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine(_invalidDirectoryError);
                _filePath = null;
                return;
            }
        }

        public Game Load()
        {
            string input = "";

            Console.WriteLine(_cancelLoadDialogue);

            if (_filePath != null)
            {
                Console.Write(_previousLoadDialogue);
                input = Console.ReadLine();
            }

            if (input == "" && _filePath != null)
            {
                Console.WriteLine("Loading " + _filePath);
            }
            else
            {
                while (_filePath == null || _filePath == "")
                {
                    Console.Write(_pathToFileLoadDialogue);
                    _filePath = Console.ReadLine();
                }
                _filePath += _fileExtension;
            }

            // TODO: create a custom exception here
            if(_filePath.ToUpper() == "CANCEL" || _filePath.ToUpper() == "CANCEL.SAV")
                throw new ArgumentOutOfRangeException();

            try
            {
                using (var stream = System.IO.File.OpenRead(_filePath))
                {
                    var serializer = new XmlSerializer(typeof(Game), _gameTypes);
                    return serializer.Deserialize(stream) as Game;
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine(_invalidDirectoryError);
                _filePath = null;
                return null;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(_invalidFileError);
                _filePath = null;
                return null;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine(_incorrectFileTypeError);
                _filePath = null;
                return null;
            }

            }
    }
}