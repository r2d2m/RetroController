using UnityEngine;
using UnityEngine.UI;

public class SampleUI : MonoBehaviour
{
    bool showInterface = true;
    public CanvasGroup instructions;
    public CanvasGroup mouseSettings;
    public Slider mouseSensitivitySlider;
    public Text mouseSensitivityText;
    MouseLook playerMouseLook;

    void Start()
    {
        playerMouseLook = FindObjectOfType<MouseLook>();
        if (playerMouseLook != null)
        {
            mouseSensitivitySlider.onValueChanged.AddListener(onValueChange);
            mouseSensitivitySlider.value = playerMouseLook.mouseSensitivity;
            mouseSensitivityText.text = playerMouseLook.mouseSensitivity.ToString();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            showInterface = !showInterface;
        }

        instructions.interactable = showInterface;
        instructions.alpha = showInterface ? 1 : 0;
        instructions.blocksRaycasts = showInterface;

        mouseSettings.interactable = showInterface;
        mouseSettings.alpha = showInterface ? 1 : 0;
        mouseSettings.blocksRaycasts = showInterface;
    }

    public void onValueChange(float value)
    {
        playerMouseLook.mouseSensitivity = mouseSensitivitySlider.value;
        mouseSensitivityText.text = playerMouseLook.mouseSensitivity.ToString();
    }
}
