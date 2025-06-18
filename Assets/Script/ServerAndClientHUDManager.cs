using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerAndClientHUDManager : NetworkBehaviour
{
    public GameObject SliderAndSkill;
    public GameObject PlayerNameAndShot;
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (isServerOnly)
        {
            SliderAndSkill.SetActive(false);
            PlayerNameAndShot.SetActive(false);
        }
    }
}
