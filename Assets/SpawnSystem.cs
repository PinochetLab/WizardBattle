using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class SpawnSystem : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    public static SpawnSystem instance;

    public static List<PhotonView> PVs = new List<PhotonView>();
    public static List<PlayerController> players = new List<PlayerController>();

    [SerializeField] Material redMat;
    [SerializeField] Material blueMat;
    [SerializeField] StatsController statsController;

    public Boiler[] boilers;
    public int count0 = 0;
    public int count1 = 0;
    bool st = true;

    void Awake()
    {
        instance = this;
        PV = GetComponent<PhotonView>();
    }

    public void NewCount(int _count0, int _count1)
    {
      //  PV.RPC("ChangeCount", RpcTarget.All, _count0, _count1);
    }

    [PunRPC]
    void ChangeCount(int _count0, int _count1)
    {
        count0 = _count0;
        count1 = _count1;
    }

    public void UpdatePointsAv(int team, int i)
    {
        PV.RPC("UpdatePoints", RpcTarget.All, team, i);
    }

    [PunRPC]
    void UpdatePoints(int team, int i)
    {
        boilers[team].spawnsAv[i] = false;
    }

    

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(SetUp());
    }

    IEnumerator SetUp()
    {

        while (GameObject.FindGameObjectsWithTag("PlayerController").Length < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            yield return new WaitForSeconds(0.01f);
        }

        if (statsController) statsController.SetAll();

        

        GameObject[] controllers = GameObject.FindGameObjectsWithTag("PlayerController");
        print("Length is " + controllers.Length);
        for (int i = 0; i < controllers.Length; i++)
        {
            //  PlayerController cont = controller.GetComponent<PlayerController>();
            int _team = Random.Range(0, 2);

            if (count0 > count1) _team = 1;
            if (count0 < count1) _team = 0;

            Transform spaw = boilers[_team].GetSpawn();

            int place = boilers[_team].spawns.IndexOf(spaw);

            int index = controllers[i].GetComponent<PhotonView>().ViewID;
            PV.RPC("ControllersSetUp", RpcTarget.All, index, _team, spaw.position, spaw.rotation, place);
          //  print("i: " + i);
         //   yield return new WaitForSeconds(0.02f);
        }
        PV.RPC("MaterialSetUp", RpcTarget.All);
    }

    [PunRPC]
    void CloseStart()
    {
     //   st = false;
    }

    public void ChangeMats()
    {
        PV.RPC("MaterialsSetUp", RpcTarget.All);
    }

    [PunRPC]
    void MaterialSetUp()
    {
        int realTeam = 0;

        foreach (GameObject cont in GameObject.FindGameObjectsWithTag("PlayerController"))
        {
            if (cont.GetComponent<PhotonView>().IsMine)
            {
                realTeam = cont.GetComponent<PlayerController>().team;
            }
        }

        foreach (GameObject cont in GameObject.FindGameObjectsWithTag("PlayerController"))
        {
            int myTeam = cont.GetComponent<PlayerController>().team;
            cont.GetComponent<PlayerController>().TeamSetUp(myTeam == realTeam);
        }
    }

    [PunRPC]
    void ControllersSetUp(int index, int _team, Vector3 pos, Quaternion rot, int place)
    {
        PlayerController controller = PhotonView.Find(index).GetComponent<PlayerController>();
        bool cond = controller.PV.Owner == PhotonNetwork.LocalPlayer;
     //   if (!cond) return;
        
        int team = _team;
        //   if (count0 > count1) team = 1;
        //   if (count0 < count1) team = 0;
        PVs.Add(controller.PV);
        players.Add(controller);

     //   print("count0 is " + count0 + " ; count1 is " + count1 + " ; team is " + team + " ; Player is " + controller.PV.Owner.NickName);

        if (team == 0) count0++;
        if (team == 1) count1++;

        controller.team = team;
        controller.playerManager.place = place;
        controller.playerManager.team = team;
        //  Transform spawn = spaw;

        //   Rigidbody rb = controller.GetComponent<Rigidbody>();

        //  controller.transform.parent = spawn;
        controller.transform.position = pos;
        controller.transform.rotation = rot;

      //  controller.TeamSetUp();

        if (controller.GetComponent<PhotonView>().IsMine)
        {
            for (int i = 0; i < boilers.Length; i++)
            {
                boilers[i].GetComponent<MeshRenderer>().material = redMat;
            }

            boilers[team].GetComponent<MeshRenderer>().material = blueMat;
        }

        //   if (rb) rb.isKinematic = true;

       // Vector3 pos = Vector3.zero;

        //   controller.transform.position = spawn.position;
        //   controller.transform.rotation = spawn.rotation;
      //  controller.GetComponent<PhotonTransformView>().enabled = false;
        



       // controller.GetComponent<PhotonTransformView>().enabled = true;

        //   print("team is " + team + " ; position is " + pos);

        //   if (team == 0) controller.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
