using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem;
    [SerializeField]
    private Transform[] blockSpawnPoints;
    [SerializeField]
    private GameObject[] blockPrefabs;
    [SerializeField]
    private Vector3 spawnGapAmount = new Vector3(0, -10, 0);
    [SerializeField]
    private GameObject rotateButtonPrefab;


    public Transform[] BlockSpawnPoints => blockSpawnPoints;

    public void SpawnBlocks()
    {
        StartCoroutine("OnSpawnBlocks");
    }

    private List<int> nextSpawnBlockIndex = new List<int>();

    private void initList()
    {
        for (int i = 0; i < blockPrefabs.Length; i++)
        {
            nextSpawnBlockIndex.Add(i);
        }
    }


    private IEnumerator OnSpawnBlocks()
    {
        for (int i = 0; i < blockSpawnPoints.Length; i++)
        {
            if (nextSpawnBlockIndex.Count == 0)
            {
                initList();
            }
            yield return new WaitForSeconds(0.1f);

            int randomIndex = Random.Range(0, nextSpawnBlockIndex.Count);
            int randomValue = nextSpawnBlockIndex[randomIndex];
            nextSpawnBlockIndex.RemoveAt(randomIndex);

            Vector3 spawnPosition = blockSpawnPoints[i].position + spawnGapAmount;
            GameObject clone = Instantiate(blockPrefabs[randomValue], spawnPosition, Quaternion.identity, blockSpawnPoints[i]);

            clone.GetComponent<DragBlock>().Setup(blockArrangeSystem, blockSpawnPoints[i].position);
        }
    }

    /*private void spawnRotationButton(GameObject clone)
    {
        for (int i = 0; i < blockSpawnPoints.Length; i++)
        {
            Vector3 buttonPosition = blockSpawnPoints[i].position + new Vector3(0, -2, 0);  // 버튼 위치 조절. -2는 예시입니다.
            GameObject buttonClone = Instantiate(rotateButtonPrefab, buttonPosition, Quaternion.identity, blockSpawnPoints[i]);
            buttonClone.GetComponent<RotationButton>().SetTargetBlock(clone.transform);
        }
    }*/
}
