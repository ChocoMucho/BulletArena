using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 0.001f;
    private Vector3 _direction = Vector3.zero;

    private Vector3 _previousPosition;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private GameObject sparkEffect;

    private void Start()
    {
        _previousPosition = transform.position;
    }

    public void SetDirection(Transform target)
    {
        _direction = (target.position - transform.position).normalized; // 길이 1로
        transform.rotation = Quaternion.LookRotation(_direction);
    }

    private void Update()
    {
        if (_direction != Vector3.zero)
        {
            transform.position += _direction * Time.deltaTime * speed;
        }
    }

    private void LateUpdate()
    {
        Vector3 preToCurDirection = (transform.position - _previousPosition).normalized;
        float distance = Vector3.Distance(transform.position, _previousPosition);
        if(Physics.Raycast(_previousPosition, preToCurDirection, out RaycastHit hit, distance, layerMask))
        {
            Debug.Log("벽이나 땅");
            speed = 0f;
            Instantiate(sparkEffect, hit.point, Quaternion.identity);
        }

        Debug.DrawRay(_previousPosition, preToCurDirection * distance, Color.red, 1f);
        _previousPosition = transform.position;
    }
}
