using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class GridManager : MonoBehaviour
{
    const int BLANK = 0;
    const int UP = 1;
    const int RIGHT = 2;
    const int DOWN = 3;
    const int LEFT = 4;
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


    int[][][] tempRules = new int[5][][]; //= new int  { { {BLANK, UP}, {BLANK, RIGHT}, {BLANK, DOWN}, {BLANK, LEFT} } };



    Dictionary<Color, int> colorNormalization;

    void Start()
    {
        tempRules[0] = new int[4][];
        tempRules[0][0] = new int[2] { BLANK, UP };
        tempRules[0][1] = new int[2] { BLANK, RIGHT };
        tempRules[0][2] = new int[2] { BLANK, DOWN };
        tempRules[0][3] = new int[2] { BLANK,LEFT};
        
        tempRules[1] = new int[4][];
        tempRules[1][0] = new int[3] { RIGHT, LEFT, DOWN };
        tempRules[1][1] = new int[3] {LEFT, UP, DOWN};
        tempRules[1][2] = new int[2] { BLANK, DOWN };
        tempRules[1][3] = new int[3] { RIGHT, UP, DOWN};

        tempRules[2] = new int[4][];
        tempRules[2][0] = new int[3] { RIGHT, LEFT, DOWN};
        tempRules[2][1] = new int[3] {LEFT, UP, DOWN};
        tempRules[2][2] = new int[3] {RIGHT, LEFT, UP};
        tempRules[2][3] = new int[2] { BLANK, LEFT};

        tempRules[3] = new int[4][];
        tempRules[3][0] = new int[2] { BLANK, UP};
        tempRules[3][1] = new int[3] {LEFT, UP, DOWN};
        tempRules[3][2] = new int[3] {RIGHT, LEFT, UP};
        tempRules[3][3] = new int[3] { RIGHT, UP, DOWN};

        tempRules[4] = new int[4][];
        tempRules[4][0] = new int[3] { RIGHT, LEFT, DOWN };
        tempRules[4][1] = new int[2] {BLANK, RIGHT};
        tempRules[4][2] = new int[3] {RIGHT, LEFT, UP};
        tempRules[4][3] = new int[3] {UP, DOWN, RIGHT};

        NormatizationOfColor();
        GetTilesFromDataSheet();
        CreationOfGrid();
    }
    void Update() 
    {

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
                    WFC();
        }
    }

    void WFC()
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
                else
                {
                    gridSprites[j + i * rows].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                }
            }
        }

        List<Cell> copyGridCell = new List<Cell>(gridCells);
        copyGridCell = copyGridCell.Where(cell=> !cell.IsCollapsed).ToList();

        if(copyGridCell.Count==0) return;

        copyGridCell.Sort((a, b) => { return a.possibleTiles.Count - b.possibleTiles.Count; });

        var subList = copyGridCell.Where(c => copyGridCell[0].possibleTiles.Count >= c.possibleTiles.Count);

        var cellPick = subList.ElementAt(UnityEngine.Random.Range(0, subList.ToList<Cell>().Count)) ;

        cellPick.IsCollapsed = true;
        var pick = cellPick.possibleTiles[UnityEngine.Random.Range(0, cellPick.possibleTiles.Count)];
        cellPick.possibleTiles = new List<int> { pick };
        


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
                    int[] options = new int[5] { BLANK,UP,RIGHT,DOWN,LEFT };
                    //Look Up
                    if (i>0)
                    {
                        Cell up = gridCells[j + (i - 1) * rows];
                        List<int> validOptions = new List<int>();
                        foreach (var option in up.possibleTiles)
                        {
                            var valid = tempRules[option][2];
                            validOptions.AddRange(valid.ToList());
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
                            var valid = tempRules[option][3];
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
                            var valid = tempRules[option][0];
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
                            var valid = tempRules[option][1];
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

    void CheckValid(ref int[]options, List<int>valid)
    {
        for (int i = options.Length-1; i >=0 ; i--)
        {
            if(!valid.Contains(options[i]))
            {

                options = options.Where((source, index) => index != i).ToArray();
                //Debug.Log(String.Join("; ", options));
            }
        }
    }

    void GetTilesFromDataSheet()
    {
        tiles = new List<Tile>();
        int counterModules=0;
        int limitTiles = spriteSheetData.modules.Length;

        while (counterModules<limitTiles)
        {
            
            if(!spriteSheetData.modules[counterModules].canRotate)
            {
                
                tiles.Add( new Tile(
                    spriteSheetData.modules[counterModules].sprite,
                    ConcatSockets(spriteSheetData.modules[counterModules])
                    ));
                counterModules++;
            }
            else
            {
                limitTiles += 3;
                tiles.Add( new Tile(
                    spriteSheetData.modules[counterModules].sprite,
                    ConcatSockets(spriteSheetData.modules[counterModules])
                    ));
                
                for (int i = 0; i < 3; i++)
                {
                    tiles.Add(tiles[counterModules].Rotate(i+1));
                }
                counterModules+=4;
            }
        }
    }

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
