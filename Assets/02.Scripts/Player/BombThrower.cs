using UnityEngine;

/// <summary>
/// 마우스 우클릭으로 폭탄을 던지는 스크립트
/// ★ 탄창 시스템 + 오브젝트 풀링 적용! ★
/// 
/// Player 오브젝트에 붙이세요!
/// </summary>
public class BombThrower : MonoBehaviour
{
    // ============================================
    // 폭탄 설정
    // ============================================
    
    [Header("폭탄 설정")]
    [Tooltip("던지는 힘")]
    public float ThrowForce = 20f;
    
    [Tooltip("위쪽으로 추가되는 힘 (포물선 궤적용)")]
    public float UpwardForce = 3f;
    
    // ============================================
    // 탄창 설정
    // ============================================
    
    [Header("탄창 설정")]
    [Tooltip("최대 폭탄 개수")]
    public int MaxBombs = 5;
    
    [Tooltip("폭탄 재충전 시간 (초)")]
    public float RechargeTime = 3f;
    
    [SerializeField] private int _currentBombs;  // Inspector에서 확인용
    private float _rechargeTimer = 0f;
    
    // 외부에서 현재 폭탄 개수 확인용
    public int CurrentBombs => _currentBombs;
    
    // ============================================
    // 발사 위치
    // ============================================
    
    [Header("발사 위치")]
    [Tooltip("폭탄이 생성될 위치 (비워두면 카메라 앞에서 생성)")]
    public Transform ThrowPoint;
    
    [Tooltip("카메라로부터 앞으로 떨어진 거리")]
    public float SpawnDistance = 1.5f;
    
    // ============================================
    // 쿨다운
    // ============================================
    
    [Header("쿨다운")]
    [Tooltip("폭탄 던지기 쿨다운 (초)")]
    public float Cooldown = 0.5f;
    
    private float _lastThrowTime = -999f;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        _currentBombs = MaxBombs;  // 시작할 때 탄창 가득!
        
        // BombPool 존재 확인
        if (BombPool.Instance == null)
        {
            Debug.LogError("[BombThrower] BombPool이 씬에 없습니다! BombPool 오브젝트를 만들어주세요!");
        }
    }

    private void Update()
    {
        // 탄창 재충전
        HandleRecharge();
        
        // 마우스 우클릭 감지
        if (Input.GetMouseButtonDown(1))
        {
            TryThrowBomb();
        }
    }

    /// <summary>
    /// 탄창 재충전 처리
    /// </summary>
    private void HandleRecharge()
    {
        // 탄창이 가득 차면 재충전 안 함
        if (_currentBombs >= MaxBombs)
        {
            _rechargeTimer = 0f;
            return;
        }
        
        // 타이머 증가
        _rechargeTimer += Time.deltaTime;
        
        // 재충전 시간이 되면 폭탄 1개 추가
        if (_rechargeTimer >= RechargeTime)
        {
            _currentBombs++;
            _rechargeTimer = 0f;
            Debug.Log($"[BombThrower] 폭탄 재충전! ({_currentBombs}/{MaxBombs})");
        }
    }

    private void TryThrowBomb()
    {
        // 쿨다운 체크
        if (Time.time - _lastThrowTime < Cooldown)
        {
            return;
        }
        
        // 탄창 체크
        if (_currentBombs <= 0)
        {
            Debug.Log("[BombThrower] 폭탄이 없습니다! 재충전을 기다리세요.");
            return;
        }
        
        // BombPool 체크
        if (BombPool.Instance == null)
        {
            Debug.LogError("[BombThrower] BombPool이 없습니다!");
            return;
        }
        
        ThrowBomb();
        _currentBombs--;  // 탄창 감소
        _lastThrowTime = Time.time;
        
        Debug.Log($"[BombThrower] 폭탄 투척! 남은 폭탄: {_currentBombs}/{MaxBombs}");
    }

    private void ThrowBomb()
    {
        // 1. 발사 위치 계산
        Vector3 spawnPosition;
        if (ThrowPoint != null)
        {
            spawnPosition = ThrowPoint.position;
        }
        else
        {
            spawnPosition = _mainCamera.transform.position 
                          + _mainCamera.transform.forward * SpawnDistance;
        }
        
        // 2. 풀에서 폭탄 가져오기 (Instantiate 대신!)
        GameObject bomb = BombPool.Instance.GetBomb();
        bomb.transform.position = spawnPosition;
        bomb.transform.rotation = Quaternion.identity;
        
        // 3. Rigidbody 가져오기
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[BombThrower] 폭탄에 Rigidbody가 없습니다!");
            return;
        }
        
        // 4. 속도 초기화 (풀에서 가져온 거라 이전 속도가 남아있을 수 있음)
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // 5. 던지는 방향 계산
        Vector3 throwDirection = _mainCamera.transform.forward + Vector3.up * UpwardForce * 0.1f;
        throwDirection.Normalize();
        
        // 6. 힘 적용
        rb.AddForce(throwDirection * ThrowForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
    }
    
    /// <summary>
    /// 탄창 비율 (0.0 ~ 1.0) - UI용
    /// </summary>
    public float GetRatio()
    {
        return (float)_currentBombs / MaxBombs;
    }
    
    /// <summary>
    /// 재충전 진행률 (0.0 ~ 1.0) - UI용
    /// </summary>
    public float GetRechargeRatio()
    {
        if (_currentBombs >= MaxBombs) return 1f;
        return _rechargeTimer / RechargeTime;
    }
}
