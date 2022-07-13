 using System.Collections;
 using System.Collections.Generic;
 using UnityEditor;
 using UnityEngine;
 
[CustomEditor(typeof(SpriteSheetData))]
 public class SpriteSheetEditor: Editor
{
    public override void OnInspectorGUI()
     {
         base.OnInspectorGUI();
         var script = (SpriteSheetData)target;
 
             if(GUILayout.Button("Configure Color Sockets", GUILayout.Height(20)))
             {
                 script.ConfigColors();
             }
     }
}