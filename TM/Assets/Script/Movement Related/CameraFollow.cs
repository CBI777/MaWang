using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /*22-1-19 ��������
    target�� ���̻� SerializeField�� �ƴ� - player�� prefab���� �����ʿ� ���� ��ȭ
    Edit-Project Settings-Script Execution Order���� default�� �ڿ� camerafollow�� �־������ν�
    �ǵ������� start�� �ð��� ����.
    �̷��� prefab���� ������ player�� �ݵ�� ��Ƴ� �� ����.
     */
    private Transform target;

    [SerializeField] //Range(0.01f, 1f)
    private float smoothSpeed = 0.125f;

    [SerializeField] //0.5, 1.14, -1.5�� ����
    private Vector3 offset = new Vector3(0.5f, 1.14f, -1.5f);

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.transform.position + offset;
        this.transform.position = Vector3.SmoothDamp(this.transform.position, desiredPosition, ref velocity, smoothSpeed);
    }
}
