using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] private Image firstImage;
    [SerializeField] private Image secondImage;
    [SerializeField] private Button playButton;

    private void Start() {
        Time.timeScale = 1.0f;
        PlayerPrefs.SetString("Verb", "NORMAL");
        PlayerPrefs.Save();
        int achivement = GlobalProc.LoadAchivement();
        if (achivement >= 1) firstImage.color = new Color(1f, 1f, 0f, 1f);
        if (achivement >= 3) secondImage.color = new Color(1f, 1f, 0f, 1f);
    }
}