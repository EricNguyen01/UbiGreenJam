using UnityEngine;
using UnityEngine.UI;

public class UnderwaterEffects : MonoBehaviour
{
    [Header("Refs")]
    public FloodController flood;           // drag your FloodVolume controller here
    public Transform cam;                   // drag FPS camera here
    public Image underwaterOverlay;         // drag UnderwaterOverlay UI image here

    [Header("Thresholds")]
    public float enterOffset = 0.1f;        // how far below surface to count as underwater
    public float exitOffset = 0.2f;         // hysteresis so it doesn't flicker

    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color underwaterFogColor = new Color(0.1f, 0.35f, 0.4f, 1f);
    public float underwaterFogDensity = 0.05f;

    private bool underwater = false;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private bool defaultFogEnabled;

    void Start()
    {
        if (!flood) flood = FindAnyObjectByType<FloodController>();
        if (!cam) cam = Camera.main.transform;

        defaultFogEnabled = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;

        if (underwaterOverlay)
            underwaterOverlay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!flood || !cam) return;

        float waterY = flood.CurrentWaterSurfaceY();
        float camY = cam.position.y;

        if (!underwater && camY < waterY - enterOffset)
            SetUnderwater(true);

        if (underwater && camY > waterY + exitOffset)
            SetUnderwater(false);
    }

    void SetUnderwater(bool state)
    {
        underwater = state;

        // Overlay tint
        if (underwaterOverlay)
            underwaterOverlay.gameObject.SetActive(state);

        // Fog
        if (enableFog)
        {
            if (state)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = underwaterFogColor;
                RenderSettings.fogDensity = underwaterFogDensity;
            }
            else
            {
                RenderSettings.fog = defaultFogEnabled;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
            }
        }
    }
}