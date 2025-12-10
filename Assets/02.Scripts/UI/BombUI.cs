using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 폭탄 탄창을 UI로 표시하는 스크립트
/// Canvas에 붙이세요!
/// </summary>
public class BombUI : MonoBehaviour
{
    [Header("BombThrower 참조")]
    [Tooltip("BombThrower 컴포넌트가 있는 오브젝트 (비워두면 자동 검색)")]
    public BombThrower BombThrower;
    
    [Header("탄창 표시 (텍스트)")]
    [Tooltip("폭탄 개수를 표시할 텍스트 (예: 3/5)")]
    public TextMeshProUGUI BombCountText;
    
    [Header("탄창 표시 (슬라이더) - 선택사항")]
    [Tooltip("탄창을 슬라이더로도 표시 (없어도 됨)")]
    public Slider BombSlider;
    
    [Header("재충전 표시 - 선택사항")]
    [Tooltip("재충전 진행률 슬라이더 (없어도 됨)")]
    public Slider RechargeSlider;
    
    [Tooltip("재충전 중일 때만 표시할 오브젝트 (없어도 됨)")]
    public GameObject RechargeIndicator;

    private void Awake()
    {
        // BombThrower 자동 검색
        if (BombThrower == null)
        {
            BombThrower = FindObjectOfType<BombThrower>();
            if (BombThrower != null)
                Debug.Log("[BombUI] BombThrower 자동 검색 성공!");
            else
                Debug.LogError("[BombUI] BombThrower를 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        // 슬라이더 초기화
        if (BombSlider != null)
        {
            BombSlider.minValue = 0f;
            BombSlider.maxValue = 1f;
        }
        
        if (RechargeSlider != null)
        {
            RechargeSlider.minValue = 0f;
            RechargeSlider.maxValue = 1f;
        }
        
        // 첫 프레임에 즉시 업데이트
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (BombThrower == null) return;
        
        // 1. 텍스트 업데이트 (3/5 형식)
        if (BombCountText != null)
        {
            BombCountText.text = $"{BombThrower.CurrentBombs}/{BombThrower.MaxBombs}";
        }
        
        // 2. 탄창 슬라이더 업데이트
        if (BombSlider != null)
        {
            BombSlider.value = BombThrower.GetRatio();
        }
        
        // 3. 재충전 슬라이더 업데이트
        if (RechargeSlider != null)
        {
            RechargeSlider.value = BombThrower.GetRechargeRatio();
        }
        
        // 4. 재충전 인디케이터 표시/숨김
        if (RechargeIndicator != null)
        {
            // 탄창이 가득 차지 않았을 때만 표시
            bool isRecharging = BombThrower.CurrentBombs < BombThrower.MaxBombs;
            RechargeIndicator.SetActive(isRecharging);
        }
    }
}
