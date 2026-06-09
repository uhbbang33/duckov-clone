using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private PoolManager _poolManager;
    private EnemySound _enemySound;
    private HealthPoint _hp;
    private GunData _gunData;

    private void Awake()
    {
        _hp = GetComponent<HealthPoint>();
    }

    public void Init(GunData gunData)
    {
        _poolManager = PoolManager.Instance;
        _enemySound = GetComponent<EnemySound>();
        _gunData = gunData;
    }

    public void HealHP(float healAmount) => _hp.Heal(healAmount);

    public void EnemyDeath()
    {
        MakeLootBox();
        _enemySound.StopFootStep();
    }

    private void MakeLootBox()
    {
        GameObject lootBox = _poolManager.GetObject(PoolId.LootBox);

        if (lootBox.GetComponent<LootBox>() == null)
        {
            Debug.LogError("LootBox Has not EnemyGunData Property");
            return;
        }

        lootBox.transform.position = transform.position;
        lootBox.transform.rotation = transform.rotation;

        lootBox.GetComponent<LootBox>().EnemyGunData = _gunData;
    }
}
