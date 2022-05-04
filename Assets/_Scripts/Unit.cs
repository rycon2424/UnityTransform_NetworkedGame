using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class Unit : NetworkedObject
{
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
    [ReadOnly] [SerializeField] bool idle = true;
    [ReadOnly] public Transform targetTransform;

    [Header("Assign Manually")]
    [SerializeField] GameObject selectedVFX;
    [SerializeField] ParticleSystem shotVFX;
    [SerializeField] Transform lineOfSight;
    [SerializeField] UnitSight unitSight;
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
        if (idle)
        {
            if (shootOnSight)
            {
                if (EnemyVisible())
                {
                    Debug.Log("Combat");
                }
            }
        }
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

    public void SetRoute(Vector3 targetLocation, PlayerAction action)
    {
        if (action != PlayerAction.look)
        {
            int length = lr.positionCount;
            lr.positionCount = length + 1;
            lr.SetPosition(length, targetLocation + Vector3.up);
        }
        Action newAction = new Action(targetLocation, action);
        plan.Add(newAction);
    }

    public void StartPlan()
    {
        StartCoroutine("RunningPlan");
    }

    IEnumerator RunningPlan()
    {
        idle = false;

        yield return new WaitForFixedUpdate();

        while (EnemyVisible())
        {
            yield return new WaitForEndOfFrame();
        }

        while (plan.Count > 0)
        {
            Action action = plan[0];

            shootOnSight = true;

            yield return new WaitForEndOfFrame();

            float agentSpeed = 0;

            switch (action.actionType)
            {
                case PlayerAction.walk:
                    agentSpeed = 1;
                    agent.SetDestination(action.targetLocation);
                    break;
                case PlayerAction.run:
                    shootOnSight = false;
                    agentSpeed = 2;
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
        idle = true;
    }

    bool EnemyVisible()
    {
        if (breakOutCombat)
        {
            targetTransform = null;
            breakOutCombat = false;
            return true;
        }
        if (shootOnSight == false)
        {
            LoseTarget();
            return false;
        }
        if (unitSight.targetList.Count < 1)
        {
            LoseTarget();
            return false;
        }

        foreach (Unit target in unitSight.targetList)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);

            if (Vector3.Distance(transform.position + offset, target.transform.position + offset) > eyeRange)
            {
                LoseTarget();
                return false;
            }

            RaycastHit hit;
            Ray ray = new Ray(transform.position + offset, (target.transform.position + offset) - (transform.position + offset));

            Vector3 targetDir = ((target.transform.position + offset) - (transform.position + offset)) * -1;
            float angle = Vector3.Angle(targetDir, lineOfSight.forward);

            angle -= 180;
            angle = Mathf.Abs(angle);

            if (angle < viewAngle)
            {
                Debug.DrawRay(transform.position + offset, (target.transform.position + offset) - (transform.position + offset), Color.black);
                if (Physics.Raycast(ray, out hit, eyeRange))
                {
                    if (hit.collider.CompareTag("Unit"))
                    {
                        Unit u = hit.collider.GetComponent<Unit>();
                        if (u.idOwner != idOwner)
                        {
                            if (u.dead == false)
                            {
                                targetTransform = hit.collider.transform;
                                agent.speed = 0;
                                anim.SetFloat("Movement", 0);
                                return true;
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
        targetUnit.TakeDamage(damage);
        if (targetUnit.health <= 0)
        {
            LoseTarget();
        }
    }

    public void RemoveLinePath()
    {
        lr.positionCount = 0;
    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        healthBar.value = health;
        if (health <= 0)
        {
            StopCoroutine("RunningPlan");
            anim.SetTrigger("Death");
            dead = true;

            lineOfSight.gameObject.SetActive(false);
            agent.SetDestination(transform.position);
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

    public void RevealMyUnit()
    {
        lineOfSight.gameObject.SetActive(true);
        // Shows Character model
    }

    void LoseTarget()
    {
        HideCharacter();
        targetTransform = null;
    }

    public void HideCharacter()
    {
        // Hides Character model
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