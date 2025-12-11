using UnityEngine;

/// <summary>
/// 총기 발사 시스템
/// Raycast 기반 히트스캔 방식
/// Player 오브젝트에 붙이세요!
/// </summary>
public class PlayerFire : MonoBehaviour
{
    // ============================================
    // 발사 설정
    // ============================================

    [Header("발사 설정")]
    [Tooltip("발사 쿨다운 (초)")]
    public float FireCooldown = 0.1f;

    [Tooltip("최대 사거리")]
    public float FireRange = 100f;

    // ============================================
    // 탄약 설정
    // ============================================

    [Header("탄약 설정")]
    [Tooltip("탄창당 최대 탄약")]
    public int MaxAmmoPerMagazine = 30;

    [Tooltip("최대 예비 탄약")]
    public int MaxReserveAmmo = 90;

    [Tooltip("재장전 시간 (초)")]
    public float ReloadTime = 2.0f;

    [SerializeField] private int _currentMagazine = 30;  // Inspector에서 확인용
    [SerializeField] private int _totalAmmo = 90;        // Inspector에서 확인용
    private bool _isReloading = false;
    private float _reloadTimer = 0f;

    // 외부에서 탄약 확인용
    public int CurrentMagazine => _currentMagazine;
    public int TotalAmmo => _totalAmmo;
    public bool IsReloading => _isReloading;

    // UI용: 이번 프레임에 발사했는지 확인
    public bool JustFired { get; private set; }

    // ============================================
    // 반동 설정
    // ============================================

    [Header("반동 설정")]
    [Tooltip("좌우 반동 (도)")]
    public float RecoilX = 2f;

    [Tooltip("위쪽 반동 (도)")]
    public float RecoilY = 3f;

    [Tooltip("반동 복구 시간 (초)")]
    public float RecoilRecoveryTime = 0.3f;

    private float _recoilRecoveryTimer = 0f;

    // ============================================
    // 참조
    // ============================================

    [Header("참조")]
    [Tooltip("총구 위치 (비워두면 카메라 앞에서 이펙트 생성)")]
    public Transform FirePoint;

    private float _lastFireTime = -999f;
    private Camera _mainCamera;
    private CameraRotate _cameraRotate;

    private void Start()
    {
        _mainCamera = Camera.main;
        _currentMagazine = MaxAmmoPerMagazine;
        _totalAmmo = MaxReserveAmmo;

        // CameraRotate 찾기 (반동 적용용)
        if (_mainCamera != null)
        {
            _cameraRotate = _mainCamera.GetComponent<CameraRotate>();
            if (_cameraRotate == null)
            {
                Debug.LogWarning("[PlayerFire] CameraRotate를 찾을 수 없습니다! 반동이 적용되지 않습니다.");
            }
        }

        Debug.Log($"[PlayerFire] 초기화 완료! 탄창: {_currentMagazine}, 예비: {_totalAmmo}");
    }

