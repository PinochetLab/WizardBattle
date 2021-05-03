using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.IO;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    Rigidbody rb;
    [HideInInspector] public PhotonView PV;

    public int team = 0;
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;

    [SerializeField] Item[] items;
    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    const float maxHealth = 100;
    public float currentHealth = maxHealth;

    Text hpText;

    [SerializeField] Animator stickAnim;

    [SerializeField] Material blueBodyMat;
    [SerializeField] Material blueHatMat;
    [SerializeField] Material redBodyMat;
    [SerializeField] Material redHatMat;
    [SerializeField] MeshRenderer bodyMesh;
    [SerializeField] MeshRenderer hatMesh1;
    [SerializeField] MeshRenderer hatMesh2;
    [SerializeField] TextMesh nickNameText;
    [SerializeField] Transform stickEnd;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform headTransform;
    [SerializeField] LayerMask raycastBlock;
    [SerializeField] CharmItem[] charmItems;
    CharmItem currentCharmItem;
    Image charmImage;
    Slider charmLoad;
    float loadingTime = 0;
    bool canShoot = false;

 

    public PlayerManager playerManager;

    public void SetCamAngle(float angle)
    {
        //   headTransform.localEulerAngles = new Vector3(0, 0, angle);
        StartCoroutine(CamAngle(angle));
    }

    

    IEnumerator CamAngle(float angle)
    {
        float speed = 100f;
        float startAngle = headTransform.localEulerAngles.z;
        if (startAngle > 180) startAngle = startAngle - 360;
        if (startAngle < -180) startAngle = startAngle + 360;
        if (angle > 180) angle = angle - 360;
        if (angle < -180) angle = angle + 360;
        float deltaAngle = angle - startAngle;
        float time = Mathf.Abs(deltaAngle / speed);
        int n = (int)(time / 0.01f);
        float dt = time / n;
        float dAngle = deltaAngle / n;
        for (int i = 0; i < n; i++)
        {
            headTransform.localEulerAngles += new Vector3(0, 0, dAngle);
            yield return new WaitForSeconds(dt);
        }
    }

    void EquipItem(int _itemIndex)
    {
        if (_itemIndex == previousItemIndex) return;
        itemIndex = _itemIndex;
        items[itemIndex].itemGameObject.SetActive(true);
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;
        
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
          //  hash.Add("Position", transform.position);
          //  hash.Add("Rotation", transform.localScale);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void Damage(float damage, PlayerController other)
    {
         PV.RPC("RPC_Damage", RpcTarget.All, PV.ViewID, damage, other.PV.ViewID);
       // currentHealth -= damage;
    }

    

    [PunRPC]
    void RPC_Damage(int playerID, float damage, int otherID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerController");
        GameObject player = PhotonView.Find(playerID).gameObject;

        PlayerController other = PhotonView.Find(otherID).GetComponent<PlayerController>();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == player)
            {
                players[i].GetComponent<PlayerController>().TakeDamage(damage);
                if (players[i].GetComponent<PlayerController>().currentHealth <= 0)
                {
                    other.playerManager.kills++;
                }
                break;
            }
        }

        other.playerManager.damage += damage;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }


    void Die()
    {
        playerManager.Die();
    }

	void Awake()
    {
        charmImage = GameObject.Find("CharmIcon").GetComponent<Image>();
        charmLoad = GameObject.Find("CharmLoad").GetComponent<Slider>();
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        hpText = GameObject.Find("HPText").GetComponent<Text>();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProperties)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProperties["itemIndex"]);
          //  transform.position = (Vector3)changedProperties["Position"];
           // transform.localRotation = (Quaternion)changedProperties["Rotation"];
        }
    }

    void Start()
    {

        

        if (PV.IsMine)
        {
            nickNameText.text = "";
            bodyMesh.enabled = false;
            hatMesh1.enabled = false;
            hatMesh2.enabled = false;
            currentCharmItem = charmItems[Random.Range(0, charmItems.Length)];
           // EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    public void ChangeMats()
    {
        StartCoroutine(ChangeMatsCor());
    }

    IEnumerator ChangeMatsCor()
    {
        yield return new WaitForSeconds(1f);
        PV.RPC("MaterialsSetUp2", RpcTarget.All, team, PV.ViewID);
    }

    [PunRPC]
    void MaterialsSetUp2(int _team, int playerID)
    {
        PhotonView.Find(playerID).GetComponent<PlayerController>().TeamSetUp(team != _team);
    }

    public void TeamSetUp(bool te)
    {

        if (te)
        {
            bodyMesh.material = blueBodyMat;
            hatMesh1.material = blueHatMat;
            hatMesh2.material = blueHatMat;
        }
        else
        {
            bodyMesh.material = redBodyMat;
            hatMesh1.material = redHatMat;
            hatMesh2.material = redHatMat;
        }
    }

    void RotateTowardsOther()
    {
        List<PhotonView> PVs = SpawnSystem.PVs;

        for (int i = 0; i < PVs.Count; i++)
        {
            if (!PVs[i]) continue;
            if (PVs[i].Owner != PhotonNetwork.LocalPlayer)
            {
                PlayerController player = PVs[i].GetComponent<PlayerController>();
                player.nickNameText.transform.forward = (cameraHolder.transform.position - player.nickNameText.transform.position).normalized;
            }
        }
    }

    void CharmLogic()
    {
        if (!canShoot)
        {
            loadingTime += Time.deltaTime;
        }
        if (loadingTime > currentCharmItem.loadTime)
        {
            canShoot = true;
        }
    }

    void CharmImageUpdate()
    {
        charmImage.sprite = currentCharmItem.icon;
        charmLoad.value = loadingTime / currentCharmItem.loadTime;
        if (canShoot) charmImage.color = Color.white;
        else charmImage.color = Color.grey;
    }

    void Update()
    {
        if (PV.IsMine) hpText.text = currentHealth.ToString();
        nickNameText.text = PV.Owner.NickName + currentHealth.ToString() + "/" + maxHealth.ToString();

        RotateTowardsOther();

      //  transform.localPosition = Vector3.zero;
      //  transform.localRotation = Quaternion.identity;

        if (!PV.IsMine) return;
        Look();

        Move();
        Jump();

        CharmLogic();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        CharmImageUpdate();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
             //   EquipItem(i);
            }
        }


        print(Input.GetAxisRaw("Mouse ScrollWheel"));
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            int next = itemIndex + 1;
            if (next > items.Length - 1) next = 0;
            EquipItem(next);
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            int next = itemIndex - 1;
            if (next < 0) next = items.Length - 1;
            EquipItem(next);
        }

        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            //  print("Laalal");
            Shoot();
            stickAnim.SetTrigger("Fire");

            canShoot = false;
            loadingTime = 0;
            currentCharmItem = charmItems[Random.Range(0, charmItems.Length)];
            //  stickAnim.ResetTrigger("Fire");

        }
    }

    void Shoot()
    {
        Physics.Raycast(headTransform.position, headTransform.forward, out RaycastHit hit, 50, raycastBlock);
        Vector3 target = headTransform.position + headTransform.forward * 50;
        if (hit.collider) target = hit.point;
     //   print("targetLegth is " + (target - cameraHolder.transform.position).magnitude);
        Vector3 origin = stickEnd.position;
        Vector3 dir = (target - origin).normalized;

      //  Debug.DrawRay(cameraHolder.transform.position, cameraHolder.transform.position + cameraHolder.transform.forward * 50f);

        stickEnd.forward = dir;

        GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), origin, stickEnd.rotation);
        bullet.GetComponent<Bullet>().AddCharm(currentCharmItem.name, this);
    }

    public void SetStartPos(Transform par)
    {
       transform.parent = par;
       transform.position = par.position;
       transform.rotation = par.rotation;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        print(grounded);
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += mouseSensitivity * Input.GetAxisRaw("Mouse Y");
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine) return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * smoothTime * Time.fixedDeltaTime);
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

}