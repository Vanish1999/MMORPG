using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 yNow;
    public Vector3 yTarget;
    void Start()
    {
        if(player != null)
        {
            yTarget = player.transform.position;
            yNow = yTarget;
        }
        
    }

    void Update()
    {
        if (player != null)
        {
            yTarget = player.transform.position;

            yNow.x = Mathf.Lerp(yNow.x, yTarget.x, 0.1f);
            yNow.y = Mathf.Lerp(yNow.y, yTarget.y, 0.1f);
            yNow.z = Mathf.Lerp(yNow.z, yTarget.z, 0.1f);

            transform.position = yNow;
            
        }
    }
}
