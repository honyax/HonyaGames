using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : SingletonMonoBehaviour<Game>
{
    [SerializeField]
    private GameObject _playerPrefab;
    public GameObject PlayerPrefab { get { return _playerPrefab; } }

    [SerializeField]
    private GameObject[] _blockPrefabs;

    private Dictionary<Vector3Int, int> _masterBlockDict = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, GameObject> _blockDict = new Dictionary<Vector3Int, GameObject>();

    public void TryCreateBlock(Vector3Int pos, int blockId)
    {
        if (_masterBlockDict.TryAdd(pos, blockId))
        {
            PhotonController.Instance.photonView.RPC(nameof(PhotonController.CreateBlockResponse),
                RpcTarget.All, pos.x, pos.y, pos.z, blockId);
        }
    }

    public void CreateBlockExec(Vector3Int pos, int blockId)
    {
        var blockObj = Instantiate(_blockPrefabs[blockId], Vector3.zero, Quaternion.identity);
        blockObj.transform.position = new Vector3(pos.x, pos.y, pos.z);
        _blockDict.Add(pos, blockObj);
    }

    public void TryDeleteBlock(Vector3Int pos)
    {
        if (_masterBlockDict.Remove(pos))
        {
            PhotonController.Instance.photonView.RPC(nameof(PhotonController.DeleteBlockResponse),
                RpcTarget.All, pos.x, pos.y, pos.z);
        }
    }

    public void DeleteBlockExec(Vector3Int pos)
    {
        if (_blockDict.TryGetValue(pos, out var go))
        {
            Destroy(go);
            _blockDict.Remove(pos);
        }
    }
}
