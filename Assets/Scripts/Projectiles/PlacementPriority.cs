using UnityEngine;

public class PlacementPriority : MonoBehaviour {

    [Tooltip("See Miscs/ENVIRONMENT PRIORITIES.txt for more information.")]
    [Range(84f, 91f)]
    public float priority = 87f;

    private void Awake() {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            priority
        );
    }
}