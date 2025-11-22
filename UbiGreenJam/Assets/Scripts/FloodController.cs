using UnityEngine;

public class FloodController : MonoBehaviour
{
    [Header("Flood Settings")]
    public float maxHeight = 3f;       // world units above base
    public float riseSpeed = 0.5f;     // units per second when rising
    public float lowerSpeed = 0.5f;    // if you want water to go down
    public PlayerFloodDetector playerFlood;

    [Header("Debug")]
    public bool startRisingOnPlay = false;

    float baseY;        // where the bottom of the water is
    float currentHeight = 0f;
    bool isRising = false;
    bool isLowering = false;

    void Start()
    {
        if (!playerFlood)
            playerFlood = FindObjectOfType<PlayerFloodDetector>();

        baseY = transform.position.y;

        if (startRisingOnPlay)
            isRising = true;

        UpdateScaleAndPosition();
    }

    void Update()
    {
        if (playerFlood)
        {
            playerFlood.waterLevelY = CurrentWaterSurfaceY();
        }

        float dt = Time.deltaTime;

        if (isRising)
        {
            currentHeight += riseSpeed * dt;
            if (currentHeight >= maxHeight)
            {
                currentHeight = maxHeight;
                isRising = false; // reached max
            }
            UpdateScaleAndPosition();
        }
        else if (isLowering)
        {
            currentHeight -= lowerSpeed * dt;
            if (currentHeight <= 0f)
            {
                currentHeight = 0f;
                isLowering = false;
            }
            UpdateScaleAndPosition();
        }
    }

    void UpdateScaleAndPosition()
    {
        // keep bottom at baseY while changing height
        float height = Mathf.Max(currentHeight, 0.01f); // avoid zero scale
        Vector3 scale = transform.localScale;
        scale.y = height;
        transform.localScale = scale;

        // cube pivot is in the center, so offset so bottom stays at baseY
        Vector3 pos = transform.position;
        pos.y = baseY + (height * 0.5f);
        transform.position = pos;
    }

    // --- Public controls for other scripts (Storm manager etc.) ---

    public void StartFlood(float speedMultiplier = 1f)
    {
        isLowering = false;
        isRising = true;
        // optional: temporary speed change
        // riseSpeed *= speedMultiplier;
    }

    public void StopFlood()
    {
        isRising = false;
    }

    public void StartLowering()
    {
        isRising = false;
        isLowering = true;
    }

    public float GetNormalizedFloodLevel()
    {
        // 0 = dry, 1 = maxHeight
        return Mathf.InverseLerp(0f, maxHeight, currentHeight);
    }

    public float CurrentWaterSurfaceY()
    {
        return baseY + currentHeight;
    }
}
