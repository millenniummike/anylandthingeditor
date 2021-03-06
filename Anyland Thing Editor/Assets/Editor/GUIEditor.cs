using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
public enum OPTIONS
{
    DEFAULT = 0,
    SPIRAL = 1,
    RING = 2,
    ARCH = 3,
    HORN = 4,
    GRID_FLAT = 5,
    GRID_FLAT_SPACED = 6,
    TOWER = 7,
    STAIRS = 8,
    BRICKS = 9,
    ROOM_GENERATE = 1000,
    TABLE_GENERATE = 1001
}

public enum PARTS
{
    CUBE = 1,
    PYRAMID = 2,
    SPHERE = 3,
    CONE = 4,
    CYLINDER = 5,
    TRIANGLE = 6,
    TRAPEZE = 7,
    HEDRA = 8,
    ICOSPHERE = 9,
    LOWPOLYSPHERE = 10,
    RAMP = 11,
    JITTERCUBE = 12,
    CHAMFERCUBE = 13,
    SPIKE = 14,
    LOWPOLYCYLINDER = 15,
    
    HALFSPHERE = 16,
    
    JITTERSPHERE = 17,
    
    BIGDIALOG = 25,
    
    QUARTERPIPE1 = 26,
    
    QUARTERPIPE2 = 27,
    
    QUARTERPIPE3 = 28,
    
    QUARTERPIPE4 = 29,
    
    QUARTERPIPE5 = 30,
    CURVERAMP = 38,
    RING4 = 57,
    BOWL1SOFT = 86,
    SPIKESOFT = 174,
    SHRINKDISK = 203,
}


[CustomEditor(typeof(Part)), CanEditMultipleObjects]
public class GUIEditor : Editor
{
    static OPTIONS op;

    static PARTS partSelected = PARTS.CUBE;
    static int b = 1;
    static Vector3 max = new Vector3(5f,5f,5f);
    static Vector3 spacing = new Vector3(1f,1f,1f);
    static Vector3 rowOffset = new Vector3(0f,0f,0f);
    static float radius = 2f;
    static float circleSize = 20;
    static float theta_scale = 0.02f;
    static Vector3 rotationOffset = new Vector3(0,0,0);
    static Vector3 scaleOffset = new Vector3(1,1,1);
    static Vector3 positionOffset = new Vector3(0f,0f,0f);

    static Vector3 jitter = new Vector3(0f,0f,0f);
    static int iterations = 5;
    static float speed = 1f;
    static int currentState = 0;

    static List<GameObject> newGameObjects;

    static GameObject lastSelected;
    public override void OnInspectorGUI()
    {
        
        partSelected = (PARTS)EditorGUILayout.EnumPopup("Part:", partSelected);
        iterations = EditorGUILayout.IntField("Iterations:", iterations);
        speed = EditorGUILayout.FloatField("Forward Speed:", speed);
        rotationOffset = EditorGUILayout.Vector3Field("Rotation offset:", rotationOffset);
        positionOffset = EditorGUILayout.Vector3Field("Position offset:", positionOffset);
        scaleOffset = EditorGUILayout.Vector3Field("Scale offset:", scaleOffset);
        jitter = EditorGUILayout.Vector3Field("Jitter:", jitter);
        max = EditorGUILayout.Vector3Field("Max grid size:", max); 
        spacing = EditorGUILayout.Vector3Field("Grid spacing:", spacing);
        rowOffset = EditorGUILayout.Vector3Field("Grid Row offset:", rowOffset);
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 90f;
        radius = EditorGUILayout.FloatField("Radius:", radius, GUILayout.MaxWidth(200.0f));
        circleSize = EditorGUILayout.FloatField("Points of circle:", circleSize, GUILayout.MaxWidth(200.0f));
        theta_scale = EditorGUILayout.FloatField("Theta scale:", theta_scale, GUILayout.MaxWidth(200.0f));
        GUILayout.EndHorizontal();

        var rect2 = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect2.x - 15, rect2.y), new Vector2(rect2.width + 15, rect2.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        GUILayout.BeginHorizontal();
        
        op = (OPTIONS)EditorGUILayout.EnumPopup("Setting:", op);

        if(GUILayout.Button("Load Setting", GUILayout.Width(100), GUILayout.Height(20))){
            loadSettings();
        }

        if(GUILayout.Button("Create Part", GUILayout.Width(100), GUILayout.Height(20))){
            newGameObjects = new List<GameObject>();
            createPart();
        }

        if(GUILayout.Button("Duplicate Part", GUILayout.Width(100), GUILayout.Height(20))){
            GameObject go = CopyObject(Selection.activeGameObject);
            newGameObjects = new List<GameObject>();
            lastSelected = Selection.activeGameObject;
            newGameObjects.Add(go);
        }
        GUILayout.EndHorizontal();

         GUILayout.BeginHorizontal();
        if(GUILayout.Button("Create Grid", GUILayout.Width(100), GUILayout.Height(20))){
            createGrid();
        }

        if(GUILayout.Button("Create Circle", GUILayout.Width(100), GUILayout.Height(20))){
        GameObject originalGo = Selection.activeGameObject;
        float theta = 0f;
        newGameObjects = new List<GameObject>();
        lastSelected = Selection.activeGameObject;

        for (int i = 0; i < circleSize; i++)
            {
                GameObject go = CopyObject(originalGo);
                Vector3 newPos = originalGo.transform.position;
                theta += (4.0f * Mathf.PI * theta_scale);
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);
                newPos.x = newPos.x + x;
                newPos.y = newPos.y + y;
                
                Vector3 newScale = Selection.activeGameObject.transform.localScale;
                newScale.x += scaleOffset.x;
                newScale.y += scaleOffset.y;
                newScale.z += scaleOffset.z;
                
                go.transform.position = newPos;
                go.transform.localScale = newScale;
                newGameObjects.Add(go);
            }
        }

