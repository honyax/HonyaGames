#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class MissingDelete
{
    // �V�[�����ɑ��݂���Missing�ȃX�N���v�g���폜����
    [MenuItem("Project/MissingDelete/Scene", false, 3)]
    public static void DeleteInScene()
    {
        GameObject[] all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < all.Length; ++i)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(all[i]);
        }
        Debug.Log("�V�[������Missing�ȃX�N���v�g�̍폜���������܂���");
    }
    // �v���n�u�Ɋ܂܂��Missing�ȃX�N���v�g���폜����
    [MenuItem("Project/MissingDelete/Prefab", false, 3)]
    public static void DeleteInPrefab()
    {
        string[] allGUID = AssetDatabase.FindAssets("t:prefab");
        for (int i = 0; i < allGUID.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(allGUID[i]);
            GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g);
        }
        AssetDatabase.Refresh();
        Debug.Log("�S�v���n�u����Missing�ȃX�N���v�g�̍폜���������܂���");
    }
}
#endif
