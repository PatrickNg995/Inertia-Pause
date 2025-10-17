using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    // Only done while in the editor
    [ExecuteInEditMode]

    // The target the Enemy should shoot at
    public Transform target;

    // Origin Point of the bullet
    public Transform bulletSpawnPoint;

    // Bullet Prefab Object
    public Transform bullet;

    // Distance between bullet & target by percentage (1 == bullet is on target)
    [SerializeField]
    [Range(0f, 1f)]
    private float BulletDistance;

    // Runs whenever a value changes
    void OnValidate()
    {
        if (target != null)
        {
            // Enemy & Bullet face the enemy
            transform.LookAt(target.transform);
            bullet.transform.forward = bulletSpawnPoint.forward;

            // Set correct y position
            float targetYPosition = target.position.y + bulletSpawnPoint.position.y;
            Vector3 targetPosition = new Vector3(target.position.x, targetYPosition, target.position.z);

            // Get the distance between the bullet & the target
            Vector3 maxDistance = bulletSpawnPoint.position - targetPosition;

            // Change the bullet's position depending on the BulletDistance modifier
            bullet.position = bulletSpawnPoint.position - (maxDistance * BulletDistance);
        }
    }
}