        if(GUILayout.Button("Generate"))
        {
           newGameObjects = new List<GameObject>();
           lastSelected = Selection.activeGameObject;
           int flag=0;
        for (int i = 0; i < iterations; i++)
            {
                GameObject go = CopyObject(Selection.activeGameObject);
                newGameObjects.Add(go);
                Vector3 newPosition;
                if (flag==0) {
                    newPosition = go.transform.TransformDirection(Vector3.forward * 0 * go.transform.localScale.z);
                } else {
                   newPosition = go.transform.TransformDirection(Vector3.forward * speed * go.transform.localScale.z); 
                }
                flag++;
                go.transform.Rotate(rotationOffset.x, rotationOffset.y, rotationOffset.z, Space.Self);
                
                newPosition += positionOffset;
                Vector3 variation = new Vector3(UnityEngine.Random.Range(0,jitter.x),UnityEngine.Random.Range(0,jitter.y),UnityEngine.Random.Range(0,jitter.z));
                newPosition += variation;

                Vector3 newScale = Selection.activeGameObject.transform.localScale;
                newScale.x *= scaleOffset.x;
                newScale.y *= scaleOffset.y;
                newScale.z *= scaleOffset.z;

                if (newScale.x < 0) { newScale.x = 0; }
                if (newScale.y < 0) { newScale.y = 0; }
                if (newScale.z < 0) { newScale.z = 0; }

                if (newScale.x==0 && newScale.y==0 && newScale.z==0)
                {break;}
                go.transform.localPosition += newPosition;
                go.transform.localScale = newScale;
                
                saveState(go);
                Selection.activeGameObject = go;
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Add State", GUILayout.Width(100), GUILayout.Height(20))){
            Part p =Selection.activeGameObject.GetComponent<Part>();
            p.states.Add(new State());
        }

        if(GUILayout.Button("Previous State", GUILayout.Width(100), GUILayout.Height(20))){
            currentState--;
            Part p =Selection.activeGameObject.GetComponent<Part>();
            if (currentState<0) {currentState=p.states.Count-1;}
            updateCurrentObject();
        }

        if(GUILayout.Button("Next State", GUILayout.Width(100), GUILayout.Height(20))){
            currentState++;
            Part p =Selection.activeGameObject.GetComponent<Part>();
            if (currentState>=p.states.Count) {currentState=0;}
            updateCurrentObject();
        }

        if(GUILayout.Button("Save State", GUILayout.Width(100), GUILayout.Height(20))){
            saveState(Selection.activeGameObject);
        }

        GUILayout.EndHorizontal();
        currentState = EditorGUILayout.IntField("Current State:", currentState);

        if(GUILayout.Button("Undo", GUILayout.Width(100), GUILayout.Height(20))){
            Selection.activeGameObject = lastSelected;
            foreach(GameObject go in newGameObjects){
                DestroyImmediate(go);
            }
        }
        try {DrawDefaultInspector ();} catch (Exception e) {

        }
    }

