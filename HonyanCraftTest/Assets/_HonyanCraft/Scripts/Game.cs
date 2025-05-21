using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : SingletonMonoBehaviour<Game>
{
    [SerializeField]
    private GameObject _playerPrefab;
    public GameObject PlayerPrefab { get { return _playerPrefab; } }

    [SerializeField]
    private Transform _sample;

    [SerializeField]
    private ParticleSystem _createBlockEffectPrefab;

    [SerializeField]
    private ParticleSystem _deleteBlockEffectPrefab;

    [SerializeField]
    private AudioSource _seAudioSource;

    [SerializeField]
    private AudioClip _createBlockSe;

    [SerializeField]
    private AudioClip _deleteBlockSe;

    [SerializeField]
    private GameObject[] _blockPrefabs;

    private Dictionary<Vector3Int, int> _masterBlockDict = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, GameObject> _blockDict = new Dictionary<Vector3Int, GameObject>();
    private int _currentBlockIndex = 0;
    public int CurrentBlockIndex { get { return _currentBlockIndex; } }
    private GameObject _sampleBlock;

    private void Start()
    {
        ReplaceSampleBlock(_currentBlockIndex);
    }

    /// <summary>
    /// ブロックのインデックスを一つ減らす
    /// </summary>
    public void DecrementBlockIndex()
    {
        if (_currentBlockIndex > 0)
        {
            _currentBlockIndex--;
            ReplaceSampleBlock(_currentBlockIndex);
        }
    }

    /// <summary>
    /// ブロックのインデックスを一つ増やす
    /// </summary>
    public void IncrementBlockIndex()
    {
        if (_currentBlockIndex < _blockPrefabs.Length - 1)
        {
            _currentBlockIndex++;
            ReplaceSampleBlock(_currentBlockIndex);
        }
    }

    private void ReplaceSampleBlock(int blockIndex)
    {
        if (_sampleBlock != null)
        {
            Destroy(_sampleBlock);
        }
        _sampleBlock = Instantiate(_blockPrefabs[blockIndex], _sample);
        _sampleBlock.transform.localPosition = Vector3.zero;
        _sampleBlock.transform.localRotation = Quaternion.identity;
    }

    public void SendMasterBlocks(Photon.Realtime.Player newPlayer)
    {
        var blocks = new int[_masterBlockDict.Count * 4];
        var index = 0;
        foreach (var block in _masterBlockDict)
        {
            blocks[index * 4 + 0] = block.Key.x;
            blocks[index * 4 + 1] = block.Key.y;
            blocks[index * 4 + 2] = block.Key.z;
            blocks[index * 4 + 3] = block.Value;
            index++;
        }
        PhotonController.Instance.photonView.RPC(nameof(PhotonController.BootResponse),
            newPlayer, blocks);
    }

    public void ReceiveMasterBlocks(int[] blocks)
    {
        var blockCount = blocks.Length / 4;
        for (var i = 0; i < blockCount; i++)
        {
            var pos = new Vector3Int(blocks[i * 4 + 0], blocks[i * 4 + 1], blocks[i * 4 + 2]);
            var blockId = blocks[i * 4 + 3];
            CreateBlockExec(pos, blockId);
        }
    }

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
        var blockPos = new Vector3(pos.x, pos.y, pos.z);
        blockObj.transform.position = blockPos;
        _blockDict.Add(pos, blockObj);
        _masterBlockDict.TryAdd(pos, blockId);

        Instantiate(_createBlockEffectPrefab, blockPos, Quaternion.identity);
        _seAudioSource.PlayOneShot(_createBlockSe);
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
            var blockPos = go.transform.position;
            Destroy(go);
            _blockDict.Remove(pos);
            _masterBlockDict.Remove(pos);

            Instantiate(_deleteBlockEffectPrefab, blockPos, Quaternion.identity);
            _seAudioSource.PlayOneShot(_deleteBlockSe);
        }
    }
}
