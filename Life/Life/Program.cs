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
                    {//If the element/argument in the arrayAll contain -- (thus has an option) then add the numParaSinceOption to the list of ints
                        listOfInts.Add(numParaSinceOption);
                        indexOption.Add(i);
                        numParaSinceOption = 0;
                    }
                    else
                    {
                        //Increment the numParaSinceOption until there is another option or until for loop reach it limit
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

            PrintProgramSetting(rows, columns, periodic, stepMode, randomFactor, generations, inputFile, maxUpdateRate);

            Grid grid = new Grid(rows, columns);
            Stopwatch watch = new Stopwatch();
            CellState[][] cellStateArray = new CellState[rows][];



            // Wait for user to press a key...
            Console.WriteLine("Press spacebar to start...");
            while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;

            // Initialize the grid window (this will resize the window and buffer)
            grid.InitializeWindow();

            // Set the footnote (appears in the bottom left of the screen).
            grid.SetFootnote(0.ToString());

            //Initialise the cellStateArray
            InitialiseCellStateJaggedArray(cellStateArray);

                for (int i = 0; i < generations; i++)
                {

                    watch.Restart();
                    grid.SetFootnote((i).ToString()); //Display the footnote as the current generation
                    for (int row = 0; row < rows; row++)
                    {
                    for (int column = 0; column < columns; column++)
                    {
                        if (i == 0 && inputFile == "N/A") //i = 0 mean that it is the first generation
                        {
                            //Random choice on whether the cell is alive or dead
                            int randomChoice = RandomFactorProbability(randomFactor);

                            if (randomChoice == 1)
                            {//If randomChoice is 1 mean that the cell will be alive
                                grid.UpdateCell(row, column, CellState.Full);
                            }
                            else
                            {
                                grid.UpdateCell(row, column, CellState.Blank);
                            }
                            grid.Render();
                        }

                        else if (i == 0 && inputFile != "N/A")
                        {
                            //Update the grid using the input file row and column and state     
                            grid.UpdateCell(2, 0, CellState.Full);
                            grid.UpdateCell(1, 1, CellState.Full);
                            grid.UpdateCell(1, 2, CellState.Full);
                            grid.UpdateCell(2, 2, CellState.Full);
                            grid.UpdateCell(3, 2, CellState.Full);

                            grid.Render();

                            



                        } 
                        else 
                        {
                            Cell cell = grid.GetCell(row, column);
                            //Add all adjacent cells into a list
                            //grid.GetAdjacentCells(row, column);

                            //GridDimensions gridDimenions = new GridDimensions();

                            if (periodic)
                            {

                                List<Cell> adjacent = grid.GetAdjacentCells(row, column, 1);

                                //Calculate whether the current cell will become dead or alive in the next generation
                                CellState state = cell.Calculate(adjacent, cell);

                                cellStateArray[row][column] = state;
                            }
                            else
                            {
                                List<Cell> adjacent = grid.GetAdjacentCells(row, column, 0);

                                //Calculate whether the current cell will become dead or alive in the next generation
                                CellState state = cell.Calculate(adjacent, cell);

                                cellStateArray[row][column] = state;
                            }
                        }


                        }

                    }


                    if (i > 0)
                    { /*The below for loop can only occur after the first generation since the adjacent cells can only be calculated
                once there are cells that has been generated already */
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

                    if (stepMode)
                    {//If stepmode is enable then the user will have to press spacebar before it can go onto the next generation
                        while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;

                    }
                    else
                    {
                        while (watch.ElapsedMilliseconds < 1000 / maxUpdateRate) ;
                    }

                }
                grid.SetFootnote(generations.ToString());


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

        

        public static void InitialiseCellStateJaggedArray(CellState [][] cellStateArray)
        {
            for (int row = 0; row < rows; row++)
            {
                cellStateArray[row] = new CellState[columns];

            }
        }

        //Determine which optional argument the user has inputted in and verify whether the user input the correct parameter for the option  

        public static int RandomFactorProbability(float randomFactor)
        {
            var random = new Random();
            List<int> probabilityList = new List<int>();
            float tempNum = randomFactor * 100;
            int aliveNum = 1;
            for (int i = 0; i < tempNum; i++)
            {//Alive number cannot exceed the max number of alive cells that can exist
                probabilityList.Add(aliveNum);
            }

            float remainingNum = 100 - tempNum;
            int deadNum = 0;
            for (int i = 0; i < remainingNum; i++)
            {
                probabilityList.Add(deadNum);
            }

            //Get a random index number from the probabilityList
            int randomIndex = random.Next(probabilityList.Count); 

            //Get the value at a particular index in the probabilityList. Value should only be either 0 or 1
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

            if (arr[startingIndex] == "--dimensions") //If user enter --dimensions
            {
                if (numOfParam == 2)
                {
                    int tempRow = int.Parse(arr[startingIndex + 1]);
                    int tempColumn = int.Parse(arr[startingIndex + 2]);
                    if (tempRow >= 4 && tempRow <= 48 && tempColumn >= 4 && tempColumn <= 48) 
                    {//Check if the number of rows and columns is in between 4 and 48
                        rows = int.Parse(arr[startingIndex + 1]);
                        columns = int.Parse(arr[startingIndex + 2]);
                        return true;
                    }
                    else
                    {

                        Console.WriteLine("Dimension is out of range. " +
                            "Rows and columns must be a positive integer between 4 - 48 (inclusive) ");

                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Dimensions has two parameters:" +
                        " --dimensions <rows> <columns> which is a positive integer between 4 - 48 (inclusive)");

                    return false;
                }
            }
            else if (arr[startingIndex] == "--periodic" && numOfParam == 0)
            {
                periodic = true;
                return true;
            }
            else if (arr[startingIndex] == "--step")
            {
                if (numOfParam == 0)
                {
                    stepMode = true;
                    return true;
                }
                else
                {
                    Console.WriteLine("--step takes no parameters but was given one");

                    return false;
                }
            }

            else if (arr[startingIndex] == "--random")
            {
                if (numOfParam == 1)
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
                        return true;
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
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Refresh rate: ", maxUpdateRate)); /// ADD
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
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Random Factor: ", (int)(randomFactor * 100) + "%"));
            if (stepMode)
            {
                Console.WriteLine(String.Format("{0,27}{1, -26}", "Step Mode: ", "Yes"));
            }
            else
            {
                Console.WriteLine(String.Format("{0,27}{1, -26}\n", "Step Mode: ", "No"));
            }
        }

        public static void ReadInputFile()
        {

            using (StreamReader file = new StreamReader(inputFile))
            {
                List<int> rowList = new List<int>();
                List<int> columnList = new List<int>();

                string line;
                string[] fields;
                const char DELIM = ' ';
                int row;
                int column;
                file.ReadLine();
                while ((line = file.ReadLine()) != null)
                {
                    fields = line.Split(DELIM);

                    row = Convert.ToInt32(fields[0]);

                    rowList.Add(row);

                    column = Convert.ToInt32(fields[1]);

                    columnList.Add(column);
                }

                int[][] location = new int[rowList.Count][];

                for (int i = 0; i < rowList.Count; i++)
                {
                    location[i] = new int[2];
                }

                for (int i = 0; i < location.Length; i++)
                {
                    location[i][0] = rowList[i];
                    location[i][1] = columnList[i];
                }

                for (int i = 0; i < location.Length; i++)
                {
                    Console.WriteLine("Row: {0}\nColumn: {1}", location[i][0], location[i][1]);
                }
            }


        }
    }
}
