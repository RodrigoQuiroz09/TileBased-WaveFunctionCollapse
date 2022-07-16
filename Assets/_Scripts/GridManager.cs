using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class GridManager : MonoBehaviour
{
    const int SIDES = 4;
    public SpriteSheetData spriteSheetData;

    public int cols;
    public int rows;
    float tileSize;

    public GameObject prefab;
    public Sprite Sample;

    public List<Tile> tiles;
    public List<Cell> gridCells;
    public List<GameObject> gridSprites;

    Dictionary<Color, int> colorNormalization;

    void Start()
    {
        NormatizationOfColor();
        GetTilesFromDataSheet();
        CreationOfGrid();
    }
    void Update() 
    {
        WFC();
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //WFC();
        }
    }

    /// <summary>
    /// Implementation of a instance of the algorithm. Searchs for the adjacent cells for each cell and remove the not valid options.
    /// <para>
    /// TODO: Bactracking of the search in the near neighbours (Save comparisons). 
    /// </para>
    /// <para>
    /// TODO: Remove redundant tiles (Optimization). 
    /// </para>
    /// <para>
    /// TODO: Check in the search of the possible tiles in the cells, collapse the cells with only 1 possible tile (Optimization).
    ///</para>
    /// </summary>
    void WFC()
    {
        PickRandomWithLeastEntropy();

        DrawGrid();

        List<Cell> nextGrid = new List<Cell>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = j + i * rows;

                if(gridCells[index].IsCollapsed)
                {
                    nextGrid.Add( gridCells[index]);
                }
                else
                {
                    List<int> options =  gridCells[index].possibleTiles;

                    //Look Up
                    if (i>0)
                    {
                        Cell up = gridCells[j + (i - 1) * rows];
                        List<int> validOptions = new List<int>();
                        foreach (var option in up.possibleTiles)
                        {
                            var valid = tiles[option].down;
                            validOptions.AddRange(valid);
                        }
                        CheckValid(ref options, validOptions);
                    }

                    //Look Right
                    if (j<cols-1)
                    {
                        Cell right = gridCells[j + 1 + i* rows];
                        List<int> validOptions = new List<int>();
                        foreach (var option in right.possibleTiles)
                        {
                            var valid = tiles[option].left;
                            validOptions.AddRange(valid.ToList());
                        }
                        CheckValid(ref options, validOptions);
                    }
                    
                    //Look Down
                    if (i<rows-1)
                    {
                        Cell down = gridCells[j + (i + 1) * rows];
                        List<int> validOptions = new List<int>();
                        foreach (var option in down.possibleTiles)
                        {
                            var valid = tiles[option].up;
                            validOptions.AddRange(valid.ToList());
                        }
                        CheckValid(ref options, validOptions);
                    } 

                    //Look Left
                    if (j>0)
                    {
                        Cell left = gridCells[j - 1 + i* rows];
                        List<int> validOptions = new List<int>();
                        foreach (var option in left.possibleTiles)
                        {
                            var valid = tiles[option].right;
                            validOptions.AddRange(valid.ToList());
                        }
                        CheckValid(ref options, validOptions);
                    }
                    nextGrid.Add( gridCells[index]);
                    nextGrid[index].possibleTiles = options.ToList();   
                }
            }
        }
        gridCells = nextGrid;
    }

    /// <summary>
    /// Grab the Cells that are not collapsed and choose the ones with least entropy (least amount of possible tiles).
    /// Pick a random cell to collapse bewtween the filtered entropy. Pick a random possible tile and saves it.
    /// </summary>
    void PickRandomWithLeastEntropy()
    {
        List<Cell> copyGridCell = new List<Cell>(gridCells);
        copyGridCell = gridCells.Where(cell => !cell.IsCollapsed).ToList();

        if(copyGridCell.Count==0) return;

        var minEntropy = copyGridCell.Select(cell=>cell.possibleTiles.Count)?.Min();
        copyGridCell = copyGridCell.Where(cell=> cell.possibleTiles.Count==minEntropy).ToList();

        var cellPick = copyGridCell.ElementAt(UnityEngine.Random.Range(0, copyGridCell.Count)) ;
        cellPick.IsCollapsed = true;
        var pick = cellPick.possibleTiles[UnityEngine.Random.Range(0, cellPick.possibleTiles.Count)];
        cellPick.possibleTiles = new List<int> { pick };
    }

    /// <summary>
    /// Draw the cells that are collapsed in the grid of sprites.
    /// </summary>
    void DrawGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Cell cell = gridCells[j + i * rows];

                if(cell.IsCollapsed)
                {
                    int index = cell.possibleTiles[0];
                    gridSprites[j + i * rows].GetComponent<SpriteRenderer>().sprite=tiles[index].sprite;
                    gridSprites[j + i * rows].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                    gridSprites[j + i * rows].GetComponent<Transform>().rotation = Quaternion.Euler(0,0,tiles[index].rotation) ;
                }
            }
        }
    }

    /// <summary>
    /// Compares what is valid for the tile the cell is looking at and removes the not valid ones in the options of the cell.
    /// <para>
    ///  <item>
    ///    <term>valid</term>
    ///   <description>{0,2}</description>
    ///  </item>
    /// </para>
    /// <para>
    ///  <item>
    ///    <term>options</term>
    ///   <description>{0,1,2,3,4}</description>
    ///  </item>
    /// </para>
    /// <para>
    ///  <item>
    ///    <term>options (output)</term>
    ///   <description>{0,2}</description>
    ///  </item>
    /// </para>
    /// </summary>
    /// <param name="options">Options available at the moment for a certain cell. Is a reference</param>
    /// <param name="valid">Valid option depending of the adjacency rules</param>
    void CheckValid(ref List<int>options, List<int>valid)
    {
        for (int i = options.Count-1; i >=0 ; i--)
        {
            if(!valid.Contains(options[i])) options.RemoveAt(i);  
        }
    }

    /// <summary>
    /// Get the SpriteSheetData object and map its tiles with the respective configured and normalized sockets (Rotates the ones marked by the Module).
    /// Creates the rules of adjacency rules of compatible sockets with its respective side in the Tiles script.
    /// </summary>
    void GetTilesFromDataSheet()
    {
        tiles = new List<Tile>();
        int counterModules=0;
        int listCounter=0;
        int limitTiles = spriteSheetData.modules.Length;

        while (counterModules<limitTiles) 
        
        {
            if(!spriteSheetData.modules[counterModules].canRotate)
            {
                tiles.Add(new Tile(
                    spriteSheetData.modules[counterModules].sprite,
                    ConcatSockets(spriteSheetData.modules[counterModules]).ToList()
                    ));
                counterModules++;
                listCounter++;
            }
            else
            {
                for (int i = 0; i < SIDES; i++)
                {
                    tiles.Add( new Tile(
                        spriteSheetData.modules[counterModules].sprite,
                        ConcatSockets(spriteSheetData.modules[counterModules]).ToList()
                        ));
                    listCounter++;
                }
                
                for (int i = 3; i > 0; i--)
                {
                    tiles[listCounter-i].Rotate(i);
                }
                counterModules++;
            }
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].CreateRules(tiles);
        }

    }

    /// <summary>
    /// Create a serialization with the normalized tiles socket values and create strings that will work as a comparison point.
    /// </summary>
    /// <param name="module"> The tile to normalize</param>
    /// <returns>The new relations of sockets and normalization of color</returns>
    string [] ConcatSockets(Module module)
    {
        string[] socketsConcat = {"","","",""};

        for (int i = 0; i < spriteSheetData.sockets; i++)
        {
            socketsConcat[0] = socketsConcat[0]+colorNormalization[module.UpSocket[i]];
            socketsConcat[1] = socketsConcat[1]+colorNormalization[module.RightSocket[i]];
            socketsConcat[2] = socketsConcat[2]+colorNormalization[module.DownSocket[i]];
            socketsConcat[3] = socketsConcat[3]+colorNormalization[module.LeftSocket[i]];
        }

        return socketsConcat;
    }


    /// <summary>
    /// Create a association with every unique color of the tile to a number for a faster concatenation and comparison.
    /// For Each module it extract each face of the tile (sockets) and maps the into a dictionary {colorNormalization}
    /// </summary>
    void NormatizationOfColor()
    {
        int counter=0;
        colorNormalization = new Dictionary<Color, int>();

        foreach (var item in spriteSheetData.modules)
        {
            for (int i = 0; i < spriteSheetData.sockets; i++)
            {
                if(!colorNormalization.ContainsKey(item.UpSocket[i]))
                {
                    colorNormalization.Add(item.UpSocket[i], counter);
                    counter++;
                }
                if(!colorNormalization.ContainsKey(item.RightSocket[i]))
                {
                    colorNormalization.Add(item.RightSocket[i], counter);
                    counter++;
                }
                if(!colorNormalization.ContainsKey(item.DownSocket[i]))
                {
                    colorNormalization.Add(item.DownSocket[i], counter);
                    counter++;
                }
                if(!colorNormalization.ContainsKey(item.LeftSocket[i]))
                {
                    colorNormalization.Add(item.LeftSocket[i], counter);
                    counter++;
                }
            }
        }
    }

    /// <summary>
    /// Instanciates a generic prefab with a sprite renderer (Can change in the future) and puts them in a list.
    /// Aligns them into a grid depending of the sample size and paints it black.
    /// Create a List od Cells which will be used in the main algorithm.
    /// </summary>
    void CreationOfGrid()
    {
        gridSprites = new List<GameObject>();
        gridCells = new List<Cell>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject tile = Instantiate(prefab, transform);
                SpriteRenderer tileSpriteRenderer = tile.GetComponent<SpriteRenderer>();
                tileSpriteRenderer.sprite = Sample;
                tileSize = tileSpriteRenderer.bounds.size.x;
                float posX = j * tileSpriteRenderer.bounds.size.x;
                float posY = i * -tileSpriteRenderer.bounds.size.x;
                tileSpriteRenderer.color=new Color(0, 0, 0);

                tile.transform.position = new Vector2(posX, posY);

                gridSprites.Add(tile);
                gridCells.Add(new Cell(tiles.Count));
            }
        }

        float gridX = cols * tileSize;
        float gridY = rows * tileSize;
        transform.position = new Vector2(-gridX / 2 + tileSize / 2, gridY / 2 - tileSize / 2);
    }
}
