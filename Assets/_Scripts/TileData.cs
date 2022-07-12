using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEditor.Callbacks;

[CreateAssetMenu(menuName = "TileData/Tile")]
public class TileData : ScriptableObject
{

    public int sockets;
    public Module [] modules;


    public void ImportSpriteSheet()
    {
        foreach (var item in modules)
        {
            item.UpSocket = new Color[sockets];
            item.RightSocket = new Color[sockets];
            item.LeftSocket = new Color[sockets];
            item.DownSocket = new Color[sockets];

            Texture2D text = item.sprite.texture;

            item.UpSocket[0] = text.GetPixel(0, 0);
        }
    }

}