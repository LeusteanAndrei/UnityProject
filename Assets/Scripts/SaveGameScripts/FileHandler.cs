using UnityEngine;
using System;
using System.IO;
using System.Globalization;




public class FileHandler 
{

    private string dirPath = "";
    private string fileName = "";

    public FileHandler(string dirPath, string fileName)
    {
        this.dirPath = dirPath;
        this.fileName = fileName;
    }

    public GameData Load()
    {

        string fullPath = Path.Combine(dirPath, fileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;

    }

    public void Save(GameData data)
    { 
        string fullPath = Path.Combine(dirPath, fileName);

        try
        {

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serializing to json
            string dataToStore = JsonUtility.ToJson(data, true);

            //writing to file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    {
                        writer.Write(dataToStore);
                    }
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Error occured while saving data to file: " + fullPath+"\n"+e);
        }
    }
}
