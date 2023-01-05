#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class EditorCubicalLayout
{
    [MenuItem("YOUR_MENU_NAME/YOUR_SUBMENU_NAME/YOUR_METHOD_NAME %&n")]
    static void CreateAPrefab()
    {
        //var parent = (GameObject)PrefabUtility.InstantiatePrefab((GameObject) Selection.activeObject);
        GameObject parent = (GameObject)Selection.activeObject;
        parent.name = "Cubical";
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                SelectPrefab(Random.Range(0, 4), new Vector3(i, 0, j) * 3, parent);

    }

    static void SelectPrefab(int prefab, Vector3 pos, GameObject parent)
    {
        GameObject cubical = null;
        switch (prefab)
        {
            case 0:
                cubical = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Gameobjects/Map/Props/Prefabs/Cubical1.prefab"));
                break;
            case 1:
                cubical = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Gameobjects/Map/Props/Prefabs/Cubical2.prefab"));
                break;
            case 2:
                cubical = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Gameobjects/Map/Props/Prefabs/Cubical3.prefab"));
                break;
            case 3:
                cubical = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Gameobjects/Map/Props/Prefabs/Cubical4.prefab"));
                break;
            case 4:
                cubical = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Gameobjects/Map/Props/Prefabs/Cubical5.prefab"));
                break;
        }
        if (cubical != null)
        {
            cubical.transform.SetParent(parent.transform);
            cubical.transform.localPosition = pos;
            cubical.transform.localEulerAngles = Vector3.zero;
            cubical.transform.localScale = Vector3.one;
        }
    }
}
#endif