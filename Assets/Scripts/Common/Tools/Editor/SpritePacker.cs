/*
 * @Author:lily
 * @Create Time:2017-08-15 10:44:13
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class SpritePacker
{
    [MenuItem("Assets/AtlasMaker")]
    [MenuItem("Tools/AtlasMaker", false, 100)]
    static private void MakeAtlas()
    {
        string atlasDir = Application.dataPath + "/Res/Atlas";
        string spriteDir = Application.dataPath + "/Res/Resources/sprite";
        //Debug.Log(atlasDir);

        if (!Directory.Exists(spriteDir))
        {
            Directory.CreateDirectory(spriteDir);
        }
        RemoveUnuseSprite(spriteDir, atlasDir);
        //List<string> sprites = GetSprites(spriteDir);
        DirectoryInfo rootDirInfo = new DirectoryInfo(atlasDir);
        foreach (FileInfo pngFile in rootDirInfo.GetFiles("*.png", SearchOption.AllDirectories))
        {
            string allPath = pngFile.FullName;
            string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
			string newPath = allPath.Replace("Atlas","Resources/sprite").Replace(".png",".prefab");
            newPath = newPath.Replace("\\", "/");
            if (File.Exists(newPath))
            {
                //Debug.Log("already exsit:{0}", absolutePath);
                continue;
			}
            GameObject go = new GameObject(sprite.name);
            go.AddComponent<SpriteRenderer>().sprite = sprite;
//            allPath = spriteDir + "/" + sprite.name + ".prefab";
			//            string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
			string dir = newPath.Substring(0,newPath.LastIndexOf("/"));
			Debug.Log (dir);
			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);			
			newPath = newPath.Substring (newPath.IndexOf("Assets"));
			newPath = newPath.Replace('\\', '/');
			Debug.LogFormat("create prefab:{0}", newPath);
			PrefabUtility.CreatePrefab(newPath, go);
            GameObject.DestroyImmediate(go);
        }
    }

    private static void RemoveUnuseSprite(string spriteDir, string atlasDir)
    {
        List<string> sprites = new List<string>(Directory.GetFiles(spriteDir, "*.prefab"));
        List<string> atlases = new List<string>(Directory.GetFiles(atlasDir, "*.png"));
        List<string> temps = new List<string>();
        foreach (var file in atlases)
        {
            string fileName = file.Replace(atlasDir, spriteDir);
            fileName = fileName.Replace(".png", ".prefab");
            temps.Add(fileName);
        }

        foreach (var file in sprites)
        {
            if (!temps.Contains(file))
            {
                Debug.LogFormat("del file:{0}", file);
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }
}
