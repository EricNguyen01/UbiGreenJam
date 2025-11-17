using UnityEngine;

[DisallowMultipleComponent]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character SO Data")]

    [SerializeField]
    public CharacterSOBase characterSOData;

    [Header("Character Base Components")]

    [SerializeField]
    protected CharacterController characterController;

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

        if (!characterController)
        {
            Debug.LogError($"Character {name} is missing its Character Controller component. " +
                           "One will be added but the character and its movement might not work correctly!");

            characterController = gameObject.AddComponent<CharacterController>();
        }
    }
}
