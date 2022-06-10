using System.Collections;
using UnityEngine;

public class EntityExplosion : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;

    private void Start() {
        audioSource.Play();
    }

    public virtual void WaitForAnimation()
    {
        Destroy(gameObject);
    }
}