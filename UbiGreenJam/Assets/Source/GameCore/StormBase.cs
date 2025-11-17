using System;
using UnityEngine;

/// <summary>
/// Runtime controller created from ScriptableStormData
/// Contains runtime logic of the storm (tick, triggers, finished)
/// </summary>
public class StormBase
{
    private ScriptableStormData _data;
    private float _elapsed;
    public bool IsFinished { get; private set; }

    public StormBase(ScriptableStormData data)
    {
        _data = data;
        _elapsed = 0f;
        IsFinished = false;
    }

    public void StartStorm()
    {
        _elapsed = 0f;
        IsFinished = false;
        // optionally spawn initial triggers
    }

    public void Tick(float delta)
    {
        if (IsFinished) return;

        _elapsed += delta;
        // Here you can process triggers, spawn events, use intensity etc.
        if (_elapsed >= _data.duration)
        {
            IsFinished = true;
        }
    }

    public void Reset()
    {
        _elapsed = 0f;
        IsFinished = false;
    }
}
