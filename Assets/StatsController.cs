using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class StatsController : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    public static StatsController instance;

    [SerializeField] GameObject stats;
    [SerializeField] Transform enemyStats;
    [SerializeField] Transform friendStats;
    [SerializeField] GameObject statsItemPrefab;
    List<PlayerManager> managers = new List<PlayerManager>();
    List<PlayerController> controllers = new List<PlayerController>();


    public void SetAll()
    {
        PV.RPC("SetAllRPC", RpcTarget.All);
        
    }

    [PunRPC]
    public void SetAllRPC()
    {
        foreach (PlayerController controller in GameObject.FindObjectsOfType(typeof(PlayerController)) as PlayerController[])
        {
            controllers.Add(controller);
            managers.Add(controller.playerManager);
        }
    }


    [PunRPC]
    void Update()
    {
        if (true)
        {
            if (Input.GetButton("Tab"))
            {
                stats.SetActive(true);

                foreach (Transform child in enemyStats)
                {
                    Destroy(child.gameObject);
                }
                foreach (Transform child in friendStats)
                {
                    Destroy(child.gameObject);
                }

                int realTeam = 0;

                for (int i = 0; i < managers.Count; i++)
                {
                    if (managers[i].PV.IsMine)
                    {
                        realTeam = managers[i].team;
                        break;
                    }
                }

                for (int i = 0; i < managers.Count; i++)
                {
                    bool friend = managers[i].team == realTeam;
                    Transform par = null;
                    if (friend) par = friendStats;
                    else par = enemyStats;
                    StatsItem statsItem = Instantiate(statsItemPrefab, par).GetComponent<StatsItem>();
                    statsItem.SetUp(managers[i].PV.Owner.NickName, managers[i].kills, managers[i].damage);
                    if (managers[i].PV.IsMine)
                    {
                        statsItem.GetComponent<Image>().color = Color.yellow;
                    }
                }
            }
            else
            {
                stats.SetActive(false);
            }
        }
    }
}
