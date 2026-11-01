using System.IO;
using UnityEngine;

namespace BaskgayBall.Persistence
{
    [System.Serializable]
    public class SaveData
    {
        // Bump this when the structure changes so old saves can be migrated.
        public int version = 1;
        public int coins;
        public int skinSelectedPlayer1;
        public int skinSelectedPlayer2;
    }

    public interface ISaveService
    {
        SaveData Load();
        void Save(SaveData data);
    }

    public class JsonFileSaveService : ISaveService
    {
        private const string FileName = "save.json";
        private string path;

        // Resolved lazily because Application.persistentDataPath cannot be
        // accessed from constructors or field initializers of a MonoBehaviour.
        private string Path_ => path ??= System.IO.Path.Combine(Application.persistentDataPath, FileName);

        public SaveData Load()
        {
            if (!File.Exists(Path_))
            {
                return new SaveData();
            }

            try
            {
                return JsonUtility.FromJson<SaveData>(File.ReadAllText(Path_));
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Save file could not be read, starting fresh: {e.Message}");
                return new SaveData();
            }
        }

        public void Save(SaveData data)
        {
            // Write to a temp file first so a crash mid-write cannot corrupt the real save.
            string tmpPath = Path_ + ".tmp";
            File.WriteAllText(tmpPath, JsonUtility.ToJson(data));

            if (File.Exists(Path_))
            {
                File.Delete(Path_);
            }
            File.Move(tmpPath, Path_);
        }
    }
}