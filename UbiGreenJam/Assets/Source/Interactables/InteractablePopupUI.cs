using TMPro;
using UnityEngine;

public class InteractablePopupUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI nameText;
    public GameObject popup;

    public void Show(string name, string message)
    {
        nameText.text = name;
        promptText.text = message;
        popup.SetActive(true);
    }

    public void Hide()
    {
        popup.SetActive(false);
    }
}
