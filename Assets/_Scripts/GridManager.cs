using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

        //gridCells[2].IsCollapsed = true;
        // gridCells[0].possibleTiles = new List<int> { 0,1 };
        // gridCells[2].possibleTiles = new List<int> { 0,1 };
    }
    void Update() 
    {
        List<Cell> copyGridCell = new List<Cell>(gridCells);
        copyGridCell.Sort((a, b) => { return a.possibleTiles.Count - b.possibleTiles.Count; });
        
        // foreach (var item in gridCells)
        // {
        //     string result = "";
        //     foreach (var x in item.possibleTiles)
        //     {
        //         result += x.ToString() + ", ";
        //     }
        //     Debug.Log(result);
        // }

        var subList = copyGridCell.Where(c => copyGridCell[0].possibleTiles.Count >= c.possibleTiles.Count);

        var cellPick = subList.ElementAt(Random.Range(0, subList.ToList<Cell>().Count)) ;

        cellPick.IsCollapsed = true;
        var pick = cellPick.possibleTiles[Random.Range(0, cellPick.possibleTiles.Count)];
        cellPick.possibleTiles = new List<int> { pick };
        
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Cell cell = gridCells[j + i * cols];

                if(cell.IsCollapsed)
                {
                    int index = cell.possibleTiles[0];
                    gridSprites[j + i * cols].GetComponent<SpriteRenderer>().sprite=tiles[index].sprite;
                    gridSprites[j + i * cols].GetComponent<Transform>().rotation = Quaternion.Euler(0,0,tiles[index].rotation) ;
                }
                else
                {
                    gridSprites[j + i * cols].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                }
            }
        }
        enabled = false;
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
