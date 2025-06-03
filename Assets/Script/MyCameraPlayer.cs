using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraPlayer : MonoBehaviour
{
    public GameObject player;
    public float yNow;
    public float yTarget;
    void Start()
    {
        yTarget= player.transform.position.y;
        yNow = yTarget;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            yTarget = player.transform.position.y;
            if (Mathf.Abs(yTarget-yNow)> 3f)
            {
                transform.position = new Vector3(player.transform.position.x,
                yTarget, transform.position.z);
                yNow = yTarget;
            }
            else
            {
                transform.position = new Vector3(player.transform.position.x,
                yNow, transform.position.z);
            }
        }
    }
}
