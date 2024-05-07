using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Multiplayer : MonoBehaviourPunCallbacks
{
    public CardManager cardManager;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Servere bağlandı");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobiye girildi");
        PhotonNetwork.JoinOrCreateRoom("Oda", new RoomOptions { MaxPlayers = 4, IsOpen = true, IsVisible = true}, TypedLobby.Default);
        //PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Odaya girildi");
        GameObject bomb = PhotonNetwork.Instantiate("Bomb", Vector3.zero, Quaternion.identity, 0, null);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Odadan çıkıldı");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Hata! Odaya girilemedi");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Hata! Herhangi bir odaya girilemedi.");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Hata! Oda Kurulamadı");
    }
}
