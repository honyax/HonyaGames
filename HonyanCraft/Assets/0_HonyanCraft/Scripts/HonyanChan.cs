using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class HonyanChan : MonoBehaviourPun
{
    private Transform _mainCamera;
    private Transform _crosshair;
    private int _blockLayer;
    private int _hitLayerMask;

    private void Start()
    {
        _mainCamera = Camera.main.transform;
        _crosshair = _mainCamera.Find("crosshair");
        var floorLayerIndex = LayerMask.NameToLayer("Floor");
        var blockLayerIndex = LayerMask.NameToLayer("Block");
        _hitLayerMask = (1 << floorLayerIndex) | (1 << blockLayerIndex);
        _blockLayer = blockLayerIndex;
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
            Game.Instance.CreateBlock(blockPosX, blockPosY, blockPosZ);
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
                    Game.Instance.DeleteBlock(blockPosX, blockPosY, blockPosZ);
                }
            }

        }

        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            Game.Instance.DecrementBlockIndex();
        }
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            Game.Instance.IncrementBlockIndex();
        }
    }
}
