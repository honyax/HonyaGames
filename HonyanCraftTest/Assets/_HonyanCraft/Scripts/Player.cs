using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var pos = transform.position;
            var blockPos = Vector3Int.CeilToInt(pos);
            PhotonController.Instance.photonView.RPC(nameof(PhotonController.CreateBlockRequest),
                RpcTarget.MasterClient, blockPos.x, blockPos.y, blockPos.z, Game.Instance.CurrentBlockIndex);
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            var pos = transform.position;
            var blockPos = Vector3Int.CeilToInt(pos);
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
    }
}
