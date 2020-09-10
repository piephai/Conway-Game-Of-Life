using System;
using System.Diagnostics;
using Display;

namespace Life
{
    class Program
    {
        static void Main(string[] args)
        {
            // The following is just an example of how to use the Display.Grid class
            // Think of it as a "Hello World" program for using this small API
            // If it works correctly, it should display a little smiley face cell
            // by cell. The program will end after you press any key.

            // Feel free to remove or modify it when you are comfortable with it.

            // Specify grid dimensions and active cells...
            int rows = 7, columns = 9;
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
                
                while (watch.ElapsedMilliseconds < 200) ;
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
    }
}
