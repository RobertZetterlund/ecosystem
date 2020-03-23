using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainKernal))]
public class MapController : Editor
{

    bool autoupdate;
    
    public override void OnInspectorGUI()
    {
        TerrainKernal kernal = (TerrainKernal)target;

        if(DrawDefaultInspector()){
            if(kernal.autoUpdate){
                kernal.UpdateMap();
                kernal.GenerateMap();
                kernal.GenerateMesh();
            }

        }else if(GUILayout.Button("Generate Map")){
            kernal.UpdateMap();
            kernal.GenerateMap();
        }else if(GUILayout.Button("Generate Mesh")){
            kernal.UpdateMap();
            kernal.GenerateMesh();
        }
        
    }


}
