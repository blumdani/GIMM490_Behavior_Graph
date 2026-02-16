using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Agent] Attacks [Enemy]", category: "Action", id: "5229548a79a0546d8e4729d68d3e93df")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<DirectCommands> DirectCommand;

    private NavMeshAgent agent;
    private DirectCommands expectedCommand;
    private float attackDistance = 5f;

    protected override Status OnStart()
    {
        if (Enemy.Value == null)
        {
            Debug.Log("Attack Action: No enemy assigned.");
            return Status.Failure;
        }

        expectedCommand = DirectCommand.Value;
        agent = Agent.Value.GetComponent<NavMeshAgent>();
        agent.isStopped = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (DirectCommand.Value != expectedCommand)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            return Status.Failure;
        }

        // If current enemy gone, exit attack mode
        if (Enemy.Value == null)
        {
            agent.isStopped = true;
            return Status.Failure;
        }

        // Move toward enemy
        agent.SetDestination(Enemy.Value.transform.position);

        float dist = Vector3.Distance(
            agent.transform.position,
            Enemy.Value.transform.position
        );

        // Attack when close enough
        if (dist <= attackDistance)
        {
            GameObject.Destroy(Enemy.Value);
            Enemy.Value = null;
            Debug.Log("Enemy destroyed");

            return Status.Failure;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }
}
