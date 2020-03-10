using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{

    bool autoupdate;

    public override void OnInspectorGUI()
    {
        GameController gameController = (GameController)target;

       if (GUILayout.Button("Spawn Rabbit"))
        {
            gameController.SpawnRabbit();
        }
        else if (GUILayout.Button("Spawn Plant"))
        {
            gameController.SpawnPlant();
        }
        else if (GUILayout.Button("Spawn All"))
        {
            gameController.SpawnEntities();
        }
        //kernal.GenerateMap();
    }


}
