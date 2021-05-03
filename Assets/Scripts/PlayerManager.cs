using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{
	[HideInInspector] public PhotonView PV;

	public int team = 0;
	public int place = -1;
	public int kills;
	public float damage;


	GameObject controller;
	//SpawnSystem spawnSystem;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	void Start()
	{
		if (PV.IsMine)
		{
			
			CreateController();
		}
	}
	


	void CreateController()
	{
	//	Transform spawn = SpawnSystem.instance.boilers[team].GetSpawn();

		if (place == -1)
        {
			controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"),
			Vector3.zero, Quaternion.identity, 0, new object[] { PV.ViewID, team });
        }
        else
        {
			Transform spawn = GameObject.Find("SpawnSystem").GetComponent<SpawnSystem>().boilers[team].spawns[place];
			controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"),
			spawn.position, spawn.rotation, 0, new object[] { PV.ViewID, team });
			SpawnSystem.PVs.Add(controller.GetComponent<PhotonView>());
			controller.GetComponent<PlayerController>().team = team;

		}
		controller.GetComponent<PlayerController>().team = team;
	}

	public void Die()
    {
		GameObject _controller = controller;
		CreateController();
		PhotonNetwork.Destroy(_controller);
		SpawnSystem.PVs.Remove(_controller.GetComponent<PhotonView>());
		controller.GetComponent<PlayerController>().ChangeMats();
		print("AAAAAAAAAa");
    }

}