using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class Player : NetworkBehaviour
{
    [Header("Move")]
    public LayerMask groundLayer;
    public GameObject Mouseposition;
    public NavMeshAgent agent;
    [Header("Anim")]
    public Animator animatorPlayer;
    [Header("Plane")]
    public GameObject plane1;
    public GameObject plane2;
    [Header("Attack")]
    public LayerMask attackLayer;
    private bool isAttack = false;
    public Transform attackTransform1;
    public GameObject attackBox;
    [Header("PlayerState")]
    public float ATK = 100;

    private GameObject tarNow;
    private GameObject tarAttack;
    

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Camera.main.transform.parent.GetComponent<MyCameraPlayer>().player = gameObject;
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            SetPlane();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            Move();
            Attack();
        }
    }

    public void SetPlane()
    {
        plane1.SetActive(true); 
        plane2.SetActive(false);
    }

    public void Move()
    {
        if (isAttack)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 pos = RayPosition();
            if (pos != Vector3.zero)
            {
                if(tarNow != null)
                    {
                        Destroy(tarNow);
                    }
                tarNow = Instantiate(Mouseposition,pos,Quaternion.identity);
                agent.SetDestination(pos);
                CmdSetAnimBool("IsRun", true);
            }
        }

        if (tarNow != null)
        {
            if(Vector3.Distance(transform.position, tarNow.transform.position) <= 1.0f)
            {
                Destroy (tarNow);
                CmdSetAnimBool("IsRun", false);
            }
        }
    }

    public  void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject tar = RayAttack();
            if (tar != null)
            {
                if (tarAttack != null)
                {
                    tarAttack.GetComponent<EnemyBase>().DonotChoose();
                }
                tarAttack = tar;
                tarAttack.GetComponent<EnemyBase>().BeChoose();

                if (Vector3.Distance(transform.position,tar.transform.position) <= 2.0f)
                {
                    transform.rotation = Quaternion.LookRotation(tar.transform.position - transform.position);
                    isAttack = true;
                    CmdSetAnimTrigger("Attack");
                    Invoke("CmdCreatAttackBox", 0.7f);
                    Invoke("RestisAttack", 1.0f);
                }
            }
        }

    }

    public Vector3 RayPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 1000.0f, groundLayer);
        if(hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "NotMoveLayer")
            {
                return Vector3.zero;
            }
            else
            {
                return hit.point;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }

    public GameObject RayAttack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 1000.0f, attackLayer);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }

    }

    [Command]
    public void CmdCreatAttackBox()
    {
        GameObject go = Instantiate(attackBox, attackTransform1.position, attackTransform1.rotation);
        PlayerAttackBox attackBoxScript = go.GetComponent<PlayerAttackBox>();
        attackBoxScript.player = gameObject;
        attackBoxScript.damage = ATK;
    }


    [Command]
    public void CmdSetAnimBool(string name, bool value)
    {
        RpcSetAnimBool(name, value);
        animatorPlayer.SetBool(name, value);
    }

    [ClientRpc]
    public void RpcSetAnimBool(string name, bool value)
    {
        animatorPlayer.SetBool(name, value);
    }
    [Command]
    public void CmdSetAnimTrigger(string name)
    {
        RpcSetAnimTrigger(name);
        animatorPlayer.SetTrigger(name);
    }

    [ClientRpc]
    public void RpcSetAnimTrigger(string name)
    {
        animatorPlayer.SetTrigger(name);
    }

    public void RestisAttack()
    {
        isAttack = false;
    }
}
