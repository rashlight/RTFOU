using UnityEngine;
using UnityEngine.UI;

public class FastRestart : MonoBehaviour {
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;
    private void Update() {
        if (Input.GetButtonDown("Restart"))
        {
            retryButton.onClick.Invoke();
            return;
        }
    }
}