using UnityEngine;

public static class TimeFormat
{
    public static string Format(int centiseconds)
    {
        int c = Mathf.Max(0, centiseconds);
        return $"{c / 6000:00}:{(c / 100) % 60:00}.{c % 100:00}";
    }
}