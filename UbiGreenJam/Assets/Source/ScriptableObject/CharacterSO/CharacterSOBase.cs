using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSOBase", menuName = "Scriptable Objects/CharacterSOBase")]
public class CharacterSOBase : ScriptableObject
{
    [field: Header("Character Base Data")]

    [field: SerializeField]
    public bool isAICharacter { get; private set; } = false;

    [field: SerializeField]
    [field: Min(0.0f)]
    public float health { get; set; } = 100.0f;

    [field: SerializeField]
    [field: Min(1.0f)]
    public float speed { get; set; } = 12.0f;

    [field: SerializeField]
    [field: Min(5.0f)]
    public float jumpHeight { get; set; } = 10.0f;

    [field: SerializeField]
    [field: Min(1.0f)]
    public float fixSpeed { get; set; } = 4.0f;
}
