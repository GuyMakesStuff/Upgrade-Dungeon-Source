using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace UpgradeDungeon.IO
{
    public static class Saver
    {
        static string Format
        {
            get
            {
                return "sav";
            }
        }

        public static void Save(SaveFile file)
        {
            string Path = GetPath(file);
            FileStream stream = new FileStream(Path, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, file);
            stream.Close();
        }
        public static SaveFile Load(SaveFile file)
        {
            string Path = GetPath(file);
            if(!File.Exists(Path))
            {
                Debug.Log("File Not Found At-" + Path);
                return null;
            }

            FileStream stream = new FileStream(Path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SaveFile LoadedData = formatter.Deserialize(stream) as SaveFile;
            stream.Close();
            return LoadedData;
        }

        static string GetPath(SaveFile file)
        {
            return Application.persistentDataPath + "/" + file.FileName + "." + Format;
        }
    }
}