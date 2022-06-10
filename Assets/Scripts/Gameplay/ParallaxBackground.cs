using UnityEngine;
 
public class ParallaxBackground : MonoBehaviour {
    public float speed = 2f;
    [SerializeField] private float threshold = 9.6f;
    private bool isBackgroundCreated = false;
    private bool isRendered = false;
    [SerializeField] private GameObject background;
    [SerializeField] private SpriteRenderer parentRenderer;
    [SerializeField] private Renderer viewpointRenderer;

    private void Start() {
        if (GlobalVar.ParralaxSpeed == 0f) this.enabled = false;
        speed *= GlobalVar.ParralaxSpeed;
    }

    private void CheckDespawnDelay()
    {
        if (!viewpointRenderer.isVisible && isRendered)
        {
            Destroy(transform.parent.gameObject);         
        }
        else if (viewpointRenderer.isVisible && !isRendered)
        {
            isRendered = !isRendered;
        }
    }

    private void LateUpdate() 
    {
        transform.parent.position -= new Vector3(
            speed * Time.deltaTime,
            0f,
            0f
        );

        if (!isBackgroundCreated && Camera.main.IsObjectVisible(viewpointRenderer))
        {
            // New background inherits old one, resets speed to default
            speed /= GlobalVar.ParralaxSpeed;
            GameObject obj = Instantiate(
                background, 
                new Vector3(transform.position.x + threshold, transform.position.y, transform.position.z), 
                Quaternion.identity
            );
            obj.name = "Alt_Background";
            // Restores intended speed
            speed *= GlobalVar.ParralaxSpeed;
            isBackgroundCreated = true;
        }

        CheckDespawnDelay();
    }
}