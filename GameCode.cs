
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace BullAndCow
{
    // "SecretCode" class represents the code the user has to guess
    class SecretCode
    {
        // Array of digits length 3-10 depending on user preference;
        // Number user is guessing
        private int[] _target;

        // Array to aid in the shuffling of the _target array
        private int[] _shuffleArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Array to eliminate any duplicates in the player's guess
        private int[] _checkArray;

        // Variable to hold the bull count (same num. same pos.)
        private int _bull;

        // Variable to hold the cow count (matching num. somewhere)
        private int _cow;

        //  Returns the target as a string
        public string Target
        { get { return String.Join("", new List<int>(_target).ConvertAll(i => i.ToString()).ToArray()); } }

        //  Returns the num. digits in target, should be the same as user entry for num. digits
        public int Length { get { return _target.Length; } }

        public int Bulls { get { return _bull; } }
        public int Cows { get { return _cow; } }

        public SecretCode(int numDigits)
        {
            if (numDigits < 3 || numDigits > 10)
            {
                throw new ArgumentException("Number of digits out of range.");
            }
            else
            {
                // Testing
                //Console.WriteLine("DEBUG: Secret constructor called successfully. ");

                // Creating a new rand object
                Random rand = new Random();

                // Testing 
                //Console.WriteLine("DEBUG: Unshuffled");

                /* Testing
                foreach (int i in _shuffleArray)
                {
                    Console.WriteLine($"DEBUG: [{i}] {i} ");
                } */

                _shuffleArray = _shuffleArray.OrderBy(x => rand.Next()).ToArray();

                _target = new int[numDigits];

                // Copy it to the array we are focusing on with the proper size 
                Array.Copy(_shuffleArray, _target, numDigits);
            }
            _bull = 0;
            _cow = 0;
        }

        // Checks the guess array against the target array 
        public void CheckGuess(int[] _guess)
        {
            // Reset the count every turn 
            _bull = 0;
            _cow = 0;

            // Use another array to eliminate any duplicates in the player's guess
            _checkArray = _guess.Distinct().ToArray();

            for (int i = 0; i < _target.Length; i++)
            {
                // If the positions & values of the numbers match, add one to bull
                if (_guess[i] == _target[i])
                {
                    _bull++;
                }
                else
                {
                    // If they don't, check that guess' number against all numbers in target array 
                    for (int j = 0; j < _checkArray.Length; j++)
                    {
                        // If the guess num. matches, add one to cow
                        if (_checkArray[j] == _target[i])
                        {
                            _cow++;
                        }
                    }
                }
            }
            Console.WriteLine($"Bulls: {_bull}, Cows: {_cow}\n");
        }
    }

    // "HighScoreList" responsible for reading and writing the high score list 
    class HighScoreList
    {
        // Values from the high score file, list of int values
        private int[] _scores = new int[8];

        public string HighScoreTable
        {
            get
            {
                return @$"Digits : Best Score
     3 : {_scores[0]}
     4 : {_scores[1]}
     5 : {_scores[2]}
     6 : {_scores[3]}
     7 : {_scores[4]}
     8 : {_scores[5]}
     9 : {_scores[6]}
    10 : {_scores[7]}
                ";
            }
        }

        public HighScoreList()
        {
            Load();
        }

        // Writes the list of high scores to the text file
        private void Save()
        {
            string filename = @"C:\data\highscore.txt";

            // Testing statement 
            //Console.Write($"DEBUG: Save Called \n");

            try
            {
                using StreamWriter writer = new StreamWriter(filename);
                for (int i = 0; i < 8; i++)
                {
                    writer.WriteLine(_scores[i]);
                }
            }
            catch (Exception)
            {
                //Console.WriteLine($"DEBUG: Save Exeption:  {e.Message} \n");
                Console.WriteLine("Error: Problem with file. You probably forgot to create the folder.");

            }
        }

        //  Reads the contents of the file C:\data\highscore.txt into the _scores properties
        private void Load()
        {
            string filename = @"C:\data\highscore.txt";

            // Testing
            //Console.WriteLine("DEBUG: Load called.");

            try
            {
                using StreamReader reader = new StreamReader(filename);

                // While there is data in the file,
                // assign to fileArray string array, then to scores int array
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    string[] fileArray = File.ReadAllLines(filename);
                    _scores = Array.ConvertAll(fileArray, _scores => int.Parse(_scores));
                }

                /* Testing
                Console.WriteLine("Score File Values: ");
                for (int i = 0; i < 8; i++)
                {
                    Console.WriteLine($"DEBUG: {_scores[i]}");
                }
                Console.WriteLine(" ");
                */

                // If file is empty, it will initialize the score board with 1000000
                if (new FileInfo(filename).Length == 0)
                {
                    // Testing
                    //Console.WriteLine("Empty file, initializing scores to max...");

                    for (int i = 0; i < 8; i++)
                    {
                        _scores[i] = 1000000;
                    }
                }
            }

            catch (Exception)
            {
                //Console.WriteLine($"DEBUG: Load Exception: {q.Message}");

                // Any other problems, the program resets everything 
                // Console.WriteLine("Creating a new High Score table...");
                for (int i = 0; i < 8; i++)
                {
                    _scores[i] = 1000000;
                }
            }
        }

        public void UpdateHighScoreForDigit(int digits, int numGuesses)
        {
            try
            {
                // If numGuesses less than the score value at the digit count 
                if (numGuesses < _scores[digits - 3])
                {
                    // Override the current score in that spot
                    _scores[digits - 3] = numGuesses;

                    // Fun message for a new high score, doubles as a testing statement...
                    Console.WriteLine($"NEW HIGH SCORE!\n");

                    Save();
                }
            }
            catch (Exception)
            {
                // Testing
                //Console.WriteLine($"DEBUG: Exception:  {w}");
            }
        }
    }

    // Class that manages a Bull and Cow game and contains the logic for the game. 
    class Game
    {
        // Secret code the user is supposed to guess
        private SecretCode _secret;

        // List of high scores, calls Load at the start
        private HighScoreList _highScoreList;

        // Secret code for testing purposes, appears only if true
        private bool _doCheat = false;

        // Array to hold the user's guess 
        private int[] _guess;

        // Variable to hold the user's input guesses
        private int _userGuessInput;

        // If the loop should continue 
        private bool _loop = true;

        // Bool to replay the game 
        private bool _stillPlaying = true;

        // How many tries it took the user
        private int _attemptCount = 0;

        public Game()
        {
            _secret = null;
            _doCheat = false;
            _highScoreList = new HighScoreList();
        }

        //  Prompts the players for the num. digits secret should have
        public void CreateSecret()
        {
            while (_loop)
            {
                try
                {
                    Console.Write($"How many digits for this game (3 - 10)? ");
                    int numDigits = Int32.Parse(Console.ReadLine());

                    // Testing
                    //Console.WriteLine($"Debug: { numDigits }");

                    // Setting _doCheat to t\f depending on answer
                    if (numDigits >= 3 && numDigits <= 10)
                    {
                        // Creates the a SecretCode object and assign it to _secret
                        _secret = new SecretCode(numDigits);

                        // Setting the size of the guess array to the number of digits input 
                        _guess = new int[numDigits];

                        // Loop stops 
                        _loop = false;
                    }
                    else
                    {
                        Console.WriteLine($"Out of range. Choose between 1-10. Try Again.\n");
                    }
                }
                catch (Exception)
                {
                    // Testing
                    //Console.WriteLine($"DEBUG: Create secret exception: {m}");

                    Console.WriteLine($"Invalid Entry. Try Again.\n");
                }
            }
        }

        public void SetupCheat()
        {
            _loop = true;
            while (_loop)
            {
                Console.Write($"\nDo you want to cheat (Y/N)? ");
                string cheatResponse = Console.ReadLine();

                // Setting _doCheat bool to t\f depending on answer
                if (cheatResponse == "Y" || cheatResponse == "y")
                {
                    _doCheat = true;
                    _loop = false;
                }
                else if (cheatResponse == "N" || cheatResponse == "n")
                {
                    _doCheat = false;
                    _loop = false;
                }
            }
        }

        public void Play()
        {
            // Programmer reminder to call CreateSecret() before SetupCheat() in Program.cs
            if (_secret == null)
            {
                throw new NullReferenceException("Fields not initialized");
            }

            while (_stillPlaying)
            {
                // If the user chose to cheat, they get to see the secret every round 
                if (_doCheat == true)
                {
                    // Hiding the actual numbers from the user by formatting to a string 
                    Console.WriteLine($"\nSecret = {_secret.Target} (Cheater Cheater Pumpkin Eater...)");
                }

                // Propts for guess entries depending on how long user's chosen secret was
                Console.WriteLine("Enter guess:");
                for (int i = 0; i < _secret.Length; i++)
                {
                    Console.Write($"Spot #{i + 1}: ");
                    try
                    {
                        _userGuessInput = Int32.Parse(Console.ReadLine());

                        // If the user entered a valid value, assign it to the array,
                        // otherwise give it a value that could never count towards their score 
                        if (_userGuessInput >= 0 && _userGuessInput <= 9)
                        {
                            // Assigning the guess to the guess array 
                            _guess[i] = _userGuessInput;

                            // Testing
                            //Console.WriteLine($"DEBUG: Guess Array Pos. [{i}] : {_guess[i]}\n");
                        }
                        else
                        {
                            _guess[i] = -2;
                            // Testing
                            //Console.WriteLine($"DEBUG: Guess Array Pos. [{i}] : {_guess[i]}\n");
                        }
                    }
                    catch (Exception)
                    {
                        // Testing
                        //Console.WriteLine($"DEBUG: Crazy input confirmed.");
                        _guess[i] = -2;

                        // Testing
                        //Console.WriteLine($"DEBUG: Guess Array Pos. {i} : {_guess[i]}\n");
                    }
                }

                // If they didn't win yet, their score goes up by one
                _attemptCount++;

                // Check the whole guess array with the method CheckGuess
                _secret.CheckGuess(_guess);

                // If the bull count is the same as the secret code's length, the game is over.
                if (_secret.Bulls == _secret.Length)
                {
                    // Winning statement 
                    Console.WriteLine($"It took you { _attemptCount} guess(es)!\n");

                    // No longer playing/guessing, so set to false
                    _stillPlaying = false;

                    // If their score is better than the current one in the file, update
                    _highScoreList.UpdateHighScoreForDigit(_secret.Length, _attemptCount);

                    // Show full table, with pretty, dazzly effects
                    ShowHighScores();
                }
            }
        }

        // Outputs the high scores with a fancy banner
        public void ShowHighScores()
        {
            Console.WriteLine($"*******************\n*** High Scores ***\n*******************\n");
            Console.WriteLine($"{_highScoreList.HighScoreTable}");
        }
    }
}
