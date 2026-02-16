using System;
using System.Collections;
using TMPro;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Heal", story: "[Agent] Heals [Target]", category: "Action", id: "0f26a98617eea44a8b10c75fb9f5c4a5")]
public partial class HealAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<DirectCommands> DirectCommand;
    public HealthController healthController;
    private Transform transform;
    private NavMeshAgent agent;
    private DirectCommands expectedCommand;

    //Healing variables
    private float healTimer = 0f;
    private float healCooldown = 1f;

    //Movement variables
    private float stopDistance = 3.5f;
    private bool hasReachedTarget = false;

    protected override Status OnStart()
    {
        healthController = GameObject.FindFirstObjectByType<HealthController>();
        //assign agent alias
        agent = Agent.Value.GetComponent<NavMeshAgent>();
        expectedCommand = DirectCommand.Value;
        agent.isStopped = false;

        if (DirectCommand.Value == DirectCommands.HealMe)
        {
            Target.Value = GameObject.FindWithTag("Player");
            hasReachedTarget = false;
        }
        else if(DirectCommand.Value == DirectCommands.HealYourself)
        {
            hasReachedTarget = true;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //Check for interrupt
        if (DirectCommand.Value != expectedCommand)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            return Status.Failure;
        }

        //Check if we have medicine to heal
        if (Inventory.Instance.GetAmount(MaterialType.Medicine) == 0)
        {
            Debug.Log("No medicine, can't heal!");
            return Status.Failure;
        }

        //Update the distance between agent and target
        Vector3 targetPosition = Target.Value.transform.position;
        float dist = Vector3.Distance(agent.transform.position, targetPosition);

        //If we haven't reached the target, move toward it
        if (!hasReachedTarget)
        {
            agent.SetDestination(targetPosition);

            //If we have reached it, stop moving.
            if (dist <= stopDistance)
            {
                hasReachedTarget = true;
                agent.isStopped = true;
            }

            return Status.Running;
        }
        //Confirm we have medicine available
        else {
            //After next to target, heal
            if (Target.Value.tag == "Player")
            {
                if (healthController.playerHealth < 100)
                {
                    //Each heal waits healCooldown seconds
                    healTimer -= Time.deltaTime;

                    if (healTimer <= 0f)
                    {
                        healthController.PlayerHeal();
                        Inventory.Instance.RemoveMaterial(MaterialType.Medicine);
                        healTimer = healCooldown;
                    }

                    return Status.Running;
                }
                else
                {
                    Debug.Log("Player Healed!");
                    return Status.Failure;
                }
            }
            else if (Target.Value.tag == "CompanionAI")
            {
                if (healthController.companionHealth < 100)
                {
                    healTimer -= Time.deltaTime;

                    if (healTimer <= 0f)
                    {
                        healthController.CompanionHeal();
                        Inventory.Instance.RemoveMaterial(MaterialType.Medicine);
                        healTimer = healCooldown;
                    }

                    return Status.Running;
                }
                else
                {
                    Debug.Log("Companion Healed!");
                    //Tell our enum we're done with this state.
                    return Status.Failure;
                }
            }
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

