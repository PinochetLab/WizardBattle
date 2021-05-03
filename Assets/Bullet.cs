using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Bullet : MonoBehaviour
{
    public Charm charm;
    int id = 0;


    public void AddCharm(string charName, PlayerController player)
    {
        
      //  charName = "CameraRotateCharm";
        //  Type type = Type.GetType(charName);
        GetComponent<PhotonView>().RPC("SetUp", RpcTarget.All, charName, player.GetComponent<PhotonView>().ViewID);
     //   print(charm);
    }

    [PunRPC]
    void SetUp(string charName, int playerID)
    {
        id = playerID;
        print("id is " + playerID);
        gameObject.AddComponent(Type.GetType(charName));
        charm = (Charm)GetComponent<Charm>();
        if (charm is DamageCharm) ((DamageCharm)charm).player = PhotonView.Find(playerID).GetComponent<PlayerController>();
    }

    void Start()
    {
        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    void Update()
    {
        transform.position += Time.deltaTime * 150 * transform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        print("GROUUUND");
        if (other.gameObject.tag == "PlayerController")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player && player.GetComponent<PhotonView>().ViewID != id)
            {
              //  print("LOLALALA");
                GetComponent<PhotonView>().RPC("CharmImpact", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
                Destroy(gameObject);
            }
        }
        
        if (other.gameObject.tag == "Ground")
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ExplosionPrefab"), transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }


    [PunRPC]
    void CharmImpact(int playerID)
    {
        PhotonView player = PhotonView.Find(playerID).GetComponent<PhotonView>();
        PlayerController playerC = player.GetComponent<PlayerController>();
        print("charm is " + charm + " ; playerC is " + playerC);
        if (player && player.IsMine)
        {
            if (playerC && charm) charm.Impact(player.GetComponent<PlayerController>());
        }
    }
}
