#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SaveDataEditorHelper
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    [MenuItem("Tools/Baskgayball/Borrar Datos Guardados (Reset Save)")]
    public static void DeleteSaveData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log($"<color=green>[Baskgayball]</color> Archivo de guardado eliminado exitosamente: {SavePath}");
        }
        else
        {
            Debug.Log($"<color=yellow>[Baskgayball]</color> No se encontro ningun archivo save.json para borrar.");
        }
    }

    [MenuItem("Tools/Baskgayball/Abrir Carpeta del Guardado")]
    public static void OpenSaveFolder()
    {
        if (File.Exists(SavePath))
        {
            EditorUtility.RevealInFinder(SavePath);
        }
        else
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }

    [MenuItem("Tools/Baskgayball/Sumar 500 Monedas (En Modo Play)")]
    public static void AddTestCoins()
    {
        if (Application.isPlaying && GameManager.Instance != null)
        {
            GameManager.Instance.CoinsMng.AddCoins(500);
            GameManager.Instance.SaveGame();
            Debug.Log("<color=green>[Baskgayball]</color> Se agregaron 500 monedas de prueba.");
        }
        else
        {
            Debug.LogWarning("[Baskgayball] Esta opcion solo funciona mientras el juego se esta ejecutando (Play Mode).");
        }
    }
}
#endif

