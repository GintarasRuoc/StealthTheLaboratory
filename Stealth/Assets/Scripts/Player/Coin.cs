using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float soundRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;


    private void Awake()
    {
        // Emit sound
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius, enemyLayer);
        if (colliders.Length > 0)
            foreach (Collider enemy in colliders)
                enemy.GetComponent<Enemy>().hearSound(transform.position);
        Debug.Log("Spanwed coin. Enemys in range " + colliders.Length);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}
