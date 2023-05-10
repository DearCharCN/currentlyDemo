using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using System;
using UnityS;
using UnityS.Mathematics;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PathCreatorWindow : EditorWindow
{

    [MenuItem("Tool/Path Creator")]
    public static void OpenWindow()
    {
        PathCreatorWindow pathCreatorWindow = new PathCreatorWindow();
        pathCreatorWindow.Show();
    }

    private float3 origin;
    private quaternion originRotation;
    private sfloat hPreLength = sfloat.One;
    private sfloat wPreLength = sfloat.One;
    private int hCount = 1;
    private int wCount = 1;

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            origin = (float3)EditorGUILayout.Vector3Field("Origin", (Vector3)origin);
            originRotation = (quaternion)Quaternion.Euler((EditorGUILayout.Vector3Field("Rotation", ((Quaternion)originRotation).eulerAngles)));

            
            EditorGUILayout.FloatField("Height Total Length (m)", (float)(hPreLength * (sfloat)hCount));
            hCount = (EditorGUILayout.IntField("Height Count", hCount));
            Mathf.Clamp(hCount, 1, hCount);
            hPreLength = (sfloat)EditorGUILayout.FloatField("Height Pre Tile Length (m)", (float)(hPreLength));


            EditorGUILayout.FloatField("Width Total Length (m)", (float)(wPreLength/(sfloat)wCount));
            wCount = EditorGUILayout.IntField("Width Count", wCount);
            Mathf.Clamp(wCount, 1, wCount);
            wPreLength = (sfloat)EditorGUILayout.FloatField("Height Pre Tile Length (m)", (float)(wPreLength));


            if (GUILayout.Button("Create Tile"))
            {
                CreateTile();
            }
        }
        EditorGUILayout.EndVertical();
    }

    List<GameObject> gameObjectsCache = new List<GameObject>();

    private void CreateTile()
    {
        _scene.transform.position = (Vector3)origin;
        _scene.transform.rotation = (Quaternion)originRotation;

        sfloat wPre = hPreLength;
        sfloat hPre = wPreLength;
        sfloat wHalf = wPre * (sfloat)0.5f;
        sfloat hHalf = hPre * (sfloat)0.5f;

        int index = 0;

        for (int h = 0; h < hCount; h++)
        {
            for (int w = 0; w < wCount; w++)
            {
                GameObject cube = null;
                if (gameObjectsCache.Count<= index)
                {
                    cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.SetParent(_scene.transform);
                    cube.GetComponent<Renderer>().material = _sceneMat;
                    gameObjectsCache.Add(cube);
                }
                else
                {
                    cube = gameObjectsCache[index];
                }
                ++index;

                sfloat x = wPre * (sfloat)w + wHalf;
                sfloat z = hPre * (sfloat)h + hHalf;
                cube.transform.position = new Vector3((float)x, 0, (float)z);
                cube.transform.localScale = new Vector3((float)wPre, 0.1f, (float)hPre);
                cube.SetActive(true);
            }
        }

        if (gameObjectsCache.Count > index)
        {
            for (int i = index; i < gameObjectsCache.Count; i++)
            {
                gameObjectsCache[i].SetActive(false);
            }
        }
    }

    ExampleScript _scene;
    Material _sceneMat;
    private void Awake()
    {
        GameObject go = new GameObject();
        _scene = go.AddComponent<ExampleScript>();
        _sceneMat = CreateMaterial();
    }

    private Material CreateMaterial()
    {
        string[] guids = AssetDatabase.FindAssets("RedTransparent");
        foreach (string guid in guids)
        {
            string path=AssetDatabase.GUIDToAssetPath(guid);
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(path);
            if(typeof(Material) == t)
            {
                return AssetDatabase.LoadAssetAtPath<Material>(path);
            }
        }
        Debug.Log("No find material 'RedTransparent'");
        return null;
    }

    private void OnDestroy()
    {
        DestroyImmediate( _scene.gameObject);
        DestroyImmediate( _sceneMat );
    }
}

public class ExampleScript : MonoBehaviour
{
    public float value = 7.0f;
}

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(ExampleScript))]
public class ExampleEditor : Editor
{
    // Custom in-scene UI for when ExampleScript
    // component is selected.
    public void OnSceneGUI()
    {
        //var t = target as ExampleScript;
        //var tr = t.transform;
        //var pos = tr.position;
        //// display an orange disc where the object is
        //var color = new Color(1, 0.8f, 0.4f, 1);
        //Handles.color = color;
        //Handles.DrawWireDisc(pos, tr.up, 1.0f);
        //// display object "value" in scene
        //GUI.color = color;
        //Handles.Label(pos, t.value.ToString("F1"));
        //Handles.PositionHandle(t.transform.position + Vector3.one, Quaternion.identity);
        //Handles.DrawWireCube(pos, new Vector3(1, 0, 1));
    }
}