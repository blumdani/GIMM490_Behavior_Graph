using System;
using System.Buffers;
using Unity.Behavior;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Gather", story: "[Agent] Gathers [Material]", category: "Action", id: "90411b5525da0f02daab1f759fe9d789")]
public partial class GatherAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Material;
    [SerializeReference] public BlackboardVariable<DirectCommands> DirectCommand;
    private GameObject material;
    private Transform transform;
    private NavMeshAgent agent;
    private DirectCommands expectedCommand;

    private float gatherTime = 3.5f;
    private float gatherTimer = 0f;
    private bool hasReachedTarget = false;

    //Close enough to target
    private float stopDistance = 2.5f;

    protected override Status OnStart()
    {
        Debug.Log("ATTEMPTING TO GATHER");
        //assign agent alias
        agent = Agent.Value.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        expectedCommand = DirectCommand.Value;

        //Initialize variables
        hasReachedTarget = false;
        gatherTimer = 0f;
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

        //If we've already collected the material, make sure we exit the action.
        if (Material.Value == null)
        {
            return Status.Failure;
        }

        //Update the distance between agent and target
        Vector3 targetPosition = Material.Value.transform.position;
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
        else
        {
            //After movement, gather for gatherTime
            gatherTimer += Time.deltaTime;
            if(gatherTimer >= gatherTime)
            {
                GameObject.Destroy(Material.Value.gameObject);
                Inventory.Instance.AddMaterial(GetMaterialType(Material.Value.gameObject));
                Material.Value = null;

                if (DirectCommand != null)
                    DirectCommand.Value = DirectCommands.Wait;

                return Status.Failure;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    private MaterialType GetMaterialType(GameObject obj)
    {
        switch (obj.tag)
        {
            case "Tree":
                return MaterialType.Tree;

            case "Rock":
                return MaterialType.Rocks;

            case "Grass":
                return MaterialType.Grass;

            case "Medicine":
                return MaterialType.Medicine;
        }

        Debug.LogWarning("Unknown material tag: " + obj.tag);
        return MaterialType.Tree; // default
    }
}

