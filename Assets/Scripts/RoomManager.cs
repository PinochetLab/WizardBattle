using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
	PhotonView PV;
	public static RoomManager Instance;
	int count0 = 0;
	int count1 = 0;
	public int team = 0;

	void Awake()
	{
		PV = GetComponent<PhotonView>();

		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if(scene.buildIndex == 1) // We're in the game scene
		{
		//	int team = 0;

			

			GameObject playerMan = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
		//	playerMan.GetComponent<PlayerManager>().team = 5;


		//	PV.RPC("PrintMessage", RpcTarget.All, "Player is " + PhotonNetwork.NickName + "; team is " + _team + 
		//		"; count0 is " + count0 + " ; count1 is " + count1);
		}
	}

	[PunRPC]
	void PrintMessage(string message)
    {
	//	print(message);
    }
	
}