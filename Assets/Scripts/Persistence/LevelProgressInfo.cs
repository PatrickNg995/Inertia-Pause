using NUnit.Framework;
using System;
using System.Collections.Generic;

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

        // Update OptionalObjectivesCompletions based on the lengths of the arrays.
        if (newLevelInfo.OptionalObjectivesCompletions.Length == OptionalObjectivesCompletions.Length)
        {
            // Update OptionalObjectivesCompletions by performing a logical OR operation on each element if the array lengths match.
            OptionalObjectivesCompletions = LogicalORBoolArrays(OptionalObjectivesCompletions, newLevelInfo.OptionalObjectivesCompletions);
        }
        else if (newLevelInfo.OptionalObjectivesCompletions.Length > OptionalObjectivesCompletions.Length)
        {
            // If the new level info array is greater than the saved one (more objectives were added), merge the arrays by performing logical OR on overlapping elements,
            // then append the rest.
            // This should only happen if we add new objectives. If we do add more objectives, we have to make sure we keep the same order of the previous ones.
            OptionalObjectivesCompletions = MergeMismatchedObjectiveArrays(newLevelInfo.OptionalObjectivesCompletions, OptionalObjectivesCompletions);
        }
        else if (newLevelInfo.OptionalObjectivesCompletions.Length < OptionalObjectivesCompletions.Length)
        {
            // If the new level info array length is less than the saved one (there are now less objectives), perform logical OR on the overlapping elements, since the
            // method won't include the extra saved data.
            // This should only happen if we remove objectives. If we do remove objectives, the remaining objectives should still be the same.
            OptionalObjectivesCompletions = LogicalORBoolArrays(newLevelInfo.OptionalObjectivesCompletions, OptionalObjectivesCompletions);
        }
    }

    private bool[] MergeMismatchedObjectiveArrays(bool[] array1, bool[] array2)
    {
        int minLength = Math.Min(array1.Length, array2.Length);
        int maxLength = Math.Max(array1.Length, array2.Length);
        List<bool> resultList = new List<bool>();

        // Perform logical OR on the overlapping elements, then append the rest.
        if (array1.Length < array2.Length)
        {
            // Perform logical OR on overlapping elements and add them to list.
            resultList.AddRange(LogicalORBoolArrays(array1, array2));

            // Append the rest of array2 to the result list.
            int appendLength = array2.Length - minLength;
            bool[] subsetArray = new bool[appendLength];
            Array.Copy(array2, minLength, subsetArray, 0, appendLength);
            resultList.AddRange(subsetArray);
        }
        else
        {
            // Perform logical OR on overlapping elements and add them to list.
            resultList.AddRange(LogicalORBoolArrays(array2, array1));

            // Append the rest of array1 to the result list.
            int appendLength = array1.Length - minLength;
            bool[] subsetArray = new bool[appendLength];
            Array.Copy(array1, minLength, subsetArray, 0, appendLength);
            resultList.AddRange(subsetArray);
        }

        return resultList.ToArray();
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
