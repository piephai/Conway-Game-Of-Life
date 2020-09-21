using System;
using System.Diagnostics;
using Display;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;

namespace Life
{
    class Program
    {
        private static int rows = 16;
        private static int columns = 16;
        private static bool periodic = false;
        private static bool stepMode = false;
        private static float randomFactor = 0.5f;
        private static int generations = 50;
        private static string inputFile = "N/A";
        private static float maxUpdateRate = 5f;
        
        static void Main(string[] args)
        {
            //Forward Declarations
            string[] arrayAll = new string[args.Length];
            var listOfInts = new List<int>();
            var indexOption = new List<int>();
            bool correctInput = true;

            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: No command line arguments provided, reverting to defaults.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (args.Length > 0)
            {
                for (int i = 0; i < arrayAll.Length; i++)
                {
                    if (args[0].Contains("--"))
                    {
                        arrayAll[i] = args[i];
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("WARNING: Parameters must be provided after options, reverting to defaults");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                int numParaSinceOption = 0;
                indexOption.Add(0);

                for (int i = 1; i < arrayAll.Length; i++)
                {
                    if (arrayAll[i].Contains("--"))
                    {// If the element/argument in the arrayAll contain -- (thus has an option) then add the numParaSinceOption to the list of ints
                        listOfInts.Add(numParaSinceOption);
                        indexOption.Add(i);
                        numParaSinceOption = 0;
                    }
                    else
                    {
                        // Increment the numParaSinceOption until there is another option or until for loop reach it limit
                        numParaSinceOption += 1;
                    }
                }
                listOfInts.Add(numParaSinceOption);

                for (int i = 0; i < listOfInts.ToArray().Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    correctInput = OptionArgument(arrayAll, indexOption.ToArray()[i], listOfInts.ToArray()[i]);
                }
                if (!correctInput)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Warning: Issue processing command line arguments, reverting to defaults.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success: command line arguments processed.");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            
            // Calling the PrintProgramSetting to print out the command line arguments
            PrintProgramSetting(rows, columns, periodic, stepMode, randomFactor, generations, inputFile, maxUpdateRate);

            Grid grid = new Grid(rows, columns);
            Stopwatch watch = new Stopwatch();
            CellState[][] cellStateArray = new CellState[rows][];

            // Wait for user to press the space bar key...
            Console.WriteLine("Press spacebar to start...");
            while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;

            // Initialize the grid window (this will resize the window and buffer)
            grid.InitializeWindow();

            // Set the footnote (appears in the bottom left of the screen).
            grid.SetFootnote(0.ToString());

            //Initialise the cellStateArray
            InitialiseCellStateJaggedArray(cellStateArray);

                for (int i = 0; i <= generations; i++)
                {
                    watch.Restart();
                    grid.SetFootnote("Iteration:    " + (i).ToString()); // Set footnote to current generation
                    for (int row = 0; row < rows; row++)
                    {
                    for (int column = 0; column < columns; column++)
                    {   
                        // Checking if it is the first generation and a seed file is not provided
                        if (i == 0 && inputFile == "N/A") //i = 0 mean that it is the first generation
                        {
                            //Random choice on whether the cell is alive or dead
                            int randomChoice = RandomFactorProbability(randomFactor);

                            if (randomChoice == 1) // If randomChoice is 1 mean that the cell will be alive
                            { 
                                grid.UpdateCell(row, column, CellState.Full);
                            }
                            else // If randomChoice is not 1, it is 0 so cell state will be dead
                            {
                                grid.UpdateCell(row, column, CellState.Blank); 
                            }
                            grid.Render();
                        }

                        // Checking if it is the first generation and a seed file is provided
                        else if (i == 0 && inputFile != "N/A")
                        {
                            using (StreamReader file = new StreamReader(inputFile))
                            {
                                string line;
                                string[] fields;
                                const char DELIM = ' ';

                                file.ReadLine();
                                
                                // While the file is not empty it will continue to ReadLine
                                while ((line = file.ReadLine()) != null)
                                {
                                    // Seperates the row and column numbers
                                    fields = line.Split(DELIM);

                                    // Update the grid's alive cells using the input file row and column 
                                    grid.UpdateCell(Convert.ToInt32(fields[0]), Convert.ToInt32(fields[1]), CellState.Full);
                                }
                            }
                            grid.Render();
                        } 

                        // For the other generations
                        else 
                        {
                            Cell cell = grid.GetCell(row, column); // Getting the location of the cell

                            // If periodic boundary conditions were set in the command line arguments
                            if (periodic)
                            {
                                //Add all adjacent cells into a list
                                List<Cell> adjacent = grid.GetAdjacentCells(row, column, 1); // 1 means periodic

                                //Calculate whether the current cell will become dead or alive in the next generation
                                CellState state = cell.Calculate(adjacent, cell);

                                cellStateArray[row][column] = state;
                            }

                            // If periodic boundary conditions were not set in the command line arguments
                            else
                            {
                                //Add all adjacent cells into a list
                                List<Cell> adjacent = grid.GetAdjacentCells(row, column, 0); // 0 means not periodic

                                //Calculate whether the current cell will become dead or alive in the next generation
                                CellState state = cell.Calculate(adjacent, cell);

                                cellStateArray[row][column] = state;
                            }
                        }
                    }
                }

                    if (i > 0)
                    { /* The below for loop can only occur after the first generation since the adjacent cells can only
                         be calculated once there are cells that has been generated already */
                        for (int row = 0; row < rows; row++)
                        {
                            for (int column = 0; column < columns; column++)
                            {
                                //Update grid with the new cell state from cellStateArray
                                grid.UpdateCell(row, column, cellStateArray[row][column]);
                                // Render updates to the console window...
                                grid.Render();
                            }
                        }
                    }

                    // If stepmode is enable then the user will have to press spacebar before it can go onto the next generation
                    if (stepMode)
                    {
                        while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
                    }
                    // Otherwise generation will update according to the default or set update rate
                    else
                    {
                        while (watch.ElapsedMilliseconds < 1000 / maxUpdateRate) ;
                    }
                }
                grid.SetFootnote("Iteration: " + generations.ToString());

                // Set complete marker as true
                grid.IsComplete = true;

                // Render updates to the console window (grid should now display COMPLETE)...
                grid.Render();

                // Wait for user to press a key...
                Console.ReadKey();

                // Revert grid window size and buffer to normal
                grid.RevertWindow();
            }

            // The following is just an example of how to use the Display.Grid class
            // Think of it as a "Hello World" program for using this small API
            // If it works correctly, it should display a little smiley face cell
            // by cell. The program will end after you press any key.

            // Feel free to remove or modify it when you are comfortable with it.

            // Specify grid dimensions and active cells...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellStateArray"></param>

        public static void InitialiseCellStateJaggedArray(CellState [][] cellStateArray)
        {
            for (int row = 0; row < rows; row++)
            {
                cellStateArray[row] = new CellState[columns];
            }
        }


        /// <summary>
        /// Sets the random factor probability by creating a list of 100 numbers filled with either a 1 (alive) or
        /// 0 (dead) number, depending on the random factor float. From the list, numbers are randomly chosen and
        /// used to intialise cell state array with dead or alive cells. 
        /// <param name="randomFactor"> A float between 0 and 1 is supplied from the command line arguments </param>
        /// <returns> Either a 1 or 0 to make the cell dead or alive </returns>
        public static int RandomFactorProbability(float randomFactor)
        {
            var random = new Random();
            List<int> probabilityList = new List<int>();
            float tempNum = randomFactor * 100;
            int aliveNum = 1;
            // Adds the 1's to the probability list
            for (int i = 0; i < tempNum; i++) 
            {
                probabilityList.Add(aliveNum);
            }
            float remainingNum = 100 - tempNum; // Subtracts tempNum from 100 to ensure probability list has 100 numbers
            int deadNum = 0;
            // Adds the 0's to the probability list
            for (int i = 0; i < remainingNum; i++)
            {
                probabilityList.Add(deadNum);
            }

            // Get a random index number from the probabilityList
            int randomIndex = random.Next(probabilityList.Count); 

            // Get the value at a particular index in the probabilityList. Value should only be either 0 or 1
            int randomVal = probabilityList[randomIndex]; 
            probabilityList.Clear();
            return randomVal;
        }

        /// <summary>
        /// Updates the state of a cell at specified grid coordinates.
        /// </summary>
        /// <param name="arr">The arr of all arguments.</param>
        /// <param name="startingIndex">The index of where there is an option argument.</param>
        /// <param name="numOfParam">The number of parameters that the user had entered after a particular option.</param>
        public static bool OptionArgument(string[] arr, int startingIndex, int numOfParam)
        {
            // If user enters --dimensions
            if (arr[startingIndex] == "--dimensions")
            {
                if (numOfParam == 2)
                {
                    int tempRow = int.Parse(arr[startingIndex + 1]);
                    int tempColumn = int.Parse(arr[startingIndex + 2]);
                    // Check if the number of rows and columns is in between 4 and 48
                    if (tempRow >= 4 && tempRow <= 48 && tempColumn >= 4 && tempColumn <= 48) 
                    {
                        rows = int.Parse(arr[startingIndex + 1]);
                        columns = int.Parse(arr[startingIndex + 2]);
                        return true;
                    }

                    // If rows and columns are not between 4 and 48, it will show an error message
                    else
                    {
                        Console.WriteLine("Dimension is out of range. " +
                                        "Rows and columns must be a positive integer between 4 - 48 (inclusive) ");
                        return false;
                    }
                }

                // If no parameters are supplied it will provide an error message
                else
                {
                    Console.WriteLine("Dimensions has two parameters:" +
                              " --dimensions <rows> <columns> which is a positive integer between 4 - 48 (inclusive)");
                    return false;
                }
            }

            // If user enters --periodic and there are no parameters, it will enable periodic boundary conditions
            else if (arr[startingIndex] == "--periodic" && numOfParam == 0)
            {
                periodic = true;
                return true;
            }

            // If user enters --step 
            else if (arr[startingIndex] == "--step")
            {
                // If no paramters are provided it will enable step mode
                if (numOfParam == 0)
                {
                    stepMode = true;
                    return true;
                }
                // If parameters are provided it will show an error message
                else
                {
                    Console.WriteLine("--step takes no parameters but was given one");
                    return false;
                }
            }

            // If user enters --random
            else if (arr[startingIndex] == "--random")
            {
                if (numOfParam == 1) // Checking that one parameter is provided
                {
                    float tempRandom = float.Parse(arr[startingIndex + 1]);
                    if (tempRandom <= 1 && tempRandom >= 0) 
                    {
                        randomFactor = tempRandom;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Random factor must be a float between 0 and 1 (inclusive)");
                        return false;
                    }
                }
                else if (numOfParam == 0)
                {
                    Console.WriteLine("Random has one parameter but none was provided: " +
                                    "--random <probability> where probability is a float between 0 and 1 (inclusive)");
                    return false;
                }
                else
                {
                    Console.WriteLine("Random has one parameter but more than one parameter was provided" +
                                    "--random <probability> where probability is a float between 0 and 1 (inclusive)");

                    return false;
                }
            }
            else if (arr[startingIndex] == "--generations")
            {
                if (numOfParam == 1)
                {
                    int tempGeneration = 0;
                    if (int.TryParse(arr[startingIndex + 1], out tempGeneration))
                    {
                        generations = tempGeneration;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Generation value must be a positive non-zero integer");

                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("--generations <number> only take one parameter");

                    return false;
                }
            }

            else if (arr[startingIndex] == "--seed")
            {
                if (numOfParam == 1)
                {
                    if (arr[startingIndex + 1].Contains(".seed"))
                    {
                        inputFile = arr[startingIndex + 1];
                        bool tempChecker = CheckIfSeedDimensionsFits();
                        if (tempChecker)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("The path provided is not in the right .seed format");

                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("--seed <filename> only take one parameter");

                    return false;
                }
            }

            else if (arr[startingIndex] == "--max-update")
            {
                if (numOfParam == 1)
                {
                    float tempMaxUpdateRate = float.Parse(arr[startingIndex + 1]);
                    if (tempMaxUpdateRate >= 1 && tempMaxUpdateRate <= 30)
                    {
                        maxUpdateRate = tempMaxUpdateRate;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Max update rate must be a float between 1 and 30 (inclusive)");

                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("--max-update <update per second> Only take one parameter");

                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        public static void PrintProgramSetting(int rows, int columns, bool periodic, bool stepMode, float randomFactor, int generations, string inputFile, float maxUpdateRate)
        {

            Console.WriteLine(String.Format("\nThe program will use the following settings:\n"));
            int wordPosition = inputFile.Length + 10;

            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Input File: ", inputFile));
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Generations: ", generations));

            //Fix the formatting of the maxUpdateRate
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Refresh rate: ", maxUpdateRate + " updates/s")); /// ADD
            if (periodic)
            {
                Console.WriteLine(String.Format("{0, 27}{1, -26}", "Periodic: ", "Yes"));
            }
            else
            {
                Console.WriteLine(String.Format("{0, 27}{1, -26}", "Periodic: ", "No"));
            }
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Rows: ", rows));
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Columns: ", columns));
            //Need fixing the printed % should be in the 0.00 format (2 decimals or floats)
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Random Factor: ", Math.Round(Convert.ToDecimal(randomFactor * 100.00f), 2) + "%"));
            if (stepMode)
            {
                Console.WriteLine(String.Format("{0,27}{1, -26}", "Step Mode: ", "Yes"));
            }
            else
            {
                Console.WriteLine(String.Format("{0,27}{1, -26}\n", "Step Mode: ", "No"));
            }
        }

        public static bool CheckIfSeedDimensionsFits()
        {
            using (StreamReader file = new StreamReader(inputFile))
            {
                string line;
                string[] fields;
                const char DELIM = ' ';
                List<int> cellRow = new List<int>();
                List<int> cellCol = new List<int>();
                file.ReadLine();
                int maxRow = 0;
                int maxCol = 0;

                while ((line = file.ReadLine()) != null)
                {
                    fields = line.Split(DELIM);

                    //Update the grid using the input file row and column 
                    cellRow.Add(Convert.ToInt32(fields[0]));
                    cellCol.Add(Convert.ToInt32(fields[1]));
                    


                }
                foreach (int item in cellRow )
                {
                    if (item > maxRow)
                    {
                        maxRow = item;
                    }
                }
                foreach (int item in cellCol)
                {
                    if (item > maxCol)
                    {
                        maxCol = item;
                    }
                }

                if (maxRow > rows)
                {
                    Console.WriteLine("Alive cell exists outside of the bound of the universe. The row value should be >= {0} ", maxRow);
                    return false;
                }
                else if (maxCol > columns)
                {
                    Console.WriteLine("Alive cell exists outside of the bound of the universe. The column value should be >= {0}", maxCol);
                    return false;
                }

                else if (maxRow > rows && maxCol > columns)
                {
                    Console.WriteLine("Alive cell exists outside of the bound of the universe. The row and column should be at least {0} {1}", maxRow, maxCol);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

    }
}
