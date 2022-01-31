using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class HonyanChan : MonoBehaviourPun
{
    [SerializeField]
    private GameObject[] _blockPrefabs;

    static private Dictionary<Tuple<int, int, int>, GameObject> _blockDict = new Dictionary<Tuple<int, int, int>, GameObject>();

    private int _blockPrefabIndex = 0;
    private Transform _mainCamera;
    private Transform _crosshair;
    private Transform _sample;
    private GameObject _sampleBlock;
    private int _blockLayer;
    private int _hitLayerMask;

    private void Start()
    {
        _mainCamera = Camera.main.transform;
        _crosshair = _mainCamera.Find("crosshair");
        _sample = _mainCamera.Find("sample");
        var floorLayerIndex = LayerMask.NameToLayer("Floor");
        var blockLayerIndex = LayerMask.NameToLayer("Block");
        _hitLayerMask = (1 << floorLayerIndex) | (1 << blockLayerIndex);
        _blockLayer = blockLayerIndex;

        ReplaceSample(_blockPrefabIndex);
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        var dir = (_crosshair.position - _mainCamera.position).normalized;
        var cameraRayLength = 3.0f;
        RaycastHit hit;
        if (Physics.Raycast(_mainCamera.position, dir, out hit, cameraRayLength, _hitLayerMask))
        {
            Debug.DrawLine(_mainCamera.position, hit.point, Color.yellow, 0.03f);
        }
        else
        {
            Debug.DrawLine(_mainCamera.position, _mainCamera.position + dir * cameraRayLength, Color.magenta, 0.03f);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            int blockPosX;
            int blockPosZ;
            int blockPosY;
            if (Physics.Raycast(_mainCamera.position, dir, out hit, cameraRayLength, _hitLayerMask))
            {
                var pointX = hit.point.x < 0 ? hit.point.x - 0.5f : hit.point.x + 0.5f;
                var pointZ = hit.point.z < 0 ? hit.point.z - 0.5f : hit.point.z + 0.5f;
                var pointY = hit.point.y + 0.1f;
                blockPosX = (int)pointX;
                blockPosZ = (int)pointZ;
                blockPosY = (int)pointY;
                Debug.Log($"Block hit point:{hit.point} => {blockPosX}, {blockPosY}, {blockPosZ}");
            }
            else
            {
                var pos = _mainCamera.position + dir * cameraRayLength;
                var pointX = pos.x < 0 ? pos.x - 0.5f : pos.x + 0.5f;
                var pointZ = pos.z < 0 ? pos.z - 0.5f : pos.z + 0.5f;
                var pointY = pos.y + 0.1f;
                blockPosX = (int)pointX;
                blockPosZ = (int)pointZ;
                blockPosY = (int)pointY;
                Debug.Log($"Block no hit pos:{pos} => {blockPosX}, {blockPosY}, {blockPosZ}");
            }
            photonView.RPC(nameof(CreateBlock), RpcTarget.All, blockPosX, blockPosY, blockPosZ, _blockPrefabIndex);
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(_mainCamera.position, dir, out hit, cameraRayLength, _hitLayerMask))
            {
                var t = hit.transform;
                if (t.gameObject.layer == _blockLayer)
                {
                    var blockPosX = (int)t.position.x;
                    var blockPosZ = (int)t.position.z;
                    var blockPosY = (int)(t.position.y - 0.5f);
                    Debug.Log($"Block hit point:{hit.point} => {blockPosX}, {blockPosY}, {blockPosZ}");
                    photonView.RPC(nameof(DeleteBlock), RpcTarget.All, blockPosX, blockPosY, blockPosZ);
                }
            }

        }

        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            if (_blockPrefabIndex > 0)
            {
                _blockPrefabIndex--;
                ReplaceSample(_blockPrefabIndex);
            }
        }
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            if (_blockPrefabIndex < _blockPrefabs.Length - 1)
            {
                _blockPrefabIndex++;
                ReplaceSample(_blockPrefabIndex);
            }
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
                var blockPos = new Vector3(blockPosX, blockPosY + 0.5f, blockPosZ) ;
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
}
