using System.Collections.Generic;
using UnityEngine;

public static class GameProgress
{
    public enum Result { None, Completed, Failed }

    public static int CompletedCount = 0;

    public static Result LastResult = Result.None;

    public static readonly List<LEVEL> UsedLevels = new List<LEVEL>();

    public static List<LEVEL> CurrentLevels = new List<LEVEL>();

    public static void ResetRun()
    {
        CompletedCount = 0;
        UsedLevels.Clear();
    }
}