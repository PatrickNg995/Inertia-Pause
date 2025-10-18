using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    // Only done while in the editor
    [ExecuteInEditMode]

    // The target the Enemy should shoot at
    public Transform Target;

    // Origin Point of the bullet
    public Transform BulletSpawnPoint;

    // Bullet Prefab Object
    public Transform Bullet;

    [SerializeField]
    private bool _isBulletDistanceOn;
    // Distance between bullet & target by percentage (1 == bullet is on target)
    [SerializeField]
    [Range(0f, 1f)]
    private float _bulletDistance;

    // Runs whenever a value changes
    void OnValidate()
    {
        if (!_isBulletDistanceOn)
        {
            return;
        }

        if (Target != null && Bullet != null && BulletSpawnPoint != null)
        {
            // Enemy & Bullet face the enemy
            transform.LookAt(Target.transform);
            Bullet.transform.forward = BulletSpawnPoint.forward;

            // Set correct y position
            float targetYPosition = Target.position.y + BulletSpawnPoint.position.y;
            Vector3 targetPosition = new Vector3(Target.position.x, targetYPosition, Target.position.z);

            // Get the distance between the bullet & the target
            Vector3 maxDistance = BulletSpawnPoint.position - targetPosition;

            // Change the bullet's position depending on the BulletDistance modifier
            Bullet.position = BulletSpawnPoint.position - (maxDistance * _bulletDistance);
        }
    }
}
