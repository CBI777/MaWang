using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /*22-1-19 변동사항
    target은 더이상 SerializeField가 아님 - player가 prefab으로 생성됨에 따른 변화
    Edit-Project Settings-Script Execution Order에서 default의 뒤에 camerafollow를 넣어줌으로써
    의도적으로 start의 시간을 늦춤.
    이러면 prefab으로 생성된 player를 반드시 잡아낼 수 있음.
     */
    private Transform target;

    [SerializeField] //Range(0.01f, 1f)
    private float smoothSpeed = 0.125f;

    [SerializeField] //0.5, 1.14, -1.5가 예쁨
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
