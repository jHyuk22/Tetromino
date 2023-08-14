using UnityEngine;

public class NoneDragBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private int orderInLayer;

    public BackgroundBlock[] SpawnBlocks(Vector2Int blockCount, Vector2 blockHalf)
    {
        BackgroundBlock[] blocks = new BackgroundBlock[blockCount.x * blockCount.y];
        for (int y = 0; y < blockCount.y; ++y)
        {
            for (int x = 0; x < blockCount.x; ++ x)
            {
                float px = -blockCount.x * 0.5f + blockHalf.x + x;
                float py = blockCount.y * 0.5f - blockHalf.y - y;
                Vector3 position = new Vector3(px, py, 0);

                GameObject clone = Instantiate(blockPrefab, position, Quaternion.identity, transform);
                clone.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
                blocks[y*blockCount.x + x] = clone.GetComponent<BackgroundBlock>();
            }
        }
        return blocks;
    }
}
