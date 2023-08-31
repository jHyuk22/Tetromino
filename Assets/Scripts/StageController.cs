using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private NoneDragBlockSpawner backgroundBlockSpawner;
    [SerializeField]
    private NoneDragBlockSpawner foregroundBlockSpawner;
    [SerializeField]
    private DragBlockSpawner dragBlockSpawner;
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem;
    [SerializeField]
    private UIController uiController;
    [SerializeField]
    private SliderTimer sliderTimer;

    private BackgroundBlock[] backgroundBlocks;
    private int currentDragBlockCount;
    public int CurrentScore { private set; get; }
    public int HighScore { private set; get; }
    public Slider slTimer;
    public bool isNewBest;

    private readonly Vector2Int blockCount = new Vector2Int(10, 10);
    private readonly Vector2 blockHalf = new Vector2(0.5f, 0.5f);
    private readonly int maxDragBlockCount = 3;

    private void Awake()
    {
        CurrentScore = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");
        isNewBest = false;

        backgroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        backgroundBlocks = new BackgroundBlock[blockCount.x * blockCount.y];
        backgroundBlocks = foregroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        blockArrangeSystem.Setup(blockCount, blockHalf, backgroundBlocks, this);
        /*for (int i=0; i<100; i++)
        {
            if (i % 10 > 3 || i >= 40)
            {
                Color color = Color.yellow;
                backgroundBlocks[i].FillBlock(color);
            }
        }*/
        StartCoroutine(SpawnDragBlocks());
    }

    private IEnumerator SpawnDragBlocks()
    {
        currentDragBlockCount = maxDragBlockCount;
        dragBlockSpawner.SpawnBlocks();

        yield return new WaitUntil(() => IsCompleteSpawnBlocks());
    }

    private bool IsCompleteSpawnBlocks()
    {
        int count = 0;
        for (int i=0; i < dragBlockSpawner.BlockSpawnPoints.Length; ++i)
        {
            if (dragBlockSpawner.BlockSpawnPoints[i].childCount != 0 && dragBlockSpawner.BlockSpawnPoints[i].GetChild(0).localPosition == Vector3.zero)
            {
                count++;
            }
        }

        return count == dragBlockSpawner.BlockSpawnPoints.Length;
    }
    public void AfterBlockArrangement(DragBlock block)
    {
        StartCoroutine("OnAfterBlockArrangement", block);
    }

    private IEnumerator OnAfterBlockArrangement(DragBlock block)
    {
        Destroy(block.gameObject);

        CurrentScore += block.ChildBlocks.Length;

        currentDragBlockCount--;
        if(currentDragBlockCount == 0)
        {
            yield return StartCoroutine(SpawnDragBlocks());
        }

        yield return new WaitForEndOfFrame();

        if (IsGameOver())
        {
            CurrentScore += (int)slTimer.value;
            if(CurrentScore > HighScore)
            {
                PlayerPrefs.SetInt("HighScore", CurrentScore);
                isNewBest = true;
            }

            uiController.GameOver();
        }
    }

    public void EndGame()
    {
        if (CurrentScore > HighScore)
        {
            PlayerPrefs.SetInt("HighScore", CurrentScore);
            isNewBest = true;
        }

        sliderTimer.StopTimer();
        uiController.GameOver();
    }

    public bool IsGameOver()
    {
        int dragBlockCount = 0;
        for(int i=0; i < dragBlockSpawner.BlockSpawnPoints.Length; ++i)
        {
            if (dragBlockSpawner.BlockSpawnPoints[i].childCount != 0)
            {
                dragBlockCount++;
                Debug.Log("IsGameOver(BlockSpawnPoints[" + i + "])");
                if (blockArrangeSystem.IsPossibleArrangement(dragBlockSpawner.BlockSpawnPoints[i].GetComponentInChildren<DragBlock>()))
                {
                    return false;
                }
            }
        }
        if (slTimer.value <= 0.0f || dragBlockCount != 0) // Check if timer has expired or blocks cannot be arranged
        {
            EndGame();
            return true;
        }

        return false;
    }
}