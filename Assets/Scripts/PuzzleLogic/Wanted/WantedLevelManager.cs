using System.Collections.Generic;
using UnityEngine;

public class WantedLevelManager : MonoBehaviour
{
    [Header("Real Buttons")]
    [SerializeField] private List<WorldButton> correctButtons;

    [Header("Fakes")]
    [SerializeField] private FakeButton fakeButtonPrefab;
    [SerializeField] private List<NumberBlockData> fakeBlockDatas;
    [SerializeField] private int fakeCount = 20;

    [Tooltip("Keeps fake spawn positions this far inside the camera edges.")]
    [SerializeField] private float spawnEdgePadding = 0.75f;

    [SerializeField] private Camera boundsCamera;   // defaults to Camera.main

    public IReadOnlyList<WorldButton> CorrectButtons => correctButtons;

    private readonly List<NumberBlockView> allBlockViews = new List<NumberBlockView>();

    private void Awake()
    {
        if (boundsCamera == null)
            boundsCamera = Camera.main;
    }

    private void Start()
    {
        CollectRealButtonViews();
        SpawnFakes();
        AssignRandomSortingOrders();
    }

    private void CollectRealButtonViews()
    {
        foreach (WorldButton button in correctButtons)
        {
            NumberBlockView view = button.GetComponentInChildren<NumberBlockView>();
            if (view != null)
                allBlockViews.Add(view);
        }
    }

    private void SpawnFakes()
    {
        float halfHeight = boundsCamera.orthographicSize - spawnEdgePadding;
        float halfWidth = boundsCamera.orthographicSize * boundsCamera.aspect - spawnEdgePadding;
        Vector3 camPos = boundsCamera.transform.position;

        for (int i = 0; i < fakeCount; i++)
        {
            Vector3 spawnPos = new Vector3(
                camPos.x + Random.Range(-halfWidth, halfWidth),
                camPos.y + Random.Range(-halfHeight, halfHeight),
                0f);

            FakeButton fake = Instantiate(fakeButtonPrefab, spawnPos, Quaternion.identity, transform);

            if (fakeBlockDatas.Count > 0)
                fake.SetBlockData(fakeBlockDatas[Random.Range(0, fakeBlockDatas.Count)]);

            NumberBlockView view = fake.GetComponentInChildren<NumberBlockView>();
            if (view != null)
                allBlockViews.Add(view);
        }
    }

    private void AssignRandomSortingOrders()
    {
        for (int i = allBlockViews.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (allBlockViews[i], allBlockViews[j]) = (allBlockViews[j], allBlockViews[i]);
        }

        for (int k = 0; k < allBlockViews.Count; k++)
        {
            int top = -2 * k;
            allBlockViews[k].SetSortingOrders(top, top - 1);
        }
    }
}
