using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class EnemyBase : NetworkBehaviour
{
    [Header("EnemySelectPlane")]
    public GameObject plane1;
    public GameObject plane2;
    [Header("EnemyState")]
    public float ATK = 0;
    public float HP = 100;
    public float DEF = 10;
    [Header("EnemyPendant")]
    public Slider HPBar;

    [SyncVar(hook =nameof(HPBarUpdate))]
    public float HPNow;
    // Start is called before the first frame update
    public virtual void Start()
    {
        HPNow = HP;
        HPBar.value = HPNow / HP;
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public void BeChoose()
    {
        plane1.SetActive(false);
        plane2.SetActive(true);
    }
    public void DonotChoose()
    {
        plane1.SetActive(true);
        plane2.SetActive(false);
    }

    public virtual void GetHit(float damage, GameObject hitGO)
    {
        HPNow = Mathf.Max(0, HPNow  - Mathf.Max(0, (damage - DEF)));
        HPBarUpdate(0,0);
    }

    public virtual void HPBarUpdate(float HPold, float HPnow)
    {
        HPBar.value = HPNow / HP;
    }
}
