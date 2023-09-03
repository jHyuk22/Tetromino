using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BlockArrangeSystem : MonoBehaviour
{
    private Vector2Int blockCount;
    private Vector2 blockHalf;
    private BackgroundBlock[] backgroundBlocks;
    private StageController stageController;

    public void Setup(Vector2Int blockCount, Vector2 blockHalf, BackgroundBlock[] backgroundBlocks, StageController stageController)
    {
        this.blockCount = blockCount;
        this.blockHalf = blockHalf;
        this.backgroundBlocks = backgroundBlocks;
        this.stageController = stageController;
    }

    public bool TryArrangementBlock(DragBlock block)
    {
        //Debug.Log("TryArrangeBlock(transform.position)" + block.transform.position);
        for (int i = 0; i < block.ChildBlocks.Length; ++i)
        {
            Vector3 position = block.transform.position + block.ChildBlocks[i];
            if (!IsBlockInsideMap(position)) return false;
            if (!IsOtherBlockInThisBlock(position)) return false;
        }

        for (int i = 0; i < block.ChildBlocks.Length; ++i)
        {
            Vector3 position = block.transform.position + block.ChildBlocks[i];
            //Debug.Log("TryArrangementBlock(transform.position): " + block.transform.position);
            //Debug.Log("TryArrangementBlock(ChildBlocks["+i+"]: " + block.ChildBlocks[i]);
            //Debug.Log("TryArrangementBlock(position): " + position);
            //Debug.Log("Index: " + PositionToIndex(position));
            backgroundBlocks[PositionToIndex(position)].FillBlock(block.Color);
        }

        stageController.AfterBlockArrangement(block);

        return true;
    }

    private bool IsBlockInsideMap(Vector3 position)
    {
        if(position.x<-blockCount.x * 0.5f + blockHalf.x || position.x>blockCount.x*0.5f-blockHalf.x||position.y <-blockCount.y*0.5f + blockHalf.y || position.y>blockCount.y*0.5f - blockHalf.y)
        {
            return false;
        }

        return true;
    }

    private int PositionToIndex(Vector3 position)
    {
        float x = blockCount.x * 0.5f - blockHalf.x + position.x; //4.5+position.x
        float y = blockCount.y * 0.5f - blockHalf.y - position.y; //4.5-position.y

        return (int)((y * blockCount.x + x)+0.5f);  //49.5-10*position.y + position.x
        //return Mathf.RoundToInt(y * blockCount.x + x);
    }

    private bool IsOtherBlockInThisBlock(Vector2 position)
    {
        int index = PositionToIndex(position);

        if (backgroundBlocks[index].BlockState == BlockState.Fill)
        {
            return false;
        }

        return true;
    }

    public bool IsPossibleArrangement(DragBlock block)
    {
        for (int y = 0; y < blockCount.y; ++y)
        {
            for (int x = 0; x < blockCount.x; ++x)
            {
                /*Vector3 position = new Vector3(-blockCount.x * 0.5f + blockHalf.x + x, blockCount.y * 0.5f - blockHalf.y - y, 0);

                position.x = block.BlockCount.x % 2 == 0 ? position.x + 0.5f : position.x;
                position.y = block.BlockCount.y % 2 == 0 ? position.y - 0.5f : position.y;*/
                for (int j = 0; j < 4; j++)     //미노의 0, 90, 180, 270도 회전에 대한 검사
                {
                    int count = 0;
                    int BlockCountX = block.BlockCount.x;
                    int BlockCountY = block.BlockCount.y;
                    int tmp;

                    Vector3 position = new Vector3(-blockCount.x * 0.5f + blockHalf.x + x, blockCount.y * 0.5f - blockHalf.y - y, 0);

                    if (j % 2 != 0)
                    {
                        tmp = BlockCountX;
                        BlockCountX = BlockCountY;
                        BlockCountY = tmp;
                    }

                    position.x = BlockCountX % 2 == 0 ? position.x + 0.5f : position.x;
                    position.y = BlockCountY % 2 == 0 ? position.y - 0.5f : position.y;

                    for (int i = 0; i < block.ChildBlocks.Length; ++i)      //미노의 각 자식 오브젝트 위치에 대한 검사
                    {
                        //Vector3 blockPosition = block.ChildBlocks[i] + position;
                        Vector3 blockPosition = CheckRotate(block.ChildBlocks[i], j) + position;
                        if (!IsBlockInsideMap(blockPosition))
                        {
                            Debug.Log("Block isn't inside the map! i: " + i + ", j: " + j +", Index: " + PositionToIndex(blockPosition) + blockPosition);
                            break;
                        }
                        if (!IsOtherBlockInThisBlock(blockPosition))
                        {
                            Debug.Log("Other Block in this block!" + i + ", j: " + j +", Index: " + PositionToIndex(blockPosition) + blockPosition);
                            break;
                        }
                        count++;
                    }

                    if (count == block.ChildBlocks.Length)
                    {
                        Debug.Log("y: " + y + ", x: " + x + ", j: " + j + " 에서 배치 가능");
                        Debug.Log("그 떄의 부모 오브젝트 좌표는: " + position);
                        for(int i=0; i<4; i++)
                        {
                            Vector3 debugPostion = CheckRotate(block.ChildBlocks[i], j) + position;
                            Debug.Log("그때의 자식오브젝트 좌표는 block.ChildBlocks[" + i + "]: " + debugPostion);
                            Debug.Log("그때의 자식오브젝트 인덱스는 block.ChildBlocks[" + i + "]: " + PositionToIndex(debugPostion));
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private Vector3 CheckRotate(Vector3 position, int i)
    {
        Vector3 newPosition = new Vector3(0.0f, 0.0f, 0.0f);

        switch (i)
        {
            case 0:
                newPosition = position;
                break;
            case 1:
                newPosition.x = position.y * -1;
                newPosition.y = position.x;
                break;
            case 2:
                newPosition.x = position.x * -1;
                newPosition.y = position.y * -1;
                break;
            case 3:
                newPosition.x = position.y;
                newPosition.y = position.x * -1;
                break;
        }
         return newPosition;
    }
}
