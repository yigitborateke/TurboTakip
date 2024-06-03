using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int TrackPosition;

    private void OnDestroy()
    {
        // if (GetComponent<PhotonView>().IsMine)
        // {
        //     GameObject exp = PhotonNetwork.Instantiate("BigExplosionEffect", transform.position,
        //         Quaternion.identity);
        //     exp.SetActive(true);
        //     StartCoroutine(deleteExplosion(2f, exp));
        // }
        
        GetComponent<AudioSource>().Play();
    }

    public IEnumerator deleteExplosion(float seconds, GameObject exp)
    {
        yield return new WaitForSeconds(seconds);
        PhotonNetwork.Destroy(exp);
    }
}
