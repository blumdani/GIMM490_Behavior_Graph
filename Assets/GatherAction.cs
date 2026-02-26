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

        agent = Agent.Value.GetComponent<NavMeshAgent>();

        expectedCommand = DirectCommand.Value;

        agent.isStopped = false;
        agent.ResetPath();
        agent.updatePosition = true;
        agent.updateRotation = true;

        hasReachedTarget = false;
        gatherTimer = 0f;

        if (Material.Value != null)
        {
            Debug.Log("DIRECT GATHER MODE");
            Debug.Log("Material passed in: " + Material.Value);
            return Status.Running;
        }

        Debug.Log("AUTONOMOUS GATHER MODE");

        MaterialType lowest = GetLowestMaterial();
        string tag = GetTagFromMaterial(lowest);

        GameObject target = FindClosestWithTag(tag);

        if (target == null)
            return Status.Failure;

        Material.Value = target;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("isStopped = " + agent.isStopped);

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
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }
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
            if (gatherTimer >= gatherTime)
            {
                var type = GetMaterialType(Material.Value);

                GameObject.Destroy(Material.Value);
                Inventory.Instance.AddMaterial(type);

                Material.Value = null;

                return Status.Failure; // force reevaluation
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = false;
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

    MaterialType GetLowestMaterial()
    {
        int wood = Inventory.Instance.GetAmount(MaterialType.Tree);
        int rock = Inventory.Instance.GetAmount(MaterialType.Rocks);
        int grass = Inventory.Instance.GetAmount(MaterialType.Grass);
        int med = Inventory.Instance.GetAmount(MaterialType.Medicine);

        MaterialType lowest = MaterialType.Tree;
        int min = wood;

        if (rock < min) { min = rock; lowest = MaterialType.Rocks; }
        if (grass < min) { min = grass; lowest = MaterialType.Grass; }
        if (med < min) { min = med; lowest = MaterialType.Medicine; }

        return lowest;
    }

    string GetTagFromMaterial(MaterialType type)
    {
        switch (type)
        {
            case MaterialType.Tree: return "Tree";
            case MaterialType.Rocks: return "Rock";
            case MaterialType.Grass: return "Grass";
            case MaterialType.Medicine: return "Medicine";
        }

        return "Tree";
    }

    GameObject FindClosestWithTag(string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        if (objs.Length == 0) return null;

        GameObject closest = objs[0];
        float minDist = Vector3.Distance(agent.transform.position, closest.transform.position);

        foreach (GameObject obj in objs)
        {
            float dist = Vector3.Distance(agent.transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }

        return closest;
    }
}

