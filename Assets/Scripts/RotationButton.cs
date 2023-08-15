using UnityEngine;
using UnityEngine.UI;

public class RotationButton : MonoBehaviour
{
    public Transform targetBlock; // 회전 대상 블럭
    private Button button;

    private void Awake()
    {
        // 버튼 컴포넌트에 접근하여 클릭 이벤트에 RotateBlock 함수 연결
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(RotateBlock);
        }
        else
        {
            Debug.LogError("Button component is missing!");
        }
    }

    public void OnMouseDown()
    {
        button.onClick.AddListener(RotateBlock);
    }

    public void RotateBlock()
    {
        if (targetBlock != null)
        {
            targetBlock.Rotate(Vector3.forward, 90f); // Z축을 중심으로 90도 회전
        }
        else
        {
            Debug.LogError("Target block is not assigned!");
        }
    }

    // 외부에서 targetBlock을 설정할 수 있게끔 하는 메서드
    public void SetTargetBlock(Transform newTarget)
    {
        targetBlock = newTarget;
    }
}
