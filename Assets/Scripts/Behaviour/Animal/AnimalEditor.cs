using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Rabbit))]
public class ObjectBuilderEditor : Editor
{
	MockFCMHandler fcmHandler;
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Animal myAnimal = (Animal)target;
		if (fcmHandler == null)
		{
			fcmHandler = new MockFCMHandler(myAnimal.GetFCMHandler());
		}

		if (GUILayout.Button("Trigger Manual Actions"))
		{
			myAnimal.SetFCMHandler(fcmHandler);
		}
		if (GUILayout.Button("GoToFood"))
		{
			if (fcmHandler == null)
				Debug.LogWarning("You need to switch to manual control before you trigger this action");
			else
				fcmHandler.SetAction(EntityAction.GoingToFood);
		}

		if (GUILayout.Button("GoToWater"))
		{
			if (fcmHandler == null)
				Debug.LogWarning("You need to switch to manual control before you trigger this action");
			else
				fcmHandler.SetAction(EntityAction.GoingToWater);
		}
		if (GUILayout.Button("Escape"))
		{
			if (fcmHandler == null)
				Debug.LogWarning("You need to switch to manual control before you trigger this action");
			else
				fcmHandler.SetAction(EntityAction.Escaping);
		}
		if (GUILayout.Button("Breed"))
		{
			if (fcmHandler == null)
				Debug.LogWarning("You need to switch to manual control before you trigger this action");
			else
				fcmHandler.SetAction(EntityAction.SearchingForMate);
		}
	}
}