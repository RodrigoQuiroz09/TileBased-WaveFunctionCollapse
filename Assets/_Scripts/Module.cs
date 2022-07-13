using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Module 
{
    public int weight=1;
    public Sprite sprite;

    public bool canRotate;

    public Color[] UpSocket;
    public Color[] RightSocket;
    public Color[] DownSocket;
    public Color[] LeftSocket;


}