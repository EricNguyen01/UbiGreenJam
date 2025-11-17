using UnityEngine;

[CreateAssetMenu(menuName = "Game/StormData", fileName = "NewStormData")]
public class ScriptableStormData : ScriptableObject
{
    public string stormName = "DefaultStorm";
    public float duration = 10f;
    public int intensity = 1;

    // Add more parameters: spawn patterns, triggers, thresholds...
}
