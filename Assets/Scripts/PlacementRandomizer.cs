using UnityEngine;
using System.Collections.Generic;

public class PlacementRandomizer : MonoBehaviour
{
    private void Start()
    {
        RandomizeParentPositions();
    }

    private void RandomizeParentPositions()
    {
        NumberBlockView[] numberBlocks = FindObjectsByType<NumberBlockView>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        List<Transform> parents = new List<Transform>();
        foreach (NumberBlockView numberBlock in numberBlocks)
        {
            Transform parent = numberBlock.transform.parent;

            if (parent != null && !parents.Contains(parent))
            {
                parents.Add(parent);
            }
        }
        List<Vector3> originalPositions = new List<Vector3>();
        foreach (Transform parent in parents)
        {
            originalPositions.Add(parent.position);
        }
        List<Vector3> shuffledPositions = new List<Vector3>(originalPositions);
        bool validShuffle = false;
        while (!validShuffle)
        {
            Shuffle(shuffledPositions);
            validShuffle = true;
            for (int i = 0; i < parents.Count; i++)
            {
                if (shuffledPositions[i] == originalPositions[i])
                {
                    validShuffle = false;
                    break;
                }
            }
        }
        for (int i = 0; i < parents.Count; i++)
        {
            parents[i].position = shuffledPositions[i];
        }
    }
    private void Shuffle(List<Vector3> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            Vector3 temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}