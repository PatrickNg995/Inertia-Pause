using UnityEngine;

public class Gun : MonoBehaviour
{
    // Bullet prefab to instantiate when shooting
    [SerializeField] private GameObject bulletPrefab;

    // Point from which bullets are spawned, an empty GameObject at the end of the gun barrel
    [SerializeField] private GameObject bulletSpawnPoint;

    public void Shoot()
    {
        // Get the transform component of the bullet spawn point
        Transform bulletSpawnTransform = bulletSpawnPoint.GetComponent<Transform>();

        // Instantiate a bullet at the bullet spawn point position with no rotation
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, Quaternion.identity);

        // Set bullet facing the right direction
        bullet.transform.forward = bulletSpawnTransform.forward;
    }
}
