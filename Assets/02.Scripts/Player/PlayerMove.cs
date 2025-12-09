using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStamina))] // 스테미나 필요!
/// <summary>
/// 키보드를 누르면 캐릭터를 그 방향으로 이동시키는 스크립트
/// Shift 키로 달리기 (스테미나 소모)
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // ============================================
    // 이동 설정
    // ============================================
    
    [Header("이동 설정")]
    public float MoveSpeed = 7f;
    
    [Tooltip("달리기 속도 배율 (1.3 = 30% 빠름)")]
    public float SprintMultiplier = 1.3f;
    
    [Tooltip("달리기 시 초당 스테미나 소모량")]
    public float SprintStaminaCost = 20f;
    
    private CharacterController _controller;
    
    // ============================================
    // 중력 & 점프
    // ============================================
    
    [Header("중력 & 점프")]
    public float Gravity = 9.81f;
    public float JumpForce = 15f;
    public int JumpCount = 0;
    
    private float _yVelocity = 0f;
    
    // ============================================
    // 스테미나 참조
    // ============================================
    
    private PlayerStamina _stamina;
    
    // 현재 달리는 중인지 (외부에서 확인용)
    public bool IsSprinting { get; private set; }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stamina = GetComponent<PlayerStamina>();
    }
    
    private void Update()
    {
        // 0. 중력 누적
        _yVelocity -= Gravity * Time.deltaTime;
        
        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        
        // 2. 입력에 따른 방향 구하기
        Vector3 direction = new Vector3(x, 0, y).normalized;
        
        // 3. 점프
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            _yVelocity = JumpForce;
            JumpCount = 1;
        }
        // 3-1. 2단 점프
        else if (Input.GetButtonDown("Jump") && JumpCount == 1 && !_controller.isGrounded && _stamina.UseInstant(20f))
        {
            _yVelocity = JumpForce;
            JumpCount = 0;
        }
        
        // 4. 카메라 기준 방향 변환
        direction = Camera.main.transform.TransformDirection(direction);
        
        // 5. 달리기 처리 & 속도 계산
        float speed = CalculateSpeed(direction);
        
        // 6. y 속도 적용
        direction.y = _yVelocity;
        
        // 7. 이동
        _controller.Move(direction * speed * Time.deltaTime);
    }
    
    /// <summary>
    /// 달리기 여부에 따른 속도 계산
    /// </summary>
    private float CalculateSpeed(Vector3 moveDirection)
    {
        bool isMoving = moveDirection.magnitude > 0.1f;
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        // 달리기 조건: Shift 누름 + 이동 중 + 스테미나 사용 가능
        if (shiftHeld && isMoving && _stamina.CanUse)
        {
            // 스테미나 소모 시도
            if (_stamina.Use(SprintStaminaCost * Time.deltaTime))
            {
                IsSprinting = true;
                return MoveSpeed * SprintMultiplier;
            }
        }
        
        IsSprinting = false;
        return MoveSpeed;
    }
}
