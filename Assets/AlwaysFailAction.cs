using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AlwaysFail", story: "Always Fail", category: "Action", id: "04d33ac8f39e7663d243fc78a0643d03")]
public partial class AlwaysFailAction : Action
{

    protected override Status OnStart()
    {
        return Status.Failure;
    }
}

