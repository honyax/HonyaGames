using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PhotonController : SingletonMonoBehaviourPunCallbacks<PhotonController>
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 8;
        PhotonNetwork.CreateRoom("MyRoom", roomOption);
    }

    public override void OnJoinedRoom()
    {
        var obj = PhotonNetwork.Instantiate(
            Game.Instance.PlayerPrefab.name,
            new Vector3(0, 1, 0),
            Quaternion.identity,
            0
            );

        var virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        var playerCameraRoot = obj.transform.Find("PlayerCameraRoot");
        virtualCamera.Follow = playerCameraRoot;
        virtualCamera.LookAt = obj.transform;
        obj.name = "MyPlayerArmature";
        obj.AddComponent<Player>();

        photonView.RPC(nameof(BootRequest), RpcTarget.MasterClient);
    }

    [PunRPC]
    public void BootRequest(PhotonMessageInfo info)
    {
        Debug.Log($"BootRequest {info.Sender.NickName}");
        Game.Instance.SendMasterBlocks(info.Sender);
    }

    [PunRPC]
    public void BootResponse(int[] blocks, PhotonMessageInfo info)
    {
        Debug.Log($"BootResponse {info.Sender.NickName}");
        Game.Instance.ReceiveMasterBlocks(blocks);
    }

    [PunRPC]
    public void CreateBlockRequest(int x, int y, int z, int blockId)
    {
        Game.Instance.TryCreateBlock(new Vector3Int(x, y, z), blockId);
    }

    [PunRPC]
    public void CreateBlockResponse(int x, int y, int z, int blockId)
    {
        Game.Instance.CreateBlockExec(new Vector3Int(x, y, z), blockId);
    }

    [PunRPC]
    public void DeleteBlockRequest(int x, int y, int z)
    {
        Game.Instance.TryDeleteBlock(new Vector3Int(x, y, z));
    }

    [PunRPC]
    public void DeleteBlockResponse(int x, int y, int z)
    {
        Game.Instance.DeleteBlockExec(new Vector3Int(x, y, z));
    }
}
