 using System.Collections;
 using System.Collections.Generic;
 using UnityEditor;
 using UnityEngine;
 
[CustomEditor(typeof(TileData))]
 public class TileDataEditor: Editor
{
    public override void OnInspectorGUI()
     {
         base.OnInspectorGUI();
         var script = (TileData)target;
 
             if(GUILayout.Button("Configure Sprite Sheet", GUILayout.Height(20)))
             {
                 script.ImportSpriteSheet();
             }
     }
}