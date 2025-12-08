using UnityEngine;

// 목표를 따라다니는 카메라
public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        transform.position = target.position;
    }
}
