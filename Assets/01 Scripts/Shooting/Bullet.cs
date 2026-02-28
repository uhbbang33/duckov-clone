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
        if (collision.gameObject.CompareTag("Bullet"))
            return;

        // hit effect
        // TODO - 적과 사물 layer다르게 해서 effect도 다르게
        GameObject effectObject = PoolManager.Instance.GetObject(PoolId.Explosion, collision.gameObject.transform, false);

        ContactPoint contactPoint = collision.contacts[0];

        effectObject.transform.position = contactPoint.point;
        effectObject.transform.rotation = Quaternion.LookRotation(contactPoint.normal);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnObject(PoolId.Bullet, gameObject);
    }

}
