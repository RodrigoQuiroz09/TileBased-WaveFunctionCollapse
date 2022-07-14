using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
