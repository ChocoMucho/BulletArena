using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.AI;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Self] Navigate To [Target], stop [StopDistance]", category: "Action", id: "e22772a4749eb18e2f3bad001e6ed5fe")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> StopDistance;

    NavMeshAgent _agent;

    protected override Status OnStart()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        _agent.speed = 5f;
        _agent.stoppingDistance = StopDistance;
        _agent.SetDestination(Target.Value.transform.position);


        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

