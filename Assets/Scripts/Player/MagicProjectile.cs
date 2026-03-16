using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject hitEffect;
    
    private float damage;
    private Vector3 direction;
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    
    public void Initialize(float damageAmount, Vector3 shootDirection)
    {
        damage = damageAmount;
        direction = shootDirection;
    }
    
    void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, DamageType.Magical);
            
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }
}
