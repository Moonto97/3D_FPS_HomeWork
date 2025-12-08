using UnityEngine;


[RequireComponent(typeof(CharacterController))]
// 키보드를 누르면 캐릭터를 그 방향으로 이동시키고 싶다.
public class PlayerMove : MonoBehaviour
{
    // 필요 속성 
    // 이동 속도
    public float MoveSpeed = 7f;
    private CharacterController _controller;
    // 중력
    public float Gravity = 9.81f;
    private float _yVelocity = 0f; // 중력에 의해 누적될 y값
    // 점프력
    public float JumpForce = 15f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        // 0. 중력 누적
        _yVelocity -= Gravity * Time.deltaTime;
        
        
        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        // 2. 입력에 다른 방향 구하기
        // 현재는 글로벌 좌표계 기준으로 움직임
        // 카메라가 처다보는 방향을 기준으로 해야하므로
        // 카메라가 처다보는 방향 (로컬 좌표계) 를 기준으로 동작하도록 해야한다.
        Vector3 direction = new Vector3(x, 0, y).normalized;
        Debug.Log(_controller.collisionFlags);
        
        // 점프
        if (Input.GetButtonDown("Jump") &&  _controller.isGrounded)
        {
            _yVelocity = JumpForce;
        }
        
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _yVelocity;
        
        // 3. 방향으로 이동시키기
        _controller.Move(direction * MoveSpeed * Time.deltaTime);
    }
}
