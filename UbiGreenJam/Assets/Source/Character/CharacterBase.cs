using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterMovement))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character SO Data")]

    [SerializeField]
    public CharacterSOBase characterSOData;

    [Header("Character Base Components")]

    [SerializeField]
    protected Camera characterCamera;

    [SerializeField]
    protected CharacterMovement characterMovement;

    [field: Header("Character Runtime Data")]

    [field: SerializeField]
    [field: ReadOnlyInspector]
    public CarryableItem playerHoldingItem { get; private set; }


    protected virtual void Awake()
    {
        if (!characterSOData)
        {
            Debug.LogError($"Fatal Error: Character {name} is missing its Character SO Data. " +
                           "Character won't work! Disabling character...");

            gameObject.SetActive(false);

            enabled = false;

            return;
        }

        characterSOData = Instantiate(characterSOData);

        if (!characterMovement)
        {
            TryGetComponent<CharacterMovement>(out characterMovement);
        }

        if (!characterMovement)
        {
            Debug.LogError($"Character {name} is missing its Character Movement component. " +
                           "One will be added but the character and its movement might not work correctly!");

            characterMovement = gameObject.AddComponent<CharacterMovement>();
        }

        characterMovement.InitCharacterComponentFrom(this);
    }
}
