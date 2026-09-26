using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    
    private Vector3 target;
    private float damage;
    private DamageType damageType;
    
    public void Initialize(Vector3 targetPosition, float projectileDamage, DamageType type)
    {
        target = targetPosition;
        damage = projectileDamage;
        damageType = type;
        Destroy(gameObject, lifetime);
    }
    
    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, damageType);
            }
            Destroy(gameObject);
        }
    }
}