using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curveMovement;
    [SerializeField]
    private AnimationCurve curveScale;

    private BlockArrangeSystem blockArrangeSystem;
    private bool pivotFlag = false;

    private float appealTIme = 0.5f;
    private float returnTime = 0.1f;

    [field:SerializeField]
    public Vector2Int BlockCount { private set; get; }

    public Color Color { private set; get; }
    public Vector3[] ChildBlocks { private set; get; }

    public void Setup(BlockArrangeSystem blockArrangeSystem, Vector3 parentPosition)
    {
        this.blockArrangeSystem = blockArrangeSystem;
        Color = GetComponentInChildren<SpriteRenderer>().color;

        ChildBlocks = new Vector3[transform.childCount];
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i] = transform.GetChild(i).localPosition;
        }
        StartCoroutine(OnMoveTo(parentPosition, appealTIme));
    }

    private void OnMouseDown()
    {
        StopCoroutine("OnScaleTo");
        StartCoroutine("OnScaleTo", Vector3.one);
    }

    private void OnMouseDrag()
    {
        Vector3 gap = new Vector3(0, BlockCount.y * 0.5f + 1, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }

    private void OnMouseUp()        //블럭개수말고 회전여부로 확인하는방법에 대해 생각해봅시다.......
    {
        Debug.Log("OnMouseUp반올림이전(transform.position): " + transform.position);

        float x = Mathf.RoundToInt(transform.position.x - BlockCount.x % 2 * 0.5f) + BlockCount.x % 2 * 0.5f;
        float y = Mathf.RoundToInt(transform.position.y - BlockCount.y % 2 * 0.5f) + BlockCount.y % 2 * 0.5f;

        transform.position = new Vector3(x, y, 0);
        Debug.Log("OnMouseUp반올림이후(transform.position): " + transform.position);

        bool isSuccess = blockArrangeSystem.TryArrangementBlock(this);

        if (isSuccess == false)
        {
            StopCoroutine("OnScaleTo");
            RotateClockWise();
            StartCoroutine("OnScaleTo", Vector3.one * 0.5f);
            StartCoroutine(OnMoveTo(transform.parent.position, returnTime));
        }
    }

    public void RotateClockWise()
    {
        transform.Rotate(0f, 0f, -90f);
        //Debug.Log("RotateClockWise(transform.position): " + transform.position);
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i] = transform.GetChild(i).position - transform.position;   //자식 오브젝트의 전역좌표에서 부모 오브젝트의 전역좌표를 빼서 자식 오브젝트의 지역좌표 산출
            //Debug.Log("RotateClockWise(ChildBlocks["+i+"]): " + ChildBlocks[i]);
        }
    }

    private IEnumerator OnMoveTo(Vector3 end, float time)
    {
        Vector3 start = transform.position;
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.position = Vector3.Lerp(start, end, curveMovement.Evaluate(percent));

            yield return null;
        }
    }

    private IEnumerator OnScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / returnTime;

            transform.localScale = Vector3.Lerp(start, end, curveScale.Evaluate(percent));

            yield return null;
        }
    }
}
