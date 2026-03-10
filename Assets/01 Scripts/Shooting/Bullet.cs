using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Rigidbody _rb;
    private Vector3 _startPos;
    private float _maxDistance;
    private PoolManager _poolManager;
    private float _damage;

    public float BulletDamage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _poolManager = PoolManager.Instance;
    }

    private void Update()
    {
        float dist = Vector3.Distance(_startPos, transform.position);
        if (dist > _maxDistance)
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
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("Bullet"))
            return;

        GameObject hitEffectObject;
        if (obj.CompareTag("Player") || obj.CompareTag("Enemy"))
        {
            hitEffectObject = _poolManager.GetObject(PoolId.BloodSmoke, obj.transform, false);

            obj.GetComponent<HealthPoint>().TakeDamage(_damage);
        }
        else
        {
            hitEffectObject = _poolManager.GetObject(PoolId.Smoke, obj.transform, false);
        }

        ContactPoint contactPoint = collision.contacts[0];

        hitEffectObject.transform.position = contactPoint.point;
        hitEffectObject.transform.rotation = Quaternion.LookRotation(contactPoint.normal);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _poolManager.ReturnObject(PoolId.Bullet, gameObject);
    }

}
