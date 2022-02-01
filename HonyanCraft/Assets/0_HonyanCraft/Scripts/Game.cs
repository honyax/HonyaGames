using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : MonoBehaviourPun
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
        photonView.RPC(nameof(CreateBlock), RpcTarget.All, blockPosX, blockPosY, blockPosZ, _blockPrefabIndex);
    }

    public void DeleteBlock(int blockPosX, int blockPosY, int blockPosZ)
    {
        photonView.RPC(nameof(DeleteBlock), RpcTarget.All, blockPosX, blockPosY, blockPosZ);
    }

    [PunRPC]
    private void CreateBlock(int blockPosX, int blockPosY, int blockPosZ, int blockPrefabIndex, PhotonMessageInfo info)
    {
        Debug.Log($"CreateBlock {info.Sender.NickName}: ({blockPosX}, {blockPosY}, {blockPosZ})");
        var tuple = Tuple.Create(blockPosX, blockPosY, blockPosZ);

        if (!_blockDict.ContainsKey(tuple))
        {
            if (0 <= blockPrefabIndex && blockPrefabIndex < _blockPrefabs.Length)
            {
                var blockObj = Instantiate(_blockPrefabs[blockPrefabIndex], Vector3.zero, Quaternion.identity);
                var blockPos = new Vector3(blockPosX, blockPosY + 0.5f, blockPosZ);
                blockObj.transform.position = blockPos;
                blockObj.layer = _blockLayer;
                _blockDict[tuple] = blockObj;
            }
        }
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