    private void Update()
    {
        // 0. 발사 플래그 초기화
        JustFired = false;

        // 1. 재장전 처리
        HandleReload();

        // 2. 반동 복구
        UpdateRecoil();

        // 3. 발사 입력 (연사 - 마우스를 누르고 있으면 계속 발사)
        if (Input.GetMouseButton(0))
        {
            TryFire();
        }

        // 4. 재장전 입력
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReload();
        }
    }

    /// <summary>
    /// 발사 시도
    /// </summary>
    private void TryFire()
    {
        // 쿨다운 체크
        if (Time.time - _lastFireTime < FireCooldown)
            return;

        // 재장전 중 체크
        if (_isReloading)
            return;

        // 탄약 체크
        if (_currentMagazine <= 0)
        {
            Debug.Log("[PlayerFire] 탄약 부족! 재장전하세요.");
            return;
        }

        Fire();
        _currentMagazine--;
        _lastFireTime = Time.time;
        JustFired = true;  // UI용 플래그 설정

        Debug.Log($"[PlayerFire] 발사! 남은 탄약: {_currentMagazine}/{MaxAmmoPerMagazine}");
    }

    /// <summary>
    /// 실제 발사 처리
    /// </summary>
    private void Fire()
    {
        // 1. Raycast 실행
        PerformRaycast();

        // 2. 총구 발사 이펙트
        CreateMuzzleFlash();

        // 3. 반동 적용
        ApplyRecoil();
    }

    /// <summary>
    /// Raycast 히트스캔 실행
    /// </summary>
    private void PerformRaycast()
    {
        // 카메라 중앙에서 Raycast (화면 중심 = 크로스헤어 위치)
        Ray ray = _mainCamera.ScreenPointToRay(new Vector3(
            Screen.width / 2f,
            Screen.height / 2f,
            0
        ));

        // 디버그: Ray 시각화
        Debug.DrawRay(ray.origin, ray.direction * FireRange, Color.red, 0.1f);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, FireRange))
        {
            // 적중 처리
            Debug.Log($"[PlayerFire] 적중! 오브젝트: {hitInfo.collider.gameObject.name}, " +
                      $"거리: {hitInfo.distance:F2}m, 위치: {hitInfo.point}");

            // 적중 파티클 생성
            CreateHitEffect(hitInfo.point, hitInfo.normal);

            // 향후 데미지 처리
            // PlayerHealth health = hitInfo.collider.GetComponent<PlayerHealth>();
            // if (health != null) health.TakeDamage(Damage);
        }
    }

    /// <summary>
    /// 총구 발사 파티클 생성
    /// </summary>
    private void CreateMuzzleFlash()
    {
        if (FirePoint != null && GunEffectPool.Instance != null)
        {
            GunEffectPool.Instance.GetMuzzleFlash(FirePoint.position, FirePoint.rotation);
        }
    }

    /// <summary>
    /// 적중 파티클 생성
    /// </summary>
    private void CreateHitEffect(Vector3 position, Vector3 normal)
    {
        if (GunEffectPool.Instance != null)
        {
            Quaternion rotation = Quaternion.LookRotation(normal);
            GunEffectPool.Instance.GetHitEffect(position, rotation);
        }
    }

    /// <summary>
    /// 재장전 시도
    /// </summary>
    private void TryReload()
    {
        // 이미 재장전 중이면 무시
        if (_isReloading)
            return;

        // 예비 탄약이 없으면 무시
        if (_totalAmmo <= 0)
        {
            Debug.Log("[PlayerFire] 예비 탄약이 없습니다!");
            return;
        }

        // 탄창이 이미 가득 차면 무시
        if (_currentMagazine >= MaxAmmoPerMagazine)
            return;

        // 재장전 시작
        _isReloading = true;
        _reloadTimer = ReloadTime;
        Debug.Log($"[PlayerFire] 재장전 시작! ({ReloadTime:F2}초)");
    }

    /// <summary>
    /// 재장전 처리
    /// </summary>
    private void HandleReload()
    {
        if (!_isReloading)
            return;

        _reloadTimer -= Time.deltaTime;

        if (_reloadTimer <= 0)
        {
            // 재장전 완료
            int ammoNeeded = MaxAmmoPerMagazine - _currentMagazine;
            int ammoToReload = Mathf.Min(ammoNeeded, _totalAmmo);

            _currentMagazine += ammoToReload;
            _totalAmmo -= ammoToReload;
            _isReloading = false;

            Debug.Log($"[PlayerFire] 재장전 완료! 탄창: {_currentMagazine}, 예비: {_totalAmmo}");
        }
    }

    /// <summary>
    /// 총기 반동 적용
    /// </summary>
    private void ApplyRecoil()
    {
        if (_cameraRotate == null)
            return;

        float recoilX = Random.Range(-RecoilX, RecoilX);
        float recoilY = Random.Range(0, RecoilY);  // 위쪽으로만 반동

        // CameraRotate를 통해 반동 적용
        _cameraRotate.AddRecoil(recoilX, recoilY);

        // 복구 타이머 설정 (현재는 사용하지 않지만 향후 확장 가능)
        _recoilRecoveryTimer = RecoilRecoveryTime;
    }

    /// <summary>
    /// 반동 복구
    /// </summary>
    private void UpdateRecoil()
    {
        if (_recoilRecoveryTimer > 0)
        {
            _recoilRecoveryTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 탄창 비율 (0.0 ~ 1.0) - UI용
    /// </summary>
    public float GetMagazineRatio()
    {
        return (float)_currentMagazine / MaxAmmoPerMagazine;
    }

    /// <summary>
    /// 재장전 진행률 (0.0 ~ 1.0) - UI용
    /// </summary>
    public float GetReloadRatio()
    {
        if (!_isReloading) return 1f;
        return 1f - (_reloadTimer / ReloadTime);
    }

    /// <summary>
    /// 전체 탄약 비율 (0.0 ~ 1.0) - UI용
    /// </summary>
    public float GetTotalAmmoRatio()
    {
        int totalPossibleAmmo = MaxAmmoPerMagazine + MaxReserveAmmo;
        int currentTotal = _currentMagazine + _totalAmmo;
        return (float)currentTotal / totalPossibleAmmo;
    }
}
