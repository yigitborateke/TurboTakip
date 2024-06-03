using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField RoomNameInput;

    public void CreateRoom()
    {
        if (RoomNameInput.text.IsNullOrEmpty())
        {
            PhotonNetwork.CreateRoom("DefaultRoomName");
        }
        else
        {
            PhotonNetwork.CreateRoom(RoomNameInput.text);
        }
    }

    public void JoinRoom()
    {
        if (RoomNameInput.text.IsNullOrEmpty())
        {
            PhotonNetwork.JoinRoom("DefaultRoomName");
        }
        else
        {
            PhotonNetwork.JoinRoom(RoomNameInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
