using UnityEngine;

[DisallowMultipleComponent]
public class InteractTrigger : MonoBehaviour, IInteractable
{
    [Header("Trigger Collider Component")]

    [SerializeField]
    public Collider triggerCollider { get; private set; }

    [Header("Trigger Collider Interactable Parent")]

    [SerializeField]
    [ReadOnlyInspector]
    private InteractableBase interactableUsingTrigger;

    [field: Header("Trigger Runtime Data")]

    [field: SerializeField]
    [field: ReadOnlyInspector]
    public bool isDisabled { get; private set; } = false;

    [field: SerializeField]
    [field: ReadOnlyInspector]
    public bool isBeingInteractedWith { get; private set; } = false;


    [field: SerializeField]
    [field: ReadOnlyInspector]
    [field: Tooltip("Runtime player interaction data read-only. " +
    "1 interact trigger should only be interacted by 1 player only")]
    public CharacterBase characterInteractingWithTrigger { get; private set; }

    private void Awake()
    {
        if (!triggerCollider)
        {
            triggerCollider = GetComponent<Collider>();
        }

        if(!triggerCollider)
        {
            triggerCollider = gameObject.AddComponent<Collider>();

            Bounds bounds = triggerCollider.bounds;

            bounds.size = new Vector3(10.0f, 10.0f, 10.0f);
        }

        if(!triggerCollider.isTrigger) triggerCollider.isTrigger = true;
    }

    private void OnValidate()
    {
        if (!triggerCollider)
        {
            Debug.LogError($"InteractTrigger: {name} is missing its trigger collider component!");
        }
    }

    private void Start()
    {
        if (!interactableUsingTrigger)
        {
            gameObject.SetActive(false);

            enabled = false;
        }
    }

    public void SetInteractableParent(InteractableBase interactable)
    {
        if (!interactable) return;

        interactableUsingTrigger = interactable; 
        
        if(!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (!enabled) enabled = true;
    }

    public void DisableInteractTrigger(bool shouldDisable)
    {
        isDisabled = shouldDisable;

        if (shouldDisable)
        {
            if (isBeingInteractedWith) 
                ForceReleaseFromInteractTrigger();
        }
    }

    //IInteractable Interface Functions ----------------------------------------------------------------------------

    public InteractableBase GetInteractable()
    {
        return interactableUsingTrigger;
    }

    public bool OnInteractBy(CharacterBase characterInteracted)
    {
        if(!enabled) return false;

        if (!characterInteracted) return false;

        if(!interactableUsingTrigger) return false;

        if (isDisabled) return false;

        interactableUsingTrigger.OnInteractBy(characterInteracted);

        characterInteractingWithTrigger = characterInteracted;

        isBeingInteractedWith = true;

        return true;
    }

    public void OnReleaseBy(CharacterBase characterInteracted)
    {
        if (!characterInteracted) return;

        if (!interactableUsingTrigger) return;

        if(characterInteractingWithTrigger && characterInteractingWithTrigger == characterInteracted)
        {
            characterInteractingWithTrigger = null;

            interactableUsingTrigger.OnReleaseBy(characterInteracted);

            isBeingInteractedWith = false;

            return;
        }

        Debug.LogWarning($"InteractTrigger: {name} is trying to release/detach from a character that is either null or is not the one that has interacted with this trigger. " +
                         "This should not happen and smth must be wrong!");
    }

    public void ForceReleaseFromInteractTrigger()
    {
        if(!isBeingInteractedWith) return;

        OnReleaseBy(characterInteractingWithTrigger);
    }
}
