using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public InteractableItemData itemData;
    public bool destroyOnPickup = false;

    [Header("Runtime State")]
    public bool isBeingHeld = false;
    [Header("UI")]
    public InteractablePopupUI popupUI;

    public void ShowPrompt()
    {
        if (popupUI == null || isBeingHeld) return;
        popupUI.transform.position = transform.position + Vector3.up * 0.01f;
        popupUI.transform.localScale = Vector3.one * 0.005f;
        string name = itemData.itemName;
        string prompt = itemData.isCarryable ? "E PICK UP" : "E OPEN";
        popupUI.Show(name, prompt);
    }

    public void HidePrompt()
    {
        popupUI?.Hide();
    }
}
