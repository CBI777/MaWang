using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class JsonFileHandler
{
    public static void SaveToJson<T> (List<T> toSave, string filename)
    {
        Debug.Log(GetPath(filename));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(filename), content);
    }

    public static List<T> ReadFromJson<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));

        if(string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;
    }

    private static string GetPath(string filename)
    {
        return Application.dataPath + "/" + filename;
        //return Application.persistentDataPath + "/" + filename;
    }

    private static void WriteFile (string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private static string ReadFile (string path)
    {
        if(File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, true);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
