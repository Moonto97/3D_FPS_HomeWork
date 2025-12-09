using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 체력과 스테미나를 UI로 표시하는 스크립트
/// Canvas에 붙이세요!
/// </summary>
public class PlayerStatsUI : MonoBehaviour
{
    [Header("플레이어 컴포넌트 참조")]
    [Tooltip("PlayerHealth 컴포넌트가 있는 오브젝트")]
    public PlayerHealth PlayerHealth;
    
    [Tooltip("PlayerStamina 컴포넌트가 있는 오브젝트")]
    public PlayerStamina PlayerStamina;
    
    [Header("체력 UI")]
    public Slider HealthSlider;
    
    [Header("스테미나 UI")]
    public Slider StaminaSlider;
    
    private void Awake()
    {
        Debug.Log("[PlayerStatsUI] Awake 시작!");
        
        // Awake에서 자동으로 찾기 (Start보다 먼저 실행됨)
        if (PlayerHealth == null)
        {
            PlayerHealth = FindObjectOfType<PlayerHealth>();
            if (PlayerHealth != null)
                Debug.Log("[PlayerStatsUI] PlayerHealth 자동 검색 성공!");
            else
                Debug.LogError("[PlayerStatsUI] PlayerHealth를 찾을 수 없습니다!");
        }
        
        if (PlayerStamina == null)
        {
            PlayerStamina = FindObjectOfType<PlayerStamina>();
            if (PlayerStamina != null)
                Debug.Log("[PlayerStatsUI] PlayerStamina 자동 검색 성공!");
            else
                Debug.LogError("[PlayerStatsUI] PlayerStamina를 찾을 수 없습니다!");
        }
    }
    
    private void Start()
    {
        Debug.Log("[PlayerStatsUI] Start 시작!");
        
        // 슬라이더 초기화
        if (HealthSlider != null)
        {
            HealthSlider.minValue = 0f;
            HealthSlider.maxValue = 1f;
            Debug.Log("[PlayerStatsUI] HealthSlider 연결됨!");
        }
        else
        {
            Debug.LogError("[PlayerStatsUI] HealthSlider가 연결되지 않았습니다!");
        }
        
        if (StaminaSlider != null)
        {
            StaminaSlider.minValue = 0f;
            StaminaSlider.maxValue = 1f;
            Debug.Log("[PlayerStatsUI] StaminaSlider 연결됨!");
        }
        else
        {
            Debug.LogError("[PlayerStatsUI] StaminaSlider가 연결되지 않았습니다!");
        }
        
        // 첫 프레임에 즉시 UI 업데이트
        UpdateHealthUI();
        UpdateStaminaUI();
    }
    
    private void Update()
    {
        UpdateHealthUI();
        UpdateStaminaUI();
    }
    
    private void UpdateHealthUI()
    {
        if (HealthSlider == null || PlayerHealth == null) return;
        
        float ratio = PlayerHealth.GetRatio();
        HealthSlider.value = ratio;
        
        // 색상은 이미지 원본 그대로 유지
    }
    
    private void UpdateStaminaUI()
    {
        if (StaminaSlider == null || PlayerStamina == null) return;
        
        StaminaSlider.value = PlayerStamina.GetRatio();
        
        // 색상은 이미지 원본 그대로 유지
    }
}