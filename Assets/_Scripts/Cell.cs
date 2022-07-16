using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Cell 
{
    public List<int> possibleTiles;

    public bool IsCollapsed;

    public Cell(int num)
    {
        IsCollapsed = false;
        possibleTiles = new List<int>();
        for (int i = 0; i < num; i++)
        {
            possibleTiles.Add(i);
        }
    }

    /// <summary>
    /// Debugging purposes
    /// </summary>
    /// <returns> Text with the content of variables in this cell</returns>
    public override string ToString()
    {
        return "Collapsed: " + IsCollapsed +  " Possible Tiles: "+String.Join("; ", possibleTiles);
    }
}
