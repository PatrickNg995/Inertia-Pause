using UnityEngine;

public class WeaponTargetPositioning : MonoBehaviour
{
    // Only done while in the editor.
    [ExecuteInEditMode]

    // The target the Enemy should aim at.
    public Transform Target;

    // Origin Point of the weapon.
    public Transform WeaponSpawnPoint;

    // Bullet Prefab Object.
    public Transform Weapon;

    [SerializeField]
    private bool _isWeaponDistanceOn;
    // Distance between bullet & target by percentage (1 == bullet is on target).
    [SerializeField]
    [Range(0f, 1f)]
    private float _weaponDistance;

    // Runs whenever a value changes.
    void OnValidate()
    {
        if (!_isWeaponDistanceOn)
        {
            return;
        }

        if (Target != null && Weapon != null && WeaponSpawnPoint != null)
        {
            // Enemy & Bullet face the enemy.
            transform.LookAt(Target.transform);
            Weapon.transform.forward = WeaponSpawnPoint.forward;

            // Set correct y position.
            float targetYPosition = Target.position.y + WeaponSpawnPoint.localPosition.y;
            Vector3 targetPosition = new Vector3(Target.position.x, targetYPosition, Target.position.z);

            // Get the distance between the bullet & the target.
            Vector3 maxDistance = WeaponSpawnPoint.position - targetPosition;
            // Change the bullet's position depending on the BulletDistance modifier.
            Weapon.position = WeaponSpawnPoint.position - (maxDistance * _weaponDistance);
        }
    }
}
