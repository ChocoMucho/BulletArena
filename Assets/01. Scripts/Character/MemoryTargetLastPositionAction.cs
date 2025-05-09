using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MemoryTargetLastPosition", story: "Memory [TargetLastPosition] of [Target]", category: "Action", id: "742cb94373ad9256c6b6625ec765f659")]
public partial class MemoryTargetLastPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetLastPosition;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        TargetLastPosition.ObjectValue = Target.Value.transform.position;

        return Status.Running;
    }

}

