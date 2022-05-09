using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class Unit : NetworkedObject
{
    [ReadOnly] public int unitID;
    [ReadOnly] public bool isMine;

    [Header("Health/Damage")]
    public bool dead;
    public int health = 100;
    public int damage = 10;

    [Header("ViewAngle")]
    [SerializeField] public float viewAngle = 70;
    [SerializeField] public float eyeRange = 10;
    
    [Header("Read Only's")]
    [ReadOnly] public int maxPoints;
    [ReadOnly] public int currentPoints;
    [ReadOnly] [SerializeField] bool shootOnSight = true;
    [ReadOnly] [SerializeField] bool lookAtPoint;
    [ReadOnly] public Transform targetTransform;
    [ReadOnly][SerializeField] Unit targetUnit;

    [Header("Assign Manually")]
    [SerializeField] GameObject[] skin;
    [SerializeField] GameObject selectedVFX;
    [SerializeField] ParticleSystem shotVFX;
    [SerializeField] Transform lineOfSight;
    public UnitSight unitSight;
    [SerializeField] Slider healthBar;
    [Space]
    public List<Action> plan = new List<Action>();

    [HideInInspector] public Animator anim;
    NavMeshAgent agent;
    LineRenderer lr;
    private bool breakOutCombat;

    Vector3 lookPoint;
    bool looking;

    public virtual void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;

        maxPoints = GameBalanceManager.instance.maxPoints;
        currentPoints = maxPoints;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position + Vector3.up);

        healthBar.gameObject.SetActive(false);

        FreezeUnit();
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }
        if (looking)
        {
            Quaternion newLookAt = Quaternion.LookRotation(lookPoint - lineOfSight.position);
            newLookAt.x = 0;
            newLookAt.z = 0;
            lineOfSight.rotation = Quaternion.Slerp(lineOfSight.rotation, newLookAt, Time.deltaTime * 10);
        }
        if (targetTransform)
        {
            Quaternion newLookAt = Quaternion.LookRotation(targetTransform.transform.position - transform.position);
            newLookAt.x = 0;
            newLookAt.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, newLookAt, Time.deltaTime * 5);
        }
    }

    public void ResetUnit()
    {
        
    }

    public void Selected()
    {
        healthBar.gameObject.SetActive(true);
        selectedVFX.SetActive(true);
    }

    public void DeSelected()
    {
        healthBar.gameObject.SetActive(false);
        selectedVFX.SetActive(false);
    }

    // Create Action
    public void SetPlan(Vector3 targetLocation, PlayerAction action)
    {
        if (lr.positionCount == 0)
        {
            SetLineRendererPos(transform.position);
        }
        if (action != PlayerAction.look)
        {
            SetLineRendererPos(targetLocation);
        }
    }

    void SetLineRendererPos(Vector3 targetLocation)
    {
        int length = lr.positionCount;
        lr.positionCount = length + 1;
        lr.SetPosition(length, targetLocation + Vector3.up);
    }

    public void StartPlan()
    {
        StartCoroutine("RunningPlan");
    }

    IEnumerator RunningPlan()
    {
        yield return new WaitForFixedUpdate();

        if (dead == false)
        {
            while (plan.Count > 0)
            {
                Action action = plan[0];

                shootOnSight = true;

                while (EnemyVisible() && action.actionType != PlayerAction.run)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();

                float agentSpeed = 0;

                switch (action.actionType)
                {
                    case PlayerAction.walk:
                        agentSpeed = 1.5f;
                        agent.SetDestination(action.targetLocation);
                        break;
                    case PlayerAction.run:
                        shootOnSight = false;
                        agentSpeed = 4;
                        agent.SetDestination(action.targetLocation);
                        break;
                    case PlayerAction.look:
                        lookPoint = action.targetLocation;
                        lookAtPoint = true;
                        looking = true;
                        break;
                    default:
                        break;
                }

                agent.speed = agentSpeed;
                anim.SetFloat("Movement", agent.speed);

                if (action.actionType != PlayerAction.look)
                {
                    while (Vector3.Distance(transform.position, action.targetLocation) > 0.1f)
                    {
                        yield return new WaitForEndOfFrame();
                        while (EnemyVisible())
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        agent.speed = agentSpeed;
                        anim.SetFloat("Movement", agentSpeed);
                    }
                }

                yield return new WaitForEndOfFrame();

                while (EnemyVisible())
                {
                    yield return new WaitForEndOfFrame();
                }

                if (lookAtPoint == false)
                {
                    looking = false;
                    lineOfSight.localEulerAngles = Vector3.zero;
                }
                lookAtPoint = false;

                shootOnSight = true;
                anim.SetFloat("Movement", 0);
                plan.Remove(action);
            }
            while (PlayActionPlan.sequencing)
            {
                while (EnemyVisible())
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }
        }
        
    }

    bool EnemyVisible()
    {
        if (breakOutCombat)
        {
            targetTransform = null;
            breakOutCombat = false;
            return true;
        }
        if (unitSight.targetList.Count < 1)
        {
            LoseTarget();
            return false;
        }
        foreach (Unit target in unitSight.targetList)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);

            float distance = Vector3.Distance(transform.position + offset, target.transform.position + offset);
            if (distance > eyeRange)
            {
                LoseTarget();
            }
            else
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position + offset, (target.transform.position + offset) - (transform.position + offset));

                Vector3 targetDir = ((target.transform.position + offset) - (transform.position + offset)) * -1;
                float angle = Vector3.Angle(targetDir, lineOfSight.forward);

                angle -= 180;
                angle = Mathf.Abs(angle);
                Debug.DrawRay(transform.position + offset, (target.transform.position + new Vector3(0, 1f, 0)) - (transform.position + offset), Color.red);
                if (angle < viewAngle)
                {
                    Debug.DrawRay(transform.position + offset, (target.transform.position + new Vector3(0, 1f, 0)) - (transform.position + offset), Color.green);
                    if (Physics.Raycast(ray, out hit, eyeRange))
                    {
                        if (hit.collider.CompareTag("Unit"))
                        {
                            Unit u = hit.collider.GetComponent<Unit>();
                            if (u.idOwner != idOwner)
                            {
                                u.RevealCharacter();
                                if (shootOnSight == false)
                                {
                                    LoseTarget();
                                    return false;
                                }
                                if (u.dead == false)
                                {
                                    targetTransform = hit.collider.transform;
                                    agent.speed = 0;
                                    anim.SetFloat("Movement", 0);
                                    targetUnit = u;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        LoseTarget();
        return false;
    }

    public void Shoot()
    {
        shotVFX.Play();
        Unit targetUnit = targetTransform.GetComponent<Unit>();
        targetUnit.TakeDamage(damage, this);
        if (targetUnit.health <= 0)
        {
            LoseTarget();
        }
    }

    public void RemoveLinePath()
    {
        lr.positionCount = 0;
    }

    public void TakeDamage(int damageTaken, Unit attacker)
    {
        health -= damageTaken;
        healthBar.value = health;
        if (health <= 0)
        {
            StopCoroutine("RunningPlan");
            anim.SetTrigger("Death");
            dead = true;

            unitSight.HideMesh();
            agent.SetDestination(transform.position);
            GetComponent<Collider>().enabled = false;
            plan = new List<Action>();
        }
    }

    public void FreezeUnit()
    {
        anim.speed = 0;
    }

    public void UnFreezeUnit()
    {
        anim.speed = 1;
    }

    public void RevealCharacter()
    {
        foreach (var s in skin)
        {
            s.SetActive(true);
        }
    }

    [Button]
    public void RevealMyUnit()
    {
        lineOfSight.gameObject.SetActive(true);
        foreach (var s in skin)
        {
            s.SetActive(true);
        }
    }

    void LoseTarget()
    {
        if (targetUnit)
            targetUnit.HideCharacter();

        targetUnit = null;
        targetTransform = null;
    }

    [Button]
    public void HideCharacter()
    {
        if (isMine)
        {
            return;
        }
        unitSight.HideMesh();
        if (dead)
        {
            return;
        }
        foreach (var s in skin)
        {
            s.SetActive(false);
        }
    }


}

[System.Serializable]
public class Action
{
    public PlayerAction actionType;
    public Vector3 targetLocation;

    public Action(Vector3 _targetLocation, PlayerAction _action)
    {
        actionType = _action;
        targetLocation = _targetLocation;
    }
}

public enum PlayerAction
{
    walk,
    run,
    look
}