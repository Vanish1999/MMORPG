using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Mirror;
using Unity.VisualScripting;

public enum EnemyType
{
    Idle,
    Move,
    Attack,
    Dead
}

public class EnemyBase : NetworkBehaviour
{
    [Header("EnemySelectPlane")]
    public GameObject plane1;
    public GameObject plane2;
    [Header("EnemyState")]
    public float ATK = 0;
    public float HP = 100;
    public float DEF = 10;
    public bool CanMoveAndAttack = false;
    [Header("EnemyPendant")]
    public Slider HPBar;
    [Header("EnemyMove")]
    public GameObject target;
    public NavMeshAgent agent;
    public Animator animatorEnemy;
    public bool hasTarget = false;
    [Header("EnemyState")]
    public EnemyType enemyType = EnemyType.Idle;
    public float AttackDistance = 1.5f;
    public float TrackingDistance = 20.0f;
    [Header("EnemyAttack")]
    public bool doAttack = false;
    public float AttackCoolTime = 1.0f;
    public float AttackDamageTime = 0.5f;
    public GameObject attackBox;
    public Transform attackTransform1;

    [SyncVar(hook =nameof(HPBarUpdate))]
    public float HPNow;
    // Start is called before the first frame update
    public virtual void Start()
    {
        HPNow = HP;
        HPBar.value = HPNow / HP;

        if (isServer)
        {
            if (CanMoveAndAttack)
            {
                EnemyTypeIdleStart();
            }
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (CanMoveAndAttack)
        {
            EnemyStateFunction();
        }

    }

    public virtual void EnemyStateFunction()
    {
        switch (enemyType)
        {
            case EnemyType.Idle:
                {
                    EnemyTypeIdleUpdate();
                }
                break;
            case EnemyType.Move:
                {
                    EnemyTypeMoveUpdate();
                }
                break;
            case EnemyType.Attack:
                {
                    EnemyTypeAttackUpdate();
                }
                break;
            case EnemyType.Dead:
                {
                    EnemyTypeDeadUpdate();
                }
                break;
        }
    }
    //Idle
    public virtual void EnemyTypeIdleStart()
    {
        CmdSetAnimBool("IsRun",false);
    }
    public virtual void EnemyTypeIdleUpdate()
    {
        if (hasTarget)
        {
            SwitchState(EnemyType.Move);
        }
    }
    public virtual void EnemyTypeIdleEnd()
    {
        
    }
    //Move
    public virtual void EnemyTypeMoveStart()
    {
        CmdSetAnimBool("IsRun", true);
    }
    public virtual void EnemyTypeMoveUpdate()
    {
        agent.SetDestination(target.transform.position);

        if(Vector3.Distance(transform.position, target.transform.position) <= AttackDistance)
        {
            SwitchState(EnemyType.Attack);
        }
        else if(Vector3.Distance(transform.position, target.transform.position) >= TrackingDistance)
        {
            SwitchState(EnemyType.Idle);
            agent.SetDestination(transform.position);
            HPNow = HP;
            target = null;
            hasTarget = false;
        }
    }
    public virtual void EnemyTypeMoveEnd()
    {
        CmdSetAnimBool("IsRun", false);
    }
    //Attack
    public virtual void EnemyTypeAttackStart()
    {
       
    }
    public virtual void EnemyTypeAttackUpdate()
    {
        if (!doAttack && (Vector3.Distance(transform.position, target.transform.position) <= AttackDistance))
        {
            doAttack = true;
            CmdSetAnimTrigger("Attack");

            transform.rotation = Quaternion.Euler(transform.rotation.x,
                (Quaternion.LookRotation(target.transform.position - transform.position)).eulerAngles.y, 
                transform.rotation.z);

            Invoke("CreatAttackBox", AttackDamageTime);

            Invoke("RestDoAttack", AttackCoolTime);
        } 
        else if (!doAttack && Vector3.Distance(transform.position,target.transform.position)> AttackDistance)
        {
            SwitchState(EnemyType.Idle);
        }
    }
    public virtual void EnemyTypeAttackEnd()
    {

    }
    //Dead
    public virtual void EnemyTypeDeadStart()
    {

    }
    public virtual void EnemyTypeDeadUpdate()
    {

    }
    public virtual void EnemyTypeDeadEnd()
    {

    }

    public virtual void SwitchState(EnemyType newenemyType)
    {
        //执行上一个状态结束函数
        EnemyType oldenemyType = enemyType;
        switch (oldenemyType)
        {
            case EnemyType.Idle:
                { EnemyTypeIdleEnd(); }
                break;
            case EnemyType.Move:
                { EnemyTypeMoveEnd(); }
                break;
            case EnemyType.Attack:
                { EnemyTypeAttackEnd(); }
                break;
            case EnemyType.Dead:
                {  EnemyTypeDeadEnd(); }
                break;
        }
        //执行新状态开始函数
        enemyType = newenemyType;
        switch (enemyType)
        {
            case EnemyType.Idle:
                { EnemyTypeIdleStart(); }
                break;
            case EnemyType.Move:
                { EnemyTypeMoveStart(); }
                break;
            case EnemyType.Attack:
                { EnemyTypeAttackEnd(); }
                break;
            case EnemyType.Dead:
                { EnemyTypeDeadStart(); }
                break;
        }
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

    public void RestDoAttack()
    {
        doAttack = false;
    }

    public virtual void GetHit(float damage, GameObject hitGO)
    {
        HPNow = Mathf.Max(0, HPNow  - Mathf.Max(0, (damage - DEF)));
        HPBarUpdate(0,0);
        if (hitGO.GetComponent<Player>())
        {
            target = hitGO;
            hasTarget = true;
        }
    }

    public virtual void HPBarUpdate(float HPold, float HPnow)
    {
        HPBar.value = HPNow / HP;
    }

    public virtual void Move()
    {
        if(target != null)
        {
            agent.SetDestination(target.transform.position);
            CmdSetAnimBool("IsRun", true);
        }
    }

    public virtual void CreatAttackBox()
    {
        GameObject go = Instantiate(attackBox, attackTransform1.position, attackTransform1.rotation);
        EnemyAttackBox attackBoxScript = go.GetComponent<EnemyAttackBox>();
        attackBoxScript.enemy = gameObject;
        attackBoxScript.damage = ATK;
    }

    public void CmdSetAnimBool(string name, bool value)
    {
        RpcSetAnimBool(name, value);
        animatorEnemy.SetBool(name, value);
    }
    [ClientRpc]
    public void RpcSetAnimBool(string name, bool value)
    {
        animatorEnemy.SetBool(name, value);
    }

    public void CmdSetAnimTrigger(string name)
    {
        RpcSetAnimTrigger(name);
        animatorEnemy.SetTrigger(name);
    }
    [ClientRpc]
    public void RpcSetAnimTrigger(string name)
    {
        animatorEnemy.SetTrigger(name);
    }
}
