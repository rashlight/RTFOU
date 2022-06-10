using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public float despawnDelay = 3f;
    [Range(1.0f, 2.0f)]
    public float easyModeEndTarget = 1.5f;

    [SerializeField] protected Player playerHandler;
    private float speed = 1f;
    private float fluctuationHorizontal = 3f;
    private float fluctuationVertical = 0.1f;
    private bool isPlatformCreatable = true;
    private bool isOriginator = true;
    private GameObject platformSlice;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Renderer viewpointRendererStart;

    private GameObject GetRandomPlatformSlice()
    {
        return GlobalVar.self.platformSlices[Random.Range(0, GlobalVar.self.platformSlices.Length)];
    }

    private void CheckDespawnDelay()
    {
        if (!viewpointRendererStart.isVisible && !spriteRenderer.isVisible)
        {
            despawnDelay -= Time.deltaTime;
            if (despawnDelay <= 0f)
            {
                if (isOriginator)
                {
                    viewpointRendererStart = spriteRenderer;
                    // Excluding last object (generator)
                    for (int i = 0; i < transform.parent.childCount - 1; i++)
                    {
                        Destroy(transform.parent.GetChild(i).gameObject);
                    }
                    despawnDelay = Mathf.Infinity;
                }
                else
                {
                    Destroy(transform.parent.gameObject);
                }
            }
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        platformSlice = GetRandomPlatformSlice();
        if (isOriginator)
        {
            speed = GlobalVar.PlatformSpeed;
            fluctuationHorizontal = GlobalVar.PlatformFluctuationHorizontal * Mathf.Clamp01(GlobalVar.GameDifficulty / easyModeEndTarget);
            fluctuationVertical = GlobalVar.PlatformFluctuationVertical * Mathf.Clamp01(GlobalVar.GameDifficulty / easyModeEndTarget);
        }       
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHandler == null) Destroy(transform.parent.gameObject);

        transform.parent.position -= new Vector3(
            speed * Time.deltaTime,
            0f,
            0f
        );

        if (isPlatformCreatable && Camera.main.IsObjectVisible(spriteRenderer))
        {
            float distance = UnityEngine.Random.Range(1f, fluctuationHorizontal);

            Vector3 newPos = new Vector3(transform.position.x + distance, -4.5f + UnityEngine.Random.Range(0f, fluctuationVertical), 85f);

            GameObject obj = Instantiate(platformSlice, newPos, Quaternion.identity);
            obj.name = platformSlice.name + "(Clone)";

            // New object follows originator's properties
            LevelGenerator parentObjectProperties = obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<LevelGenerator>();          
            parentObjectProperties.speed = GlobalVar.PlatformSpeed;
            parentObjectProperties.despawnDelay = 0.5f + distance / speed;
            parentObjectProperties.fluctuationHorizontal = GlobalVar.PlatformFluctuationHorizontal * Mathf.Clamp01(GlobalVar.GameDifficulty / easyModeEndTarget);
            parentObjectProperties.fluctuationVertical = GlobalVar.PlatformFluctuationVertical * Mathf.Clamp01(GlobalVar.GameDifficulty / easyModeEndTarget);
            parentObjectProperties.platformSlice = GetRandomPlatformSlice();
            parentObjectProperties.playerHandler = playerHandler;
            parentObjectProperties.isOriginator = false;        
            
            isPlatformCreatable = !isPlatformCreatable;
        }

        CheckDespawnDelay();
    }
}
