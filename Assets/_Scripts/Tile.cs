using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile 
{
    public Sprite sprite;
    public string[] sockets;

    public int rotation = 0;

    public Tile(Sprite img, string[] sockets)
    {
        sprite = img;
        this.sockets = sockets;
    }
    
    public void Rotate(int num)
    {
        rotation=-90*num;
        string[] newSockets={"","","",""};

        int len = newSockets.Length;

        for (int i = 0; i < len; i++)
        {
            newSockets[i] = sockets[(i - num + len) % len];
        }
    }
}
