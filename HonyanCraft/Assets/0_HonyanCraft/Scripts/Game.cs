using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject[] _blockPrefabs;

    private Dictionary<Tuple<int, int, int>, GameObject> _blockDict = new Dictionary<Tuple<int, int, int>, GameObject>();

    private int _blockPrefabIndex = 0;
    private int _blockLayer;
    private Transform _sample;
    private GameObject _sampleBlock;

    private void Start()
    {
        var mainCamera = Camera.main.transform;
        _sample = mainCamera.Find("sample");
        _sampleBlock = null;
        _blockPrefabIndex = 0;
        var blockLayerIndex = LayerMask.NameToLayer("Block");
        _blockLayer = blockLayerIndex;

        ReplaceSample(_blockPrefabIndex);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        photonView.RPC(nameof(BootRequest), RpcTarget.MasterClient);
    }

    public void DecrementBlockIndex()
    {
        if (_blockPrefabIndex > 0)
        {
            _blockPrefabIndex--;
            ReplaceSample(_blockPrefabIndex);
        }
    }

    public void IncrementBlockIndex()
    {
        if (_blockPrefabIndex < _blockPrefabs.Length - 1)
        {
            _blockPrefabIndex++;
            ReplaceSample(_blockPrefabIndex);
        }
    }

    private void ReplaceSample(int blockPrefabIndex)
    {
        if (0 <= blockPrefabIndex && blockPrefabIndex < _blockPrefabs.Length)
        {
            if (_sampleBlock != null)
            {
                Destroy(_sampleBlock);
            }

            var blockObj = Instantiate(_blockPrefabs[blockPrefabIndex], _sample);
            var t = blockObj.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            _sampleBlock = blockObj;
        }
    }

    public void CreateBlock(int blockPosX, int blockPosY, int blockPosZ)
    {
        photonView.RPC(nameof(CreateBlockRequest), RpcTarget.MasterClient, blockPosX, blockPosY, blockPosZ, _blockPrefabIndex);
    }

    public void DeleteBlock(int blockPosX, int blockPosY, int blockPosZ)
    {
        photonView.RPC(nameof(DeleteBlockRequest), RpcTarget.MasterClient, blockPosX, blockPosY, blockPosZ);
    }

    [PunRPC]
    private void CreateBlock(int blockPosX, int blockPosY, int blockPosZ, int blockPrefabIndex, PhotonMessageInfo info)
    {
        Debug.Log($"CreateBlock {info.Sender.NickName}: ({blockPosX}, {blockPosY}, {blockPosZ})");
        CreateBlockExec(blockPosX, blockPosY, blockPosZ, blockPrefabIndex);
    }
    private void CreateBlockExec(int blockPosX, int blockPosY, int blockPosZ, int blockPrefabIndex)
    {
        var tuple = Tuple.Create(blockPosX, blockPosY, blockPosZ);

        var blockObj = Instantiate(_blockPrefabs[blockPrefabIndex], Vector3.zero, Quaternion.identity);
        var blockPos = new Vector3(blockPosX, blockPosY + 0.5f, blockPosZ);
        blockObj.transform.position = blockPos;
        blockObj.layer = _blockLayer;
        _blockDict[tuple] = blockObj;
    }

    [PunRPC]
    private void DeleteBlock(int blockPosX, int blockPosY, int blockPosZ, PhotonMessageInfo info)
    {
        Debug.Log($"DeleteBlock {info.Sender.NickName}: ({blockPosX}, {blockPosY}, {blockPosZ})");
        var tuple = Tuple.Create(blockPosX, blockPosY, blockPosZ);

        GameObject blockObj;
        if (_blockDict.TryGetValue(tuple, out blockObj))
        {
            Destroy(blockObj);
            _blockDict.Remove(tuple);
        }
    }

    private Dictionary<Tuple<int, int, int>, int> _masterBlockDict = new Dictionary<Tuple<int, int, int>, int>();

    [PunRPC]
    private void CreateBlockRequest(int blockPosX, int blockPosY, int blockPosZ, int blockPrefabIndex, PhotonMessageInfo info)
    {
        Debug.Log($"CreateBlockRequest {info.Sender.NickName}: ({blockPosX}, {blockPosY}, {blockPosZ})");
        var tuple = Tuple.Create(blockPosX, blockPosY, blockPosZ);

        if (!_masterBlockDict.ContainsKey(tuple))
        {
            if (0 <= blockPrefabIndex && blockPrefabIndex < _blockPrefabs.Length)
            {
                // ブロック作成OK
                _masterBlockDict[tuple] = blockPrefabIndex;
                photonView.RPC(nameof(CreateBlock), RpcTarget.All, blockPosX, blockPosY, blockPosZ, blockPrefabIndex);
            }
        }
    }

    [PunRPC]
    private void DeleteBlockRequest(int blockPosX, int blockPosY, int blockPosZ, PhotonMessageInfo info)
    {
        Debug.Log($"DeleteBlockRequest {info.Sender.NickName}: ({blockPosX}, {blockPosY}, {blockPosZ})");
        var tuple = Tuple.Create(blockPosX, blockPosY, blockPosZ);

        if (_masterBlockDict.ContainsKey(tuple))
        {
            _masterBlockDict.Remove(tuple);
            photonView.RPC(nameof(DeleteBlock), RpcTarget.All, blockPosX, blockPosY, blockPosZ);
        }
    }

    [PunRPC]
    private void BootRequest(PhotonMessageInfo info)
    {
        Debug.Log($"BootRequest {info.Sender.NickName}");
        var parameter = new int[_masterBlockDict.Count * 4];
        var index = 0;
        foreach (var masterBlock in _masterBlockDict)
        {
            var tuple = masterBlock.Key;
            var block = masterBlock.Value;
            parameter[index * 4 + 0] = tuple.Item1;
            parameter[index * 4 + 1] = tuple.Item2;
            parameter[index * 4 + 2] = tuple.Item3;
            parameter[index * 4 + 3] = block;
            index++;
        }
        photonView.RPC(nameof(BootResponse), info.Sender, parameter);
    }

    [PunRPC]
    private void BootResponse(int[] intArray, PhotonMessageInfo info)
    {
        Debug.Log($"BootResponse {info.Sender.NickName}:{intArray.Length / 4}");
        var count = intArray.Length / 4;
        for (var i = 0; i < count; i++)
        {
            var blockPosX = intArray[i * 4 + 0];
            var blockPosY = intArray[i * 4 + 1];
            var blockPosZ = intArray[i * 4 + 2];
            var blockPrefabIndex = intArray[i * 4 + 3];
            CreateBlockExec(blockPosX, blockPosY, blockPosZ, blockPrefabIndex);
        }
    }

    #region Singleton
    private static Game instance;
    public static Game Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(Game);

                instance = (Game)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }
            }

            return instance;
        }
    }

    virtual protected void Awake()
    {
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する。
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this;
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }
        Destroy(this);
        return false;
    }
    #endregion
}