    void updateCurrentObject(){
        Part p = Selection.activeGameObject.GetComponent<Part>();
        Selection.activeGameObject.transform.position = p.states[currentState].position;
        Selection.activeGameObject.transform.eulerAngles = p.states[currentState].rotation;
        Selection.activeGameObject.transform.localScale = p.states[currentState].scale;
        Selection.activeGameObject.transform.position = p.states[currentState].position;

        Renderer rend = Selection.activeGameObject.transform.GetChild(0).GetComponent<Renderer>();
		var tempMaterial = new Material(rend.sharedMaterial);
 		tempMaterial.color = p.states[currentState].color;
 		rend.sharedMaterial = tempMaterial;
    }

    static void saveState(GameObject sourceGo){
        Part p = sourceGo.GetComponent<Part>();
        State c = p.states[currentState];
        c.position =  sourceGo.transform.position;
        c.rotation = sourceGo.transform.eulerAngles;
        c.scale = sourceGo.transform.localScale;
        Renderer rend = sourceGo.transform.GetChild(0).GetComponent<Renderer>();
		var tempMaterial = rend.sharedMaterial;
 		c.color = tempMaterial.color;
        p.states[currentState] = c;
    }

    static void saveStateAll(GameObject sourceGo){
        int index = 0;
        foreach (State s in sourceGo.GetComponent<Part>().states) {
            Part p = sourceGo.GetComponent<Part>();
            State o = s;
            o.position =  sourceGo.transform.position;
            o.rotation = sourceGo.transform.eulerAngles;
            o.scale = sourceGo.transform.localScale;
            p.states[index] = o;
            index++;
        }
    }
    
    static GameObject CreateObject (State state, int b)
	{
        GameObject objectLoaded = Resources.Load("" + b, typeof(GameObject)) as GameObject;
        GameObject newObject = Instantiate(objectLoaded, state.position, Quaternion.identity);
        newObject.transform.localScale = state.scale;
        newObject.transform.localEulerAngles = state.rotation;
        //newObject.transform.SetParent(parent.transform);
        newObject.name = "" + b;
        newObject.tag = "Part";
		newObject.AddComponent<Part>();
        Part p = newObject.GetComponent<Part>();
		p.states.Add(state);
        Renderer rend = newObject.transform.GetChild(0).GetComponent<Renderer>();
        var tempMaterial = new Material(rend.sharedMaterial);
        tempMaterial.color = state.color;
        rend.sharedMaterial = tempMaterial;
        Selection.activeGameObject = newObject;
        saveState(Selection.activeGameObject);
        return newObject;
	}

    static GameObject CopyObject(GameObject originalGo){
		GameObject go = Instantiate(originalGo);
        go.transform.parent = originalGo.transform.parent;
        Selection.activeGameObject = go;
        go.name = originalGo.name;
        return go;
    }

    static List<State> cloneExistingStates(){
        Part p = Selection.activeGameObject.GetComponent<Part>();
		List<State> states = p.states;

		return states;
	}

	static int cloneExistingB(){
		int b = int.Parse(Selection.activeGameObject.name);
		return b;
	}

    static void createPart(){
        b = (int) partSelected;
        State newState = new State(1);
        lastSelected = Selection.activeGameObject;
        GameObject go = CreateObject(newState,b);
        newGameObjects.Add(go);
        Selection.activeGameObject = go;
    }

