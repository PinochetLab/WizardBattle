using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Boiler : MonoBehaviourPunCallbacks
{
    public List<Transform> spawns = new List<Transform>();
    [HideInInspector] public List<bool> spawnsAv = new List<bool>();
    [SerializeField] int team;
    PhotonView PV;
    public SpawnSystem spawnSystem;

    void Awake()
    {
        PV = GetComponent<PhotonView>();

        for (int i = 0; i < spawns.Count; i++)
        {
            spawns[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < spawns.Count; i++)
        {
            spawnsAv.Add(true);
        }
    }

    public Transform GetSpawn()
    {
      //  return spawns[Random.Range(0, spawns.Count - 1)];
        for (int i = 0; i < spawns.Count; i++)
        {
            if (spawnsAv[i])
            {
                PV.RPC("DisableCheckPoint", RpcTarget.All, i);
                return spawns[i];
            }
        }
        return null;
    }

    [PunRPC]
    void DisableCheckPoint(int i)
    {
        spawnsAv[i] = false;
    }
}
