using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class Player : NetworkBehaviour
{
    public LayerMask groundLayer;
    public GameObject Mouseposition;
    public NavMeshAgent agent;
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
        Move();
    }

    public void Move()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 pos = RayPosition();
            if (pos != Vector3.zero)
            {
                Instantiate(Mouseposition,pos,Quaternion.identity);
                agent.SetDestination(pos);
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
}
