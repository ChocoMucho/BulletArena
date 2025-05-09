using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.Behavior;
using System.Collections;

public class Enemy : MonoBehaviour
{
    NavMeshAgent _agent;
    public Transform playerTransform;
    Animator _animator;

    [SerializeField] private List<GameObject> wayPoints;
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    [Header("�÷��̾� Ž��")]
    [SerializeField] private GameObject _eyePositionObject;
    [SerializeField] private int _rayCount = 10;
    [SerializeField] private float _viewAngle = 10f;
    [SerializeField] private float _maxDistance = 10f;
    private bool _isTargetDetected;
    [SerializeField] public LayerMask DetectionMask;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        SetBehavior();

       //StartCoroutine(CheckTarget());
    }

    void SetBehavior()
    {
        behaviorAgent.SetVariableValue("PatrolPoints", wayPoints);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //DetectPlayer();
    }

    private void Move()
    {
        //_agent.destination = playerTransform.position;
        //_animator.SetFloat(EnemyAnimationHashes.Blend, _agent.velocity.magnitude);
        Debug.Log(_agent.velocity.magnitude);
    }

    private void DetectPlayer()
    {
        // _eyePositionObject�� Ray ������
        Vector3 forward = _eyePositionObject.transform.forward;
        Vector3 right = _eyePositionObject.transform.right;

        for (int i = 0; i < _rayCount; ++i)
        {
            float rollAngle = (360 / _rayCount) * i;
            Quaternion roll = Quaternion.AngleAxis(rollAngle, forward);
            Quaternion pitch = Quaternion.AngleAxis(_viewAngle, roll * right);
            Vector3 rayDirection = pitch * forward;

            Debug.DrawRay(_eyePositionObject.transform.position, rayDirection * _maxDistance, Color.red);
        }

    }

    IEnumerator CheckTarget()
    {      
        while(true)
        {
            Vector3 origin = _eyePositionObject.transform.position;
            Vector3 forward = _eyePositionObject.transform.forward;

            float stepAngle = _viewAngle / _rayCount;

            // ã�Ҵ� ǥ�� ����

            for(int i = 0; i < _rayCount; ++i)
            {
                float angle = -(_viewAngle / 2) + stepAngle * i; 
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up); // ������ ��ä���̶� Vector3.up ����
                Vector3 direction = rotation * forward;

                if(Physics.Raycast(origin, direction, out RaycastHit hit, _maxDistance, DetectionMask))
                {
                    Debug.DrawLine(origin, hit.point, Color.red);
                    // ã�Ҵ� ǥ��
                    // �ߺ��̱��ѵ�..���ɿ� ���� ���� ��������
                }
                else
                {
                    Debug.DrawRay(origin, direction * _maxDistance, Color.green);
                }
                
            }

            yield return null;
        }
    }
}
