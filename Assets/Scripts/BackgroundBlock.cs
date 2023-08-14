using System.Collections;
using UnityEngine;

public enum BlockState { Empty = 0, Fill = 1 };

public class BackgroundBlock : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public BlockState BlockState { private set; get; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BlockState = BlockState.Empty;
    }
    
    public void FillBlock(Color color)
    {
        BlockState = BlockState.Fill;
        spriteRenderer.color = color;
    }
}
