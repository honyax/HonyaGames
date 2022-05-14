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
    private void BootRequest(PhotonMessageInfo info)
    {
        Debug.Log($"BootRequest {info.Sender.NickName}");
        photonView.RPC(nameof(BootResponse), info.Sender);
    }

    [PunRPC]
    private void BootResponse(PhotonMessageInfo info)
    {
        Debug.Log($"BootResponse {info.Sender.NickName}");
    }
}