    static void createGrid(){
        newGameObjects = new List<GameObject>();
            lastSelected = Selection.activeGameObject;
            GameObject originalGo = Selection.activeGameObject;
            Vector3 rowOffsetCurrent = new Vector3(0,0,0);
            int flip = 1;
            for (float x = 0; x < max.x; x = x + 1){
                for (float y = 0; y < max.y; y = y + 1){
                    for (float z = 0; z < max.z; z = z + 1){
                        GameObject go = CopyObject(Selection.activeGameObject);
                        Vector3 newPos = originalGo.transform.position;
                        newPos.x += x * spacing.x;
                        newPos.y += y * spacing.y;
                        newPos.z += z * spacing.z;

                        newPos.x += positionOffset.x;
                        newPos.y += positionOffset.y;
                        newPos.z += positionOffset.z;

                        newPos += rowOffsetCurrent;

                        Vector3 newScale = Selection.activeGameObject.transform.localScale;
                        newScale.x *= scaleOffset.x;
                        newScale.y *= scaleOffset.y;
                        newScale.z *= scaleOffset.z;
                        go.transform.Rotate(rotationOffset.x, rotationOffset.y, rotationOffset.z, Space.Self);
                        go.transform.position = newPos;
                        go.transform.localScale = newScale;
                        Selection.activeGameObject = go;
                        saveStateAll(go);
                        newGameObjects.Add(go);
                    }
                    rowOffsetCurrent += rowOffset * flip;
                    flip *= -1;
                }
            }
    }
    void loadSettings(){
        // set defaults
                iterations=5;
                rotationOffset = new Vector3(0,0,0);
                positionOffset = new Vector3(0,0,0);
                scaleOffset = new Vector3(1,1,1);
                jitter = new Vector3(0,0,0);
                speed = 1;
                Vector3 pos;
            switch (op)
         {
                //** todo load from file */
                case OPTIONS.SPIRAL:
                    iterations=20;
                    rotationOffset.y=30;
                    positionOffset.y=1;
                break;

                case OPTIONS.RING:
                    iterations=36;
                    rotationOffset.x=10;
                break;

                case OPTIONS.ARCH:
                    iterations=18;
                    rotationOffset.x=10;
                break;

                case OPTIONS.HORN:
                    iterations=18;
                    speed=0.8f;
                    rotationOffset.x=-20;
                    scaleOffset=new Vector3(0.9f,0.9f,0.9f);
                break;
                case OPTIONS.GRID_FLAT:
                    max = new Vector3(10,1,10);
                    spacing = new Vector3(1,1,1);
                break;

                case OPTIONS.GRID_FLAT_SPACED:
                    max = new Vector3(10,1,10);
                    spacing = new Vector3(1.1f,1.1f,1.1f);
                break;

                case OPTIONS.TOWER:
                    iterations = 10;
                    speed = 0.8f;
                    scaleOffset=new Vector3(0.8f,0.8f,0.8f);
                break;

                case OPTIONS.STAIRS:
                    iterations = 10;
                    positionOffset=new Vector3(0f,1f,0f);
                break;

                case OPTIONS.BRICKS:
                    iterations = 10;
                    spacing = new Vector3(1.1f,1.1f,1.1f);
                    max = new Vector3(10,1,10);
                    rowOffset = new Vector3(0,0,0.5f);  
                break;

                case OPTIONS.ROOM_GENERATE:
                    newGameObjects = new List<GameObject>();
                    partSelected=PARTS.CUBE;
                    // grid floor
                    createPart();
                    Selection.activeGameObject.transform.localScale=new Vector3(1,0.3f,1);
                    Selection.activeGameObject.transform.position=new Vector3(-5,0,-5);
                    saveState(Selection.activeGameObject);
                    op = OPTIONS.GRID_FLAT_SPACED;
                    loadSettings();
                    createGrid();

                    // floor
                    createPart();
                    Selection.activeGameObject.transform.localScale=new Vector3(10,0.1f,10);
                    
                    // ceiling
                    createPart();
                    Selection.activeGameObject.transform.localScale=new Vector3(10,0.3f,10);
                    Selection.activeGameObject.transform.position=new Vector3(0,10,0);
                    saveState(Selection.activeGameObject);
                    // wall1
                    createPart();
                    Selection.activeGameObject.transform.localScale=new Vector3(10,10,0.3f);
                    Selection.activeGameObject.transform.position=new Vector3(0,5,5);
                    saveState(Selection.activeGameObject);
                    // wall2
                    createPart();
                    Selection.activeGameObject.transform.localScale=new Vector3(10,10,0.3f);
                    Selection.activeGameObject.transform.position=new Vector3(0,5,-5);
                    saveState(Selection.activeGameObject);
                    

                break;

                case OPTIONS.TABLE_GENERATE:

                break;
            }
    }
}

