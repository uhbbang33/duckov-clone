using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Rigidbody _rb;
    private Vector3 _startPos;
    private float _maxDistance;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float dist = Vector3.Distance(_startPos, transform.position);
        if(dist > _maxDistance)
        {
            ReturnToPool();
        }
    }

    public void Fire(Vector3 direction, float range)
    {
        _rb.linearVelocity = direction * _speed;
        _startPos = transform.position;
        _maxDistance = range;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnObject(PoolId.Bullet, gameObject);
    }

}
