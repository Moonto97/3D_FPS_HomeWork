using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 총기 이펙트 오브젝트 풀 - 총구 발사 이펙트와 적중 이펙트를 재사용!
///
/// ★ 오브젝트 풀링이란? ★
/// 파티클 이펙트를 계속 생성(Instantiate)하고
/// 삭제(Destroy)하면 성능이 떨어져요.
/// 그래서 미리 여러 개 만들어두고 껐다 켰다 하며 재사용하는 기법이에요!
///
/// ★ UnityEngine.Pool 사용 ★
/// Unity에서 공식 제공하는 ObjectPool을 사용합니다.
/// - 자동으로 풀 크기 관리
/// - Get/Release로 간편한 사용
/// - 최대 크기 제한 가능
///
/// 빈 GameObject에 붙이고 이름을 "GunEffectPool"로 지어주세요!
/// </summary>
public class GunEffectPool : MonoBehaviour
{
    [Header("파티클 프리팹")]
    [Tooltip("총구 발사 파티클 프리팹")]
    public GameObject MuzzleFlashPrefab;

    [Tooltip("적중 파티클 프리팹")]
    public GameObject HitEffectPrefab;

    [Header("풀 설정")]
    [Tooltip("기본 풀 크기 (미리 만들어둘 이펙트 개수)")]
    public int DefaultCapacity = 10;

    [Tooltip("최대 풀 크기 (이 크기를 초과하면 반환된 오브젝트 삭제)")]
    public int MaxPoolSize = 30;

    // UnityEngine.Pool의 ObjectPool 사용
    private ObjectPool<GameObject> _muzzleFlashPool;
    private ObjectPool<GameObject> _hitEffectPool;

    // 싱글톤 - 어디서든 GunEffectPool.Instance로 접근 가능!
    public static GunEffectPool Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ObjectPool 초기화
        InitializePools();
    }

    /// <summary>
    /// UnityEngine.Pool의 ObjectPool 초기화
    /// </summary>
    private void InitializePools()
    {
        // 총구 파티클 풀
        if (MuzzleFlashPrefab != null)
        {
            _muzzleFlashPool = new ObjectPool<GameObject>(
                createFunc: () => CreateEffect(MuzzleFlashPrefab),
                actionOnGet: OnGetEffect,
                actionOnRelease: OnReleaseEffect,
                actionOnDestroy: OnDestroyEffect,
                collectionCheck: true,
                defaultCapacity: DefaultCapacity,
                maxSize: MaxPoolSize
            );

            Debug.Log($"[GunEffectPool] 총구 파티클 풀 초기화 완료! (기본: {DefaultCapacity}, 최대: {MaxPoolSize})");
        }
        else
        {
            Debug.LogWarning("[GunEffectPool] MuzzleFlashPrefab이 설정되지 않았습니다!");
        }

        // 적중 파티클 풀
        if (HitEffectPrefab != null)
        {
            _hitEffectPool = new ObjectPool<GameObject>(
                createFunc: () => CreateEffect(HitEffectPrefab),
                actionOnGet: OnGetEffect,
                actionOnRelease: OnReleaseEffect,
                actionOnDestroy: OnDestroyEffect,
                collectionCheck: true,
                defaultCapacity: DefaultCapacity,
                maxSize: MaxPoolSize
            );

            Debug.Log($"[GunEffectPool] 적중 파티클 풀 초기화 완료! (기본: {DefaultCapacity}, 최대: {MaxPoolSize})");
        }
        else
        {
            Debug.LogWarning("[GunEffectPool] HitEffectPrefab이 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// 풀에서 새 이펙트를 생성할 때 호출됨
    /// </summary>
    private GameObject CreateEffect(GameObject prefab)
    {
        GameObject effect = Instantiate(prefab, transform);
        effect.SetActive(false);
        return effect;
    }

    /// <summary>
    /// 풀에서 이펙트를 꺼낼 때 호출됨
    /// </summary>
    private void OnGetEffect(GameObject effect)
    {
        effect.SetActive(true);
    }

    /// <summary>
    /// 풀에 이펙트를 반환할 때 호출됨
    /// </summary>
    private void OnReleaseEffect(GameObject effect)
    {
        effect.SetActive(false);
        effect.transform.SetParent(transform);
    }

    /// <summary>
    /// 풀 크기를 초과해서 이펙트가 삭제될 때 호출됨
    /// </summary>
    private void OnDestroyEffect(GameObject effect)
    {
        Destroy(effect);
    }

    /// <summary>
    /// 총구 발사 파티클 생성
    /// 사용법: GunEffectPool.Instance.GetMuzzleFlash(FirePoint.position, FirePoint.rotation);
    /// </summary>
    public void GetMuzzleFlash(Vector3 position, Quaternion rotation)
    {
        if (_muzzleFlashPool == null)
        {
            Debug.LogWarning("[GunEffectPool] 총구 파티클 풀이 초기화되지 않았습니다!");
            return;
        }

        GameObject effect = _muzzleFlashPool.Get();
        effect.transform.position = position;
        effect.transform.rotation = rotation;

        // 파티클 시스템 재생
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        // 파티클 재생 후 자동 반환
        StartCoroutine(ReturnAfterSeconds(effect, _muzzleFlashPool, 0.5f));
    }

    /// <summary>
    /// 적중 파티클 생성
    /// 사용법: GunEffectPool.Instance.GetHitEffect(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
    /// </summary>
    public void GetHitEffect(Vector3 position, Quaternion rotation)
    {
        if (_hitEffectPool == null)
        {
            Debug.LogWarning("[GunEffectPool] 적중 파티클 풀이 초기화되지 않았습니다!");
            return;
        }

        GameObject effect = _hitEffectPool.Get();
        effect.transform.position = position;
        effect.transform.rotation = rotation;

        // 파티클 시스템 재생
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        // 파티클 재생 후 자동 반환
        StartCoroutine(ReturnAfterSeconds(effect, _hitEffectPool, 1f));
    }

    /// <summary>
    /// 일정 시간 후 이펙트를 풀로 반환
    /// </summary>
    private System.Collections.IEnumerator ReturnAfterSeconds(
        GameObject effect,
        ObjectPool<GameObject> pool,
        float seconds)
    {
        yield return new WaitForSeconds(seconds);
        pool.Release(effect);
    }

    private void OnDestroy()
    {
        // 풀 정리
        _muzzleFlashPool?.Clear();
        _hitEffectPool?.Clear();
    }
}
