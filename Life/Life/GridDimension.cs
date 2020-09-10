using System;

public class GridDimensions
{
	private int row = 16;
	private int column = 16;

	public int Row
    {
        get
        {
			return row;
        }
        set
        {
            row = value;
        }
    }

    public int Column
    {
        get
        {
            return column;
        }
        set
        {
            column = value;
        }
    }

    public GridDimensions()
    {
       
    }
	public GridDimensions(int row, int column)
	{
        this.Row = row;
        this.Column = column;
	}
}
