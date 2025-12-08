using System;
using UnityEngine;

// 카메라 회전 기능
// 마우스를 조작하면 카메라를 그 방향으로 회전하고 싶다.

public class CameraRotate : MonoBehaviour
{
    public float RotationSpeed = 200f;
    
    // 유니티는 0~360 각도 체계이므로 우리가 다로 저장할 -360~360 체계로 누적할 변수
    private float _accumulationX = 0f;
    private float _accumulationY = 0f;
    private void Update()
    {
        // 게임 시작하면 y축이 0도에서 -1도ㄴ
        if (Input.GetMouseButton(1))
        {
            return;
        }
        
        // 1. 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        // 2. 마우스 입력을 누적한다. -> 누적된 회전 방향
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;
        _accumulationY += -mouseY * RotationSpeed * Time.deltaTime;
        // 3. 사람처럼 -90~90 도 사이로 제한한다.
        _accumulationY = Mathf.Clamp(_accumulationY, -90, 90);
        // 4. 누적한 회전 방향으로 카메라 회전하기
        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX, 0);
        
        
        // 쿼터니언 : 사원수
        // 쓰는 이유는 짐벌락 현상 방지
        // 공부 : 짐벌락, 쿼터니언 외에도 게임 수학 추천받아 공부하자 (ai한테)
        
        // 문제 : 잘 되긴 되는데 한번씩 세상이 뒤집어진다.
        
        
        
        

    }
}
