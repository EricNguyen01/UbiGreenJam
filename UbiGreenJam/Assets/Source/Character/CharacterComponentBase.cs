using UnityEngine;

[DisallowMultipleComponent]
public abstract class CharacterComponentBase : MonoBehaviour
{
    [Header("Character Component Base Data")]

    [SerializeField]
    [ReadOnlyInspector]
    protected CharacterBase characterUsingComponent;

    public virtual bool InitCharacterComponentFrom(CharacterBase character)
    {
        if (!character || !character.characterSOData)
        {
            enabled = false;

            return false;
        }

        characterUsingComponent = character;

        return true;
    }
}
