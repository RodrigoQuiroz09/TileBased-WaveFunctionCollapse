using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEditor.Callbacks;

[CreateAssetMenu(menuName = "SpriteSheetData/Tile")]
public class SpriteSheetData : ScriptableObject
{

    public int sockets;
    public Module [] modules;


    /// <summary>
    /// Proportionate the steps depending of the size of the tiles and the amount of sockets to get a color.
    /// </summary>
    public void ConfigColors()
    {
        foreach (var item in modules)
        {
            item.UpSocket = new Color[sockets];
            item.RightSocket = new Color[sockets];
            item.DownSocket = new Color[sockets];
            item.LeftSocket = new Color[sockets];


            Texture2D text = item.sprite.texture;
            // Rect Pivot is down left of the sprite

            float widthScale = item.sprite.rect.width / (sockets+1);
            float heightScale = item.sprite.rect.height / (sockets+1);
            for (int i = 0; i < sockets; i++)
            {   
                item.UpSocket[i]=text.GetPixel(
                    Mathf.FloorToInt(item.sprite.rect.x + (widthScale * (i+1))), 
                    Mathf.FloorToInt(item.sprite.rect.y + item.sprite.rect.height)
                );

                item.RightSocket[i] = text.GetPixel(
                    Mathf.FloorToInt(item.sprite.rect.x + item.sprite.rect.width), 
                    Mathf.FloorToInt(item.sprite.rect.y + item.sprite.rect.height - (heightScale * (i+1)))
                );

                item.DownSocket[i] = text.GetPixel(
                    Mathf.FloorToInt(item.sprite.rect.x + item.sprite.rect.width - + (widthScale * (i+1))), 
                    Mathf.FloorToInt(item.sprite.rect.y)
                );
                item.LeftSocket[i] = text.GetPixel(
                    Mathf.FloorToInt(item.sprite.rect.x), 
                    Mathf.FloorToInt(item.sprite.rect.y + (heightScale * (i+1)))
                );
            }
        }
    
    }
}