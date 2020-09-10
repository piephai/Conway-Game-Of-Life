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
        //GridDimensions gridDimensions = new GridDimensions();
            string[] arrayAll = new string[args.Length];
            var listOfInts = new List<int>();
            var indexOption = new List<int>();
            bool correctInput = true;

            //if (args.Length == 0)
            //{
            //    Console.WriteLine("WARNING: No command line arguments provided.");
            //    return;
            //}
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
                        Console.WriteLine("WARNING: Parameters must be provided after options.");
                        return;
                    }
                }

                int numParaSinceOption = 0;
                indexOption.Add(0);
                for (int i = 1; i < arrayAll.Length; i++)
                {
                    if (arrayAll[i].Contains("--"))
                    {
                        listOfInts.Add(numParaSinceOption);
                        indexOption.Add(i);
                        numParaSinceOption = 0;
                    }
                    else
                    {

                        numParaSinceOption += 1;
                    }
                }
                listOfInts.Add(numParaSinceOption);
                for (int i = 0; i < listOfInts.ToArray().Length; i++)
                {

                    correctInput = OptionArgument(arrayAll, indexOption.ToArray()[i], listOfInts.ToArray()[i]);
                    if (!correctInput)
                    {
                        //Console.WriteLine("Incorrect input");
                        //Console.ReadKey();
                        return;
                    }
                }
            }

            PrintProgramSetting(rows, columns, periodic, stepMode, randomFactor, generations, inputFile, maxUpdateRate);



            // The following is just an example of how to use the Display.Grid class
            // Think of it as a "Hello World" program for using this small API
            // If it works correctly, it should display a little smiley face cell
            // by cell. The program will end after you press any key.

            // Feel free to remove or modify it when you are comfortable with it.

            // Specify grid dimensions and active cells...
            
            
            int[,] cells = {
                { 5, 3 },
                { 4, 3 },
                { 5, 5 },
                { 4, 5 },
                { 2, 1 },
                { 1, 2 },
                { 1, 3 },
                { 1, 4 },
                { 1, 5 },
                { 1, 6 },
                { 2, 7 }
            };

            // Construct grid...
            Grid grid = new Grid(rows, columns);

            // Wait for user to press a key...
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            // Initialize the grid window (this will resize the window and buffer)
            grid.InitializeWindow();

            // Set the footnote (appears in the bottom left of the screen).
            grid.SetFootnote("Smiley");

            Stopwatch watch = new Stopwatch();

            // For each of the cells...
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                watch.Restart();

              
               

                // Update grid with a new cell...
                grid.UpdateCell(cells[i, 0], cells[i, 1], CellState.Full);

                // Render updates to the console window...
                grid.Render();
                
                
                // Update grid with a new cell...
                grid.UpdateCell(cells[i, 0], cells[i, 1], CellState.Full);

                // Render updates to the console window...
                grid.Render();
                if (stepMode)
                {
                    while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
                }
                else
                {
                    while (watch.ElapsedMilliseconds < 1000 / maxUpdateRate) ;
                }

            }

            // Set complete marker as true
            grid.IsComplete = true;

            // Render updates to the console window (grid should now display COMPLETE)...
            grid.Render();

            // Wait for user to press a key...
            Console.ReadKey();

            // Revert grid window size and buffer to normal
            grid.RevertWindow();
        }

        //Determine which optional argument the user has inputted in and verify whether the user input the correct parameter for the option  
        public static bool OptionArgument(string[] arr, int startingIndex, int numOfParam)
        {

            if (arr[startingIndex] == "--dimensions")
            {
                if (numOfParam == 2)
                {
                    int tempRow = int.Parse(arr[startingIndex + 1]);
                    int tempColumn = int.Parse(arr[startingIndex + 2]);
                    if (tempRow >= 4 && tempRow <= 48 && tempColumn >= 4 && tempColumn <= 48)
                    {
                        rows = int.Parse(arr[startingIndex + 1]);
                        columns = int.Parse(arr[startingIndex + 2]);
                        return true;
                    }
                    else
                    {

                        Console.WriteLine("Dimension is out of range. " +
                            "Rows and columns must be a positive integer between 4 - 48 (inclusive) ");
                        Console.ReadKey();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Dimensions has two parameters:" +
                        " --dimensions <rows> <columns> which is a positive integer between 4 - 48 (inclusive)");
                    Console.ReadKey();
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
                    Console.ReadKey();
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
                        Console.ReadKey();
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
                    Console.ReadKey();
                    return false;
                }
            }
            else if (arr[startingIndex] == "--generations")
            {
                if (numOfParam == 1)
                {
                    int tempGeneration = 0;
                    if (int.TryParse(arr[startingIndex], out tempGeneration))
                    {
                        generations = tempGeneration;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Generation value must be a positive non-zero integer");
                        Console.ReadKey();
                        return false; 
                    }
                }
                //else if (numOfParam < 1)
                //{
                //    Console.WriteLine("--generations take one parameter of <number> but none was provided");
                //    Console.ReadKey();
                //    return false;
                //}
                else
                {
                    Console.WriteLine("--generations <number> only take one parameter");
                    Console.ReadKey();
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
                        Console.ReadKey();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("--seed <filename> only take one parameter");
                    Console.ReadKey();
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
                        Console.ReadKey();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("--max-update <update per second> Only take one parameter");
                    Console.ReadKey();
                    return false;
                }

            }
            else
            {
                return false;
            }


            //string result = string.Empty;
            //if (amountToPrint >= 2)
            //{
            //    result += arr[startingIndex] + ": ";
            //    result += "{ ";
            //    result += arr[startingIndex + 1];
            //    for (int i = startingIndex + 2; i <= (startingIndex + amountToPrint); i++)
            //    {
            //        result += ", ";
            //        result += arr[i];

            //    }
            //    result += " }";
            //}
            //else if (amountToPrint > 0 && amountToPrint <= 1)
            //{
            //    result += arr[startingIndex] + ": ";
            //    result += "{ ";
            //    result += arr[startingIndex + 1];
            //    result += " }";
            //}
            //else
            //{
            //    result += arr[startingIndex];
            //}
            //return result;
        }

        public static void PrintProgramSetting(int rows, int columns, bool periodic, bool stepMode, float randomFactor, int generations, string inputFile, float maxUpdateRate)
        {

            Console.WriteLine(String.Format("\nThe program will use the following settings:\n"));
            int wordPosition = inputFile.Length + 10;

            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Input File: ", inputFile) );
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Generations: ", generations));
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Refresh rate: ", maxUpdateRate )); /// ADD
            if (periodic)
            {
                Console.WriteLine(String.Format("{0, 27}{1, -26}", "Periodic: ", "Yes"));
            }
            else
            {
                Console.WriteLine(String.Format("{0, 27}{1, -26}", "Periodic: ", "No"));
            }
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Rows: " ,rows));
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Columns: ", columns));
            Console.WriteLine(String.Format("{0, 27}{1, -26}", "Random Factor: ", randomFactor + "%"));
            if (stepMode)
            {
                Console.WriteLine(String.Format("{0,27}{1, -26}", "Step Mode: ", "Yes"));
            }
            else
            {
                Console.WriteLine(String.Format("{0,27}{1, -26}\n", "Step Mode: ", "No"));
            }
            

        }



        
    }
}
