using UnityEngine;

[CreateAssetMenu(menuName = "Game/StormData", fileName = "NewStormData")]
public class ScriptableStormData : ScriptableObject
{
    [field: Header("Storm General Data")]

    [field: SerializeField]
    public string stormName { get; private set; } = "DefaultStorm";

    [field: SerializeField]
    [field: Min(1.0f)]
    public float duration { get; private set; } = 10f;

    public enum StormDamageType
    {
        Fixed = 0,// damage does not change throughout entire duration of the storm
        PerTick = 1,// damage will change during storm's duration
    }

    [field: Header("Storm Damage Settings")]

    [field: SerializeField]
    public StormDamageType stormDamageType { get; private set; } = StormDamageType.PerTick;

    [field: SerializeField]
    [field: Min(1.0f)]
    [field: Tooltip("Damage multiplier that will be applied after all other damage modifications. " +
    "Set to value other than 1 if you want to change the overall damage of the storm specifically for a round.")]
    public float roundDamageMultiplier { get; private set; } = 1.0f;

    [field: Header("Fixed Damage Settings")]
    [field: SerializeField]
    [field: Min(1.0f)]
    public float stormFixedDamage { get; private set; } = 5.0f;

    [field: Header("Damage Per Tick Settings")]

    [field: SerializeField]
    [field: Min(1.0f)]
    public float damagePerTick { get; private set; } = 1.0f;

    private bool applyDamageMultAfterTicks = false;

    [field: SerializeField]
    [field: Min(1.0f)]
    [field: DisableIf("applyDamageMultAfterTicks", false)]
    public float numberOfTicksToApplyDamageMult = 5.0f;// how many ticks should pass before the damage multiplier below is applied

    [field: SerializeField]
    [field: Min(1.0f)]
    [field: DisableIf("applyDamageMultAfterTicks", false)]
    public float damageMultToApplyAfterTicks = 1.5f;// the damage multiplier after number of ticks have passed (if allowed)

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(stormDamageType == StormDamageType.PerTick)
        {
            applyDamageMultAfterTicks = true;
        }
        else applyDamageMultAfterTicks = false;
    }
#endif

    // Add more parameters: spawn patterns, triggers, thresholds...
}
