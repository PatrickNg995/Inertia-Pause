using System.Collections;
using UnityEngine;

public static class PrepauseSimulationUtility
{
    /// <summary>
    /// Simulates projectile movement from startPoint to endPoint over the specified duration.
    /// </summary>
    public static IEnumerator SimulateProjectileMovement(Transform transform, Vector3 startPoint, Vector3 endPoint, float simulationDuration)
    {
        // Calculate speed needed to reach end point in the given duration.
        float speed = Vector3.Distance(startPoint, endPoint) / simulationDuration;
        float elapsedTime = 0f;

        // Move the bullet towards the end point over the simulation duration.
        while (elapsedTime < simulationDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is set accurately.
        transform.position = endPoint;
    }
}
