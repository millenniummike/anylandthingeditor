using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Text;

public class Manipulate : MonoBehaviour
{

	static State cloneExistingState(){
		int b = int.Parse(Selection.activeGameObject.name);
		State newState = new State(b);
		newState.position = Selection.activeGameObject.transform.position;
		newState.scale = Selection.activeGameObject.transform.localScale;
		newState.rotation = Selection.activeGameObject.transform.eulerAngles;
		Renderer r = Selection.activeGameObject.transform.GetChild(0).GetComponent<Renderer>();
		newState.color = r.sharedMaterial.color;
		return newState;
	}

	static int cloneExistingB(){
		int b = int.Parse(Selection.activeGameObject.name);
		return b;
	}

    [MenuItem ("Anyland/Duplicate/Duplicate Selected Object")]
	static void MenuDuplicate()
	{
		State newState = cloneExistingState();
		Manipulate.CreateObject(newState,cloneExistingB());
	}

	[MenuItem ("Anyland/Generate/All Objects - grid")]
	static void MenuGenerateGridAll()
	{
		State newState = new State(1);
		newState.scale = new Vector3(0.2f,0.2f,0.2f);
		int b = 1;
		for (int x = 0; x < 20; x++){
			for (int y = 0; y < 15; y++){
				newState.position.x = 0f + x;
				newState.position.y = 0f + y;
				Manipulate.CreateObject(newState,b);
				b++;
				if (b>17 && b<25) {b=25;}
				if (b>89 && b<144) {b=144;}
				if (b==206) {b=207;}
				if (b>251) {b=1;}
			}
		}
	}

	static void CreateObject (State state,int b)
	{
        GameObject objectLoaded = Resources.Load("" + b, typeof(GameObject)) as GameObject;
        GameObject newObject = Instantiate(objectLoaded, state.position, Quaternion.identity);
        newObject.transform.localScale = state.scale;
        newObject.transform.localEulerAngles = state.rotation;
		newObject.name = "" + b;
        //newObject.transform.SetParent(parent.transform);
        newObject.tag = "Part";
		newObject.AddComponent<Part>();

		Part p = newObject.GetComponent<Part>();
		p.states.Add(state);

        Renderer rend = newObject.transform.GetChild(0).GetComponent<Renderer>();
        var tempMaterial = new Material(rend.sharedMaterial);
        tempMaterial.color = state.color;
        rend.sharedMaterial = tempMaterial;
	}
}
