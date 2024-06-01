using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField RoomNameInput;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(RoomNameInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(RoomNameInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("SampleScene");
    }
}
