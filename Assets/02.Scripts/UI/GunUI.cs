using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 총기 UI를 표시하는 스크립트
/// Canvas에 붙이세요!
/// </summary>
public class GunUI : MonoBehaviour
{
    [Header("PlayerFire 참조")]
    [Tooltip("PlayerFire 컴포넌트가 있는 오브젝트 (비워두면 자동 검색)")]
    public PlayerFire PlayerFire;

    [Header("탄약 표시")]
    [Tooltip("탄약을 표시할 텍스트 (예: 30/90)")]
    public TextMeshProUGUI AmmoText;

    [Header("크로스헤어")]
    [Tooltip("크로스헤어 이미지")]
    public Image Crosshair;

    [Tooltip("발사 시 크로스헤어 확대 크기")]
    public float CrosshairExpandSize = 1.2f;

    [Tooltip("크로스헤어 복구 속도")]
    public float CrosshairExpandSpeed = 10f;

    [Header("재장전 표시 - 선택사항")]
    [Tooltip("재장전 진행률 슬라이더 (없어도 됨)")]
    public Slider ReloadSlider;

    [Tooltip("재장전 중일 때만 표시할 오브젝트 (없어도 됨)")]
    public GameObject ReloadIndicator;

    [Tooltip("재장전 텍스트 (Reloading... 표시, 없어도 됨)")]
    public TextMeshProUGUI ReloadText;

    private Vector3 _crosshairOriginalScale;
    private float _crosshairCurrentScale = 1f;

    private void Awake()
    {
        // PlayerFire 자동 검색
        if (PlayerFire == null)
        {
            PlayerFire = FindObjectOfType<PlayerFire>();
            if (PlayerFire != null)
                Debug.Log("[GunUI] PlayerFire 자동 검색 성공!");
            else
                Debug.LogError("[GunUI] PlayerFire를 찾을 수 없습니다!");
        }

        // 크로스헤어 원본 크기 저장
        if (Crosshair != null)
        {
            _crosshairOriginalScale = Crosshair.transform.localScale;
        }
    }

    private void Start()
    {
        // 슬라이더 초기화
        if (ReloadSlider != null)
        {
            ReloadSlider.minValue = 0f;
            ReloadSlider.maxValue = 1f;
        }

        // 첫 프레임에 즉시 업데이트
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
        UpdateCrosshair();
    }

    private void UpdateUI()
    {
        if (PlayerFire == null) return;

        // 1. 탄약 텍스트 업데이트 (30/90 형식)
        if (AmmoText != null)
        {
            AmmoText.text = $"{PlayerFire.CurrentMagazine}/{PlayerFire.TotalAmmo}";
        }

        // 2. 재장전 UI 업데이트
        bool isReloading = PlayerFire.IsReloading;

        // 재장전 슬라이더
        if (ReloadSlider != null)
        {
            ReloadSlider.value = PlayerFire.GetReloadRatio();
        }

        // 재장전 인디케이터 (전체 UI 그룹)
        if (ReloadIndicator != null)
        {
            ReloadIndicator.SetActive(isReloading);
        }

        // 재장전 텍스트
        if (ReloadText != null)
        {
            ReloadText.gameObject.SetActive(isReloading);
        }
    }

    private void UpdateCrosshair()
    {
        if (Crosshair == null) return;
        if (PlayerFire == null) return;

        // 실제로 발사가 일어났을 때 크로스헤어 확대 효과
        if (PlayerFire.JustFired)
        {
            _crosshairCurrentScale = CrosshairExpandSize;
        }

        // 원래 크기로 복구
        _crosshairCurrentScale = Mathf.Lerp(
            _crosshairCurrentScale,
            1f,
            Time.deltaTime * CrosshairExpandSpeed
        );

        Crosshair.transform.localScale = _crosshairOriginalScale * _crosshairCurrentScale;
    }
}
