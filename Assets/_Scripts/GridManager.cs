using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public SpriteSheetData spriteSheetData;

    public int cols;
    public int rows;
    float tileSize;

    public GameObject prefab;
    public Sprite Sample;

    

    void Start()
    {
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject tile = Instantiate(prefab, transform);
                SpriteRenderer tileSpriteRenderer = tile.GetComponent<SpriteRenderer>();
                tileSpriteRenderer.sprite = Sample;
                tileSize = tileSpriteRenderer.bounds.size.x;
                //Debug.Log(tileSpriteRenderer.bounds.size.x);
                float posX = j * tileSpriteRenderer.bounds.size.x;
                float posY = i * -tileSpriteRenderer.bounds.size.x;

                tile.transform.position = new Vector2(posX, posY);
            }
        }

        float gridX = cols * tileSize;
        float gridY = rows * tileSize;
        transform.position = new Vector2(-gridX / 2 + tileSize / 2, gridY / 2 - tileSize / 2);
    }

}
