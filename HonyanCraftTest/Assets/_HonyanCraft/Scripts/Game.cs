using UnityEngine;

public class Game : SingletonMonoBehaviour<Game>
{
    [SerializeField]
    private GameObject _playerPrefab;
    public GameObject PlayerPrefab { get { return _playerPrefab; } }

    [SerializeField]
    private GameObject[] _blockPrefabs;

    public void CreateBlock(int x, int y, int z)
    {
        var blockObj = Instantiate(_blockPrefabs[0], Vector3.zero, Quaternion.identity);
        blockObj.transform.position = new Vector3(x, y, z);
    }
}
