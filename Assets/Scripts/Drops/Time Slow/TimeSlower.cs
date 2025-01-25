using UnityEngine;

/// <summary>
/// Global or static data to manage the time-slow factor for enemies/bullets.
/// By default, Factor = 1f (normal speed). 
/// </summary>
public static class TimeSlower
{
    public static float SlowFactor { get; set; } = 1f; // Default to 1 (normal time)
}
