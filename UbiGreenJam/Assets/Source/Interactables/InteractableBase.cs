using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class InteractableBase : MonoBehaviour
{
    [Header("Interactable Base Components")]

    [SerializeField]
    protected List<InteractTrigger> interactTriggers = new List<InteractTrigger>();

    [Header("Interactable Base Settings")]

    [SerializeField]
    [Range(1, 4)]
    protected int numCharactersInteractAllow = 1;

    [Header("Runtime ReadOnly Data")]

    [SerializeField]
    [ReadOnlyInspector]
    protected bool isCarryable = false;

    [SerializeField]
    [ReadOnlyInspector]
    public List<CharacterBase> charsInteractedWithList { get; protected set; } = new List<CharacterBase>();

    protected virtual void Awake()
    {
        foreach(InteractTrigger trigger in GetComponentsInChildren<InteractTrigger>())
        {
            if (!interactTriggers.Contains(trigger))
                interactTriggers.Add(trigger);

            trigger.SetInteractableParent(this);
        }
    }

    protected virtual void OnValidate() { }

    public virtual bool OnInteractBy(CharacterBase characterInteracted)
    {
        if (!characterInteracted) return false;

        if(charsInteractedWithList.Count == 0)
        {
            charsInteractedWithList.Add(characterInteracted);

            return true;
        }

        if (charsInteractedWithList.Contains(characterInteracted)) return true;

        if (charsInteractedWithList.Count >= numCharactersInteractAllow) return false;

        charsInteractedWithList.Add(characterInteracted);

        return true;
    }

    public virtual void OnReleaseBy(CharacterBase characterInteracted)
    {
        if (!characterInteracted) return;

        if (charsInteractedWithList.Contains(characterInteracted))
        {
            charsInteractedWithList.Remove(characterInteracted);

            return;
        }
    }
}
