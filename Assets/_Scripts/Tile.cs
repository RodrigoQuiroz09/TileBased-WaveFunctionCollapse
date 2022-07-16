using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile 
{
    public Sprite sprite;
    public List<string> sockets;
    public List<int> up;
    public List<int> right;
    public List<int> down;
    public List<int> left;

    public int rotation = 0;
    string Reverse(string str) 
    {  
        char[] chars = str.ToCharArray();  
        for (int i = 0, j = str.Length - 1; i < j; i++, j--) 
        {  
            char c = chars[i];  
            chars[i] = chars[j];  
            chars[j] = c;  
        }  
        return new string(chars);  
    }  
    public Tile(Sprite img, List<string>sockets)
    {
        sprite = img;
      
        this.sockets = new List<string>( sockets);
    }

    bool CompareSockets(string a, string b)
    {
        return a == Reverse(b);
    }

    public void CreateRules(List<Tile> tiles)
    {
        up = new List<int>();
        right = new List<int>();
        down = new List<int>();
        left = new List<int>();
        for (int i = 0; i < tiles.Count; i++)
        {
            //UP
            if(CompareSockets(tiles[i].sockets[2],sockets[0]))
            {
                this.up.Add(i);
            }

            //RIGHT
            if(CompareSockets(tiles[i].sockets[3],sockets[1]))
            {
                this.right.Add(i);
            }

            //DOWN
            if(CompareSockets(tiles[i].sockets[0],sockets[2]))
            {
                this.down.Add(i);
            }

            //LEFT
            if(CompareSockets(tiles[i].sockets[1],sockets[3]))
            {
                this.left.Add(i);
            }
        }
    }

    public void Rotate(int num)
    {
        rotation=-90*num;
        List<string> newSockets=new List<string>();

        int len = sockets.Count;

        for (int i = 0; i < len; i++)
        {
            newSockets.Add(sockets[(i - num + len) % len]);
        }
        sockets = newSockets;
    }
}
