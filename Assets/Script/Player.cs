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

    private GameObject tarNow;
    // Start is called before the first frame update

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Camera.main.transform.parent.GetComponent<MyCameraPlayer>().player = gameObject;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            Move();
        }
    }

    public void Move()
    {
        if (Input.GetMouseButton(1))
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
}
