using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public InteractableItemData itemData;
    public bool destroyOnPickup = false;

    [Header("Runtime State")]
    public bool isBeingHeld = false;

    [Header("UI")]
    public InteractablePopupUI popupUI;

    [Tooltip("World-space offset for popup above the object")]
    public Vector3 popupOffset = new Vector3(0f, 0.6f, 0f);

    private bool promptVisible = false;
    private string lastName;
    private string lastPrompt;
    private int lastCost;

    public void ShowPrompt()
    {
        if (popupUI == null || isBeingHeld || itemData == null) return;

        // Build text
        string name = itemData.itemName;
        string prompt = itemData.isCarryable ? "E PICK UP" : "E OPEN";
        int cost = itemData.cost;

        // If already visible with same content, just update follow target and bail
        if (promptVisible && name == lastName && prompt == lastPrompt && cost == lastCost)
        {
            popupUI.SetFollowTarget(transform, popupOffset);
            return;
        }

        lastName = name;
        lastPrompt = prompt;
        lastCost = cost;

        promptVisible = true;
        popupUI.SetFollowTarget(transform, popupOffset);
        popupUI.Show(name, prompt, cost);
    }

    public void HidePrompt()
    {
        if (!promptVisible) return;
        promptVisible = false;
        popupUI?.Hide();
    }
}
