using UnityEngine;

/// <summary>
/// 스테미나만 관리하는 스크립트
/// 달리기, 구르기, 특수공격 등 어디서든 사용 가능!
/// Player 오브젝트에 붙이세요!
/// </summary>
public class PlayerStamina : MonoBehaviour
{
    [Header("스테미나 설정")]
    [Tooltip("최대 스테미나")]
    public float MaxStamina = 100f;
    
    [Tooltip("초당 회복량")]
    public float RegenRate = 15f;
    
    [Tooltip("스테미나 사용 후 회복 시작까지 대기 시간(초)")]
    public float RegenDelay = 1f;
    
    [Tooltip("스테미나 소진 후 다시 사용 가능해지는 비율 (0.3 = 30%)")]
    [Range(0f, 1f)]
    public float RecoveryThreshold = 0.3f;
    
    // [SerializeField] = private여도 Inspector에서 보임!
    [Header("디버그 (읽기 전용)")]
    [SerializeField] private float _currentStamina;
    [SerializeField] private float _regenTimer = 0f;
    [SerializeField] private bool _isExhausted = false;
    
    public float CurrentStamina => _currentStamina;
    public bool CanUse => !_isExhausted;
    public bool IsFull => _currentStamina >= MaxStamina;
    public bool IsExhausted => _isExhausted;
    
    private void Awake()
    {
        _currentStamina = MaxStamina;
        Debug.Log($"[PlayerStamina] Awake! 스테미나 초기화: {_currentStamina}");
    }
    
    private void Update()
    {
        HandleRegen();
    }
    
    /// <summary>
    /// 스테미나를 사용합니다. (달리기 등 지속 소모용)
    /// 사용법: playerStamina.Use(20f * Time.deltaTime);
    /// </summary>
    public bool Use(float amount)
    {
        if (_isExhausted)
            return false;
        
        if (_currentStamina <= 0)
        {
            _isExhausted = true;
            Debug.Log("스테미나 소진!");
            return false;
        }
        
        _currentStamina -= amount;
        _currentStamina = Mathf.Max(0f, _currentStamina);
        _regenTimer = RegenDelay;
        
        return true;
    }
    
    /// <summary>
    /// 스테미나를 즉시 사용합니다. (구르기 등 한 번에 소모용)
    /// 사용법: if(playerStamina.UseInstant(25f)) { 구르기 실행 }
    /// </summary>
    public bool UseInstant(float amount)
    {
        if (_isExhausted || _currentStamina < amount)
            return false;
        
        _currentStamina -= amount;
        _regenTimer = RegenDelay;
        
        if (_currentStamina <= 0)
        {
            _currentStamina = 0f;
            _isExhausted = true;
            Debug.Log("스테미나 소진!");
        }
        
        return true;
    }
    
    /// <summary>
    /// 스테미나가 충분한지 확인 (소모하지 않음)
    /// </summary>
    public bool HasEnough(float amount)
    {
        return !_isExhausted && _currentStamina >= amount;
    }
    
    /// <summary>
    /// 스테미나 즉시 회복 (아이템 등)
    /// </summary>
    public void Recover(float amount)
    {
        _currentStamina += amount;
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
        CheckExhaustedRecovery();
    }
    
    /// <summary>
    /// 스테미나 최대치로 회복
    /// </summary>
    public void RecoverFull()
    {
        _currentStamina = MaxStamina;
        _isExhausted = false;
    }
    
    /// <summary>
    /// 스테미나 비율 (0.0 ~ 1.0) - UI용
    /// </summary>
    public float GetRatio()
    {
        return _currentStamina / MaxStamina;
    }
    
    private void HandleRegen()
    {
        if (_currentStamina >= MaxStamina)
        {
            _currentStamina = MaxStamina;
            return;
        }
        
        if (_regenTimer > 0)
        {
            _regenTimer -= Time.deltaTime;
            return;
        }
        
        _currentStamina += RegenRate * Time.deltaTime;
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
        
        CheckExhaustedRecovery();
    }
    
    private void CheckExhaustedRecovery()
    {
        if (_isExhausted && _currentStamina >= MaxStamina * RecoveryThreshold)
        {
            _isExhausted = false;
            Debug.Log("스테미나 회복! 다시 사용 가능!");
        }
    }
}
