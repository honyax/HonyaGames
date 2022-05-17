using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    private Transform _cachedTransform;
    private Transform _blockPosition;
    private GameObject _block;

    private void Start()
    {
        _cachedTransform = transform;
        _blockPosition = _cachedTransform.Find("BlockPosition");
        _block = _blockPosition.Find("Cell2").gameObject;
        var mat = _block.GetComponent<MeshRenderer>().material;
        var color = mat.color;
        color.a = 0.1f;
        mat.color = color;
        _block.SetActive(false);
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        var pos = _cachedTransform.position;
        var blockPos = Vector3Int.CeilToInt(pos);
        _blockPosition.position = blockPos;
        _blockPosition.rotation = Quaternion.identity;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PhotonController.Instance.photonView.RPC(nameof(PhotonController.CreateBlockRequest),
                RpcTarget.MasterClient, blockPos.x, blockPos.y, blockPos.z, Game.Instance.CurrentBlockIndex);
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            PhotonController.Instance.photonView.RPC(nameof(PhotonController.DeleteBlockRequest),
                RpcTarget.MasterClient, blockPos.x, blockPos.y, blockPos.z);
        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            Game.Instance.ChangeBlockIndex(true);
        }
        else if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            Game.Instance.ChangeBlockIndex(false);
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            _block.SetActive(!_block.activeSelf);
        }
    }
}
