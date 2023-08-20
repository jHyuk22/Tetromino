using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curveMovement;
    [SerializeField]
    private AnimationCurve curveScale;
    [SerializeField]
    private StageController stageController;

    private BlockArrangeSystem blockArrangeSystem;
    private int rotationState = 0;

    private float appealTIme = 0.5f;
    private float returnTime = 0.1f;

    [field: SerializeField]
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
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        StopCoroutine("OnScaleTo");
        StartCoroutine("OnScaleTo", Vector3.one);
    }

    private void OnMouseDrag()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        Vector3 gap = new Vector3(0, BlockCount.y * 0.5f, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }

    private void OnMouseUp()        //블럭개수말고 회전여부로 확인하는방법에 대해 생각해봅시다.......
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        //Debug.Log("OnMouseUp반올림이전(transform.position): " + transform.position);

        //float x = Mathf.RoundToInt(transform.position.x - BlockCount.x % 2 * 0.5f) + BlockCount.x % 2 * 0.5f;
        //float y = Mathf.RoundToInt(transform.position.y - BlockCount.y % 2 * 0.5f) + BlockCount.y % 2 * 0.5f;
        float offsetX = 0.5f, offsetY = 0.5f;

        switch (rotationState)
        {
            case 0:
            case 2:
                offsetX = BlockCount.x % 2 * 0.5f;
                offsetY = BlockCount.y % 2 * 0.5f;
                break;
            case 1:
            case 3:
                offsetX = BlockCount.y % 2 * 0.5f;
                offsetY = BlockCount.x % 2 * 0.5f;
                break;
        }

        float x = Mathf.RoundToInt(transform.position.x - offsetX) + offsetX;
        float y = Mathf.RoundToInt(transform.position.y - offsetY) + offsetY;

        transform.position = new Vector3(x, y, 0);
        //Debug.Log("OnMouseUp반올림이후(transform.position): " + transform.position);

        bool isSuccess = blockArrangeSystem.TryArrangementBlock(this);

        if (isSuccess == false)
        {
            StopCoroutine("OnScaleTo");
            RotateClockWise();
            StartCoroutine("OnScaleTo", Vector3.one * 0.5f);
            StartCoroutine(OnMoveTo(transform.parent.position, returnTime));
        }
    }

    private float findChildBlockPosition(float truePosition)
    {
        truePosition *= 2;
        truePosition = Mathf.Round(truePosition);
        truePosition /= 2;

        return truePosition;

    }

    public void RotateClockWise()
    {
        
        transform.Rotate(0f, 0f, 90f);
        rotationState = (rotationState + 1) % 4;
        //Debug.Log("RotateClockWise(transform.position): " + transform.position);
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i].x = findChildBlockPosition(Mathf.RoundToInt((transform.GetChild(i).position.x - transform.position.x) * 10.0f) / 10.0f);  //자식 오브젝트의 전역좌표에서 부모 오브젝트의 전역좌표를 빼서 자식 오브젝트의 지역좌표 산출
            ChildBlocks[i].y = findChildBlockPosition(Mathf.RoundToInt((transform.GetChild(i).position.y - transform.position.y) * 10.0f) / 10.0f);
            ChildBlocks[i].z = findChildBlockPosition(Mathf.RoundToInt((transform.GetChild(i).position.z - transform.position.z) * 10.0f) / 10.0f);
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
