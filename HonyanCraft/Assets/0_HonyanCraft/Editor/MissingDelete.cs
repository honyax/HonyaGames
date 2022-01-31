using UnityEditor;
using UnityEngine;

public static class MissingDelete
{
    [MenuItem("Tools/MissingDelete/Scene", false, 3)]
    public static void DeleteInScene()
    {
        GameObject[] all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < all.Length; ++i)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(all[i]);
        }
        Debug.Log("シーン中のMissingなスクリプトの削除が完了しました");
    }
}
