using UnityEngine;

public class CarryableItem : InteractableBase
{
    [Header("Carryable Item Settings")]

    [SerializeField]
    [Range(1, 4)]
    protected int numPlayerRequiredToCarry = 1;

    protected override void Awake()
    {
        base.Awake();

        isCarryable = true;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!isCarryable) isCarryable = true;

        if (numPlayerRequiredToCarry > numCharactersInteractAllow) 
            numPlayerRequiredToCarry = numCharactersInteractAllow;
    }

    public override bool OnInteractBy(CharacterBase characterInteracted)
    {
        if(!base.OnInteractBy(characterInteracted)) return false;

        //TODO: Carryable logic here...

        return true;
    }

    public override void OnReleaseBy(CharacterBase characterInteracted)
    {
        base.OnReleaseBy(characterInteracted);

        //TODO: Release carryable logic here...
    }
}
