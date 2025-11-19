using System;

[Serializable]
public class LevelProgressInfo
{
    public string LevelEnvironmentName;
    public string LevelAssetsName;
    public int PersonalBestActionCount;
    public bool[] OptionalObjectivesCompletions;

    public LevelProgressInfo(string levelEnvironmentName, string levelAssetsName, int actionCount, bool[] objectiveCompletions)
    {
        LevelEnvironmentName = levelEnvironmentName;
        LevelAssetsName = levelAssetsName;
        PersonalBestActionCount = actionCount;
        OptionalObjectivesCompletions = objectiveCompletions;
    }

    public void UpdateProgress(LevelProgressInfo newLevelInfo)
    {
        // Update ActionCount to the lowest value.
        if (newLevelInfo.PersonalBestActionCount < PersonalBestActionCount)
        {
            PersonalBestActionCount = newLevelInfo.PersonalBestActionCount;
        }

        if (OptionalObjectivesCompletions.Length == newLevelInfo.OptionalObjectivesCompletions.Length)
        {
            // Update OptionalObjectivesCompletions by performing a logical OR operation on each element if the array lengths match.
            OptionalObjectivesCompletions = LogicalORBoolArrays(OptionalObjectivesCompletions, newLevelInfo.OptionalObjectivesCompletions);
        }
        else
        {
            // If the lengths don't match, merge the arrays by performing logical OR on overlapping elements, then appending the rest.
            // This should only happen if we add new objectives. If we do add more objectives, we have to make sure we keep the same order of the previous ones.
            OptionalObjectivesCompletions = MergeMismatchedObjectiveArrays(newLevelInfo.OptionalObjectivesCompletions, OptionalObjectivesCompletions);
        }
    }

    private bool[] MergeMismatchedObjectiveArrays(bool[] array1, bool[] array2)
    {
        int minLength = Math.Min(array1.Length, array2.Length);
        bool[] resultArray = new bool[Math.Max(array1.Length, array2.Length)];

        // Perform logical OR on the overlapping elements, then append the rest.
        if (array1.Length < array2.Length)
        {
            resultArray = LogicalORBoolArrays(array1, array2);
            Array.Copy(array2, minLength, resultArray, minLength, array2.Length - minLength);
        }
        else
        {
            resultArray = LogicalORBoolArrays(array2, array1);
            Array.Copy(array1, minLength, resultArray, minLength, array1.Length - minLength);
        }

        return resultArray;
    }

    private bool[] LogicalORBoolArrays(bool[] array1, bool[] array2)
    {
        bool[] resultArray = new bool[array1.Length];

        for (int i = 0; i < array1.Length; i++)
        {
            resultArray[i] = array1[i] || array2[i];
        }

        return resultArray;
    }
}
