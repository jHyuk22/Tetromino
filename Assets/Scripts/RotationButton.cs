using UnityEngine;
using UnityEngine.UI;

public class RotationButton : MonoBehaviour
{
    public Transform targetBlock; // ȸ�� ��� ��
    private Button button;

    private void Awake()
    {
        // ��ư ������Ʈ�� �����Ͽ� Ŭ�� �̺�Ʈ�� RotateBlock �Լ� ����
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
            targetBlock.Rotate(Vector3.forward, 90f); // Z���� �߽����� 90�� ȸ��
        }
        else
        {
            Debug.LogError("Target block is not assigned!");
        }
    }

    // �ܺο��� targetBlock�� ������ �� �ְԲ� �ϴ� �޼���
    public void SetTargetBlock(Transform newTarget)
    {
        targetBlock = newTarget;
    }
}
