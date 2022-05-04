using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class Unit : NetworkedObject
{
    [ReadOnly] public int maxPoints;
    [ReadOnly] public int currentPoints;
    [ReadOnly] [SerializeField] bool shootOnSight = true;
    [ReadOnly] [SerializeField] bool lookAtPoint;
    [ReadOnly] [SerializeField] bool idle;
    [Header("Assign Manually")]
    [SerializeField] GameObject selectedVFX;
    [SerializeField] ParticleSystem shotVFX;
    [SerializeField] MeshRenderer sightVisual;
    [SerializeField] UnitSight unitSight;
    [Space]
    public List<Action> plan = new List<Action>();

    Animator anim;
    NavMeshAgent agent;
    LineRenderer lr;
    Transform lineOfSight;

    Vector3 lookPoint;
    bool looking;

    public virtual void Start()
    {
        maxPoints = GameBalanceManager.instance.maxPoints;
        currentPoints = maxPoints;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        lineOfSight = sightVisual.transform.parent;

        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position + Vector3.up);

        sightVisual.enabled = false;
    }

    private void Update()
    {
        if (looking)
        {
            Quaternion newLookAt = Quaternion.LookRotation(lookPoint - lineOfSight.position);
            newLookAt.x = 0;
            newLookAt.z = 0;
            lineOfSight.rotation = Quaternion.Slerp(lineOfSight.rotation, newLookAt, Time.deltaTime * 5);
        }
        if (idle)
        {
            if (EnemyVisible())
            {
                Debug.Log("Combat");
            }
        }
    }

    public void Selected()
    {
        sightVisual.enabled = true;
        selectedVFX.SetActive(true);
    }

    public void DeSelected()
    {
        sightVisual.enabled = false;
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
        StartCoroutine(RunningPlan());
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
                    if (shootOnSight)
                    {
                        while (EnemyVisible())
                        {
                            agent.speed = 0;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    agent.speed = agentSpeed;
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
        if (unitSight.targetList.Count < 1)
        {
            return false;
        }
        foreach (Unit target in unitSight.targetList)
        {
            RaycastHit hit;
            Vector3 offset = new Vector3(0, 0.5f, 0);
            Ray ray = new Ray(transform.position + offset, (target.transform.position + offset) - (transform.position + offset));
            Debug.DrawRay(transform.position + offset, (target.transform.position + offset) - (transform.position + offset), Color.black, 2f);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Unit"))
                {
                    if (hit.collider.GetComponent<Unit>().idOwner != idOwner)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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