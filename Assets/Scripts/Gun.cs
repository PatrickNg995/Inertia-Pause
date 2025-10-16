using UnityEngine;

public class Gun : MonoBehaviour
{
    // Bullet prefab to instantiate when shooting.
    [SerializeField] private GameObject _bulletPrefab;

    // Point from which bullets are spawned, an empty GameObject at the end of the gun barrel.
    [SerializeField] private GameObject _bulletSpawnPoint;

    public void Shoot()
    {
        // Get the transform component of the bullet spawn point.
        Transform bulletSpawnTransform = _bulletSpawnPoint.GetComponent<Transform>();

        // Instantiate a bullet at the bullet spawn point position with no rotation.
        GameObject bullet = Instantiate(_bulletPrefab, bulletSpawnTransform.position, Quaternion.identity);

        // Set bullet facing the right direction.
        bullet.transform.forward = bulletSpawnTransform.forward;
    }
}
