using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    // The gun the Enemy is holding
    [SerializeField] private Gun gun;

    // The target the Enemy should shoot at
    public GameObject target;

    // Total number of shots the enemy can fire
    public int totalShots = 1;

    // Number of shots fired so far
    private int shotsFired = 0;

    public void FixedUpdate()
    {
        // If there is a target, continue firing until out of shots
        if (shotsFired < totalShots && target != null)
        {
            // Look at the target and shoot
            transform.LookAt(target.transform);
            gun.Shoot();
            shotsFired++;
        }
    }
}
