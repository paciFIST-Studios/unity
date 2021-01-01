using System.IO;

using UnityEngine;

public class SaveSystem : ScriptableObject
{
    public static void SavePlayer(PlayerData pd)
    {
        string path = Application.persistentDataPath + ".player";
        using (var stream = new FileStream(path, FileMode.Create))
        using (var writer = new StreamWriter(stream))
        {
            string json = JsonUtility.ToJson(pd);
            writer.Write(json);
        }
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + ".player";

        if(!File.Exists(path))
        {
            Debug.LogError("WARninG! NO SAVE FILE FOuND! path=" + path);
            return null;
        }

        var playerData = ScriptableObject.CreateInstance<PlayerData>();

        using (var stream = new FileStream(path, FileMode.Open))
        using (var reader = new StreamReader(stream))
        {
            string json = reader.ReadToEnd();
            // We have to overwrite an existing object, in order
            // to update a ScriptableObject
            JsonUtility.FromJsonOverwrite(json, playerData);
        }

        return (PlayerData)playerData;
    }
}
