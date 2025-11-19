using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public InteractableItemData itemData;
    public bool destroyOnPickup = false;

    [Header("Runtime State")]
    public bool isBeingHeld = false;

    private void Reset()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
        }
    }
}
