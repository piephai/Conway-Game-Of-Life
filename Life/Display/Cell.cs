using System.Collections.Generic;
using System;

namespace Display
{
    /// <summary>
    /// States for the cells in the grid. Each cell state is associated with a 
    /// different character when rendering cells on the grid.
    /// </summary>
    /// <author>Benjamin Lewis</author>
    /// <date>August 2020</date>
    public enum CellState
    {
        Blank,
        Full,
        Dark,
        Medium,
        Light
    }

    /// <summary>
    /// Grid cell used to store data in the grid.
    /// </summary>
    /// <author>Benjamin Lewis</author>
    /// <date>August 2020</date>
    public class Cell
    {
        private readonly char[] RENDER_CHARACTERS = new char[] {
            '\u0020',   // ' '
            '\u2588',   // '█'
            '\u2593',   // '▓'
            '\u2592',   // '▒'
            '\u2591'    // '░'
        };

        private CellState state;
        
        /// <summary>
        /// Initializes the cell (as blank)
        /// </summary>
        public Cell()
        {
            state = CellState.Blank;
        }

        /// <summary>
        /// Sets the state of the cell
        /// </summary>
        /// <param name="state">The new state of the cell</param>
        public void SetState(CellState state)
        {
            this.state = state;
        }

        //Check if the cell is alive or dead. If alive return true
        public bool IsAlive (Cell cell)
        {
            if (cell.state == CellState.Full) //Check the cell state of the current cell. If it is equal to CellState.Full then the current cell is alive
            {
                return true;
            }
            return false;
            
        }

        public CellState Calculate(List<Cell> adjacent, Cell currentCell)
        {

            // Initialise the list of cells
            List<Cell> aliveCellList = new List<Cell>();
            foreach (Cell surroundingCell in adjacent)
            {
                if (surroundingCell.state == CellState.Full)
                {//If the cell state = full then cell is alive. That cell is then added to the aliveCellList
                    aliveCellList.Add(surroundingCell);
                }
            }
            int count = aliveCellList.Count; 

            if (IsAlive(currentCell)) //Call method isAlive to check whether the current cell is alive or dead
            {
            // Less than two or greater than three is always dead.
                if (count < 2 || count > 3)
                {
                    //return false;
                    return CellState.Blank;
                }
                //Alive cells can only have exactly 2 or 3 live cell neighbours to stay alive 
                else if (count == 2 || count == 3)
                {

                    return CellState.Full;
                    //return true;
                    // For live (OldState = true) cells, they are alive. For dead ones,
                    // they live only if there is exactly three. This uses the OR logic to
                    // to combine the two statements together.

                }
                return CellState.Blank;
            }
            else
            {//Dead cells need exactly 3 alive cells to become alive
                if (count == 3)
                {
                    return CellState.Full;
                }
                else
                {
                    return CellState.Blank;
                }
            }
           
            
        }

        /// <summary>
        /// Writes the cell to the buffer at the specified buffer coordinates 
        /// and cell size using the cells state character. 
        /// </summary>
        /// <param name="buffer">The render buffer.</param>
        /// <param name="row">The top left buffer row index.</param>
        /// <param name="col">The top left buffer rolumn index.</param>
        /// <param name="width">The number of characters to draw horizontally.</param>
        /// <param name="height">The number of characters to draw vertically.</param>
        public void Draw(ref char[][] buffer, int row, int col, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    buffer[row + i][col + j] = RENDER_CHARACTERS[(int)state];
                }
            }
        }

    }
}
