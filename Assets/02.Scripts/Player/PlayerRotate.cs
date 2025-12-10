using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 200f;
    private float _accumulationX = 0f;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;
        
        transform.eulerAngles = new Vector3(0, _accumulationX, 0);
    }
}