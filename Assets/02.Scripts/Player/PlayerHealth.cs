using UnityEngine;

/// <summary>
/// 플레이어의 체력(HP)만 관리하는 스크립트
/// Player 오브젝트에 붙이세요!
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    [Tooltip("최대 체력")]
    public float MaxHealth = 100f;
    
    // [SerializeField] = private여도 Inspector에서 보임!
    [SerializeField] private float _currentHealth;
    public float CurrentHealth => _currentHealth;
    
    // 사망 여부
    public bool IsDead => _currentHealth <= 0;
    
    private void Awake()
    {
        _currentHealth = MaxHealth;
        Debug.Log($"[PlayerHealth] Awake! 체력 초기화: {_currentHealth}");
    }
    
    /// <summary>
    /// 데미지를 받을 때 호출
    /// </summary>
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
        
        Debug.Log($"데미지! 체력: {_currentHealth}/{MaxHealth}");
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 체력 회복
    /// </summary>
    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
        
        Debug.Log($"회복! 체력: {_currentHealth}/{MaxHealth}");
    }
    
    /// <summary>
    /// 체력 비율 (0.0 ~ 1.0) - UI용
    /// </summary>
    public float GetRatio()
    {
        return _currentHealth / MaxHealth;
    }
    
    private void Die()
    {
        Debug.Log("플레이어 사망!");
    }
}
