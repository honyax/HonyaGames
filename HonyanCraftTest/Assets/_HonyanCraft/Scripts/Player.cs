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
            Game.Instance.CreateBlock(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
        }
    }
}
