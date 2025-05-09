using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTargetDetect", 
    story: "[Self] Detect [Target] from [DetectionOrigin] [RayCount] [ViewAngle] [MaxDistance] ", 
    category: "Conditions", 
    id: "5296369cdb66c5b8f1544794781abfff")]
public partial class CheckTargetDetectCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> DetectionOrigin;
    [SerializeReference] public BlackboardVariable<int> RayCount;
    [SerializeReference] public BlackboardVariable<float> ViewAngle;
    [SerializeReference] public BlackboardVariable<float> MaxDistance;

    private LayerMask detectionMask;

    public override bool IsTrue()
    {
        return CheckTarget();
    }

    public override void OnStart()
    {
        detectionMask = Self.Value.GetComponent<Enemy>().DetectionMask;
    }

    private bool CheckTarget()
    {
        Vector3 origin = DetectionOrigin.Value.transform.position;
        Vector3 forward = DetectionOrigin.Value.transform.forward;

        float stepAngle = ViewAngle / RayCount;

        // ã�Ҵ� ǥ�� ����
        bool check = false;

        for (int i = 0; i < RayCount; ++i)
        {
            float angle = -(ViewAngle / 2) + stepAngle * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up); // ������ ��ä���̶� Vector3.up ����
            Vector3 direction = rotation * forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, MaxDistance, detectionMask))
            {
                int hitLayerMask = hit.collider.gameObject.layer;

                if ((LayerMask.GetMask("Player") & hitLayerMask) != 0)
                {
                    Debug.DrawLine(origin, hit.point, Color.red);
                    check = true;
                }
                
                if ((LayerMask.GetMask("Wall") & hitLayerMask) != 0)
                {
                    Debug.DrawLine(origin, hit.point, Color.green);
                }
                
                // ã�Ҵ� ǥ��
                // �ߺ��̱��ѵ�..���ɿ� ���� ���� ��������
                
            }
            else
            {
                Debug.DrawRay(origin, direction * MaxDistance, Color.green);
            }

        }

        return check;
    }
}
