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

        return Mathf.RoundToInt(y * blockCount.x + x);  //49.5-10*position.y + position.x
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
                int count = 0;
                Vector3 position = new Vector3(-blockCount.x * 0.5f + blockHalf.x + x, blockCount.y * 0.5f - blockHalf.y - y, 0);

                position.x = block.BlockCount.x % 2 == 0 ? position.x + 0.5f : position.x;
                position.y = block.BlockCount.y % 2 == 0 ? position.y - 0.5f : position.y;
                for (int i = 0; i < block.ChildBlocks.Length; ++i)
                {
                    Vector3 blockPosition = block.ChildBlocks[i] + position;
                    if (!IsBlockInsideMap(blockPosition)) break;
                    if (!IsOtherBlockInThisBlock(blockPosition)) break;

                    count++;
                }

                if (count == block.ChildBlocks.Length)
                {
                    return true;
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
                newPosition.y = position.x * -1;
                break;
            case 2:
                newPosition.x = position.x;
                newPosition.y = position.y * -1;
                break;
            case 3:
                newPosition.x = position.y;
                newPosition.y = position.x * -1;
                break;
        }

        //Debug.Log("CheckRotate(newPosition): " + newPosition);
        return newPosition;
    }
}
