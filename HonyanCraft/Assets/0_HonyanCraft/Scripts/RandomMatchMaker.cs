using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _unityChanPrefab;

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
            _unityChanPrefab.name,
            new Vector3(0, 1, 0),
            Quaternion.identity,
            0
            );
        obj.name = "HonyanCraftChan";

        var virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        var playerCameraRoot = obj.transform.Find("PlayerCameraRoot");
        virtualCamera.Follow = playerCameraRoot;
        virtualCamera.LookAt = obj.transform;
    }
}
