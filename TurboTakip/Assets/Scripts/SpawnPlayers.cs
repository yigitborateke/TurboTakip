using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    private void Start()
    {
        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        Vector3 pos = GameObject.Find("Group 1").transform.position;
        Quaternion rot = new Quaternion();
        if (id == 1)
        {
            rot = new Quaternion(0.0f,0.1594613939523697f,0.0f,0.9872042536735535f);
        }
        else if (id == 2)
        {
            rot = new Quaternion(0.0f,0.1594613939523697f,0.0f,0.9872042536735535f);
        }
        GameObject p = PhotonNetwork.Instantiate(id.ToString(), pos, rot);
    }
}
