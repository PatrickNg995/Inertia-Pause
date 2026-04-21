using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraLayerCullDistances : MonoBehaviour
{
    [SerializeField] private float _interactionObjectsCullDistance = 50f;
    [SerializeField] private float _playerBoundaryCullDistance = 5f;

    private void Start()
    {
        Camera cam = GetComponent<Camera>();
        float[] distances = new float[32];

        // Layer 6: InteractionObjects — max interaction distance is 2 units, no need to render past 50
        distances[6] = _interactionObjectsCullDistance;

        // Layer 8: PlayerBoundary — invisible collision volumes, minimal draw distance
        distances[8] = _playerBoundaryCullDistance;

        cam.layerCullDistances = distances;
        cam.layerCullSpherical = true;
    }
}
