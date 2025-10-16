using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Shooting Settings")]
    // The gun the Enemy is holding.
    [SerializeField] private Gun Gun;

    // The target the Enemy should shoot at.
    [SerializeField] private GameObject Target;

    // Total number of shots the enemy can fire.
    [SerializeField] private int TotalShots = 1;

    // Number of shots fired so far.
    private int _shotsFired = 0;

    public void FixedUpdate()
    {
        // If there is a target, continue firing until out of shots.
        if (_shotsFired < TotalShots && Target != null)
        {
            // Look at the target and shoot.
            transform.LookAt(Target.transform);
            Gun.Shoot();
            _shotsFired++;
        }
    }
}
