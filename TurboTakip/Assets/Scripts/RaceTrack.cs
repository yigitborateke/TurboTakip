using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class RaceTrack : MonoBehaviour
{
    public struct Obstacle
    {
        public int NodePosition;
        public int BlockingRounds;
        public int InstanceID;
    }

    public const byte bombExploded = 3;
    
    Transform[] _childObjects;
    public List<Transform> childNodeList = new List<Transform>();

    public List<Obstacle> nodesWithObstacles = new List<Obstacle>(); 

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    private void Start()
    {
        FillNodes();
        // Obstacle obs;
        // obs.Type = (int)CardCollection.Card.CardType.Bomb;
        // obs.BlockingRounds = 1;
        // obs.OwnerID = 2;
        // obs.NodePosition = 2;
        // nodesWithObstacles.Add(obs);
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     PhotonNetwork.Instantiate("Bomb", childNodeList[2].position, Quaternion.identity);
        // }

    }
    
    void FillNodes()
    {
        childNodeList.Clear();
        _childObjects = GetComponentsInChildren<Transform>();

        foreach(Transform child in _childObjects)
        {
            if(child != this.transform)
            {
                childNodeList.Add(child);
            }
        }
    }
    
    public int IsObstacleOnPath(int trackPosition)
    {
        foreach (var obstacle in nodesWithObstacles)
        {
            if (trackPosition == obstacle.NodePosition)
            {
                Debug.Log("bomb in path expolode");
                // object[] content = new object[] { nodesWithObstacles.IndexOf(obstacle), obstacle.Type, obstacle.NodePosition };
                // RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                // PhotonNetwork.RaiseEvent(bombExploded, content, raiseEventOptions, SendOptions.SendReliable);
                
                object[] content = new object[] {trackPosition, obstacle.InstanceID};
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(bombExploded, content, raiseEventOptions, SendOptions.SendReliable);

                return obstacle.BlockingRounds;
            }
        }
        return 0;
    }


}
