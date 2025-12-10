using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 폭탄 오브젝트 풀 - 폭탄을 미리 만들어두고 재사용!
///
/// ★ 오브젝트 풀링이란? ★
/// 게임에서 총알, 폭탄 같은 오브젝트를 계속 생성(Instantiate)하고
/// 삭제(Destroy)하면 성능이 떨어져요.
/// 그래서 미리 여러 개 만들어두고 껐다 켰다 하며 재사용하는 기법이에요!
///
/// ★ UnityEngine.Pool 사용 ★
/// Unity에서 공식 제공하는 ObjectPool을 사용합니다.
/// - 자동으로 풀 크기 관리
/// - Get/Release로 간편한 사용
/// - 최대 크기 제한 가능
///
/// 빈 GameObject에 붙이고 이름을 "BombPool"로 지어주세요!
/// </summary>
public class BombPool : MonoBehaviour
{
    [Header("풀 설정")]
    [Tooltip("폭탄 프리팹")]
    public GameObject BombPrefab;

    [Tooltip("기본 풀 크기 (미리 만들어둘 폭탄 개수)")]
    public int DefaultCapacity = 10;

    [Tooltip("최대 풀 크기 (이 크기를 초과하면 반환된 오브젝트 삭제)")]
    public int MaxPoolSize = 20;

    // UnityEngine.Pool의 ObjectPool 사용
    private ObjectPool<GameObject> _pool;

    // 싱글톤 - 어디서든 BombPool.Instance로 접근 가능!
    public static BombPool Instance { get; private set; }

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
        InitializePool();
    }

    /// <summary>
    /// UnityEngine.Pool의 ObjectPool 초기화
    /// </summary>
    private void InitializePool()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: OnCreateBomb,           // 새 폭탄 생성 방법
            actionOnGet: OnGetBomb,             // 풀에서 꺼낼 때 실행
            actionOnRelease: OnReleaseBomb,     // 풀에 반환할 때 실행
            actionOnDestroy: OnDestroyBomb,     // 풀 크기 초과로 삭제될 때 실행
            collectionCheck: true,              // 중복 반환 체크
            defaultCapacity: DefaultCapacity,   // 기본 풀 크기
            maxSize: MaxPoolSize                // 최대 풀 크기
        );

        // 미리 생성 (Warm up)
        GameObject[] prewarmBombs = new GameObject[DefaultCapacity];
        for (int i = 0; i < DefaultCapacity; i++)
        {
            prewarmBombs[i] = _pool.Get();
        }
        for (int i = 0; i < DefaultCapacity; i++)
        {
            _pool.Release(prewarmBombs[i]);
        }

        Debug.Log($"[BombPool] ObjectPool 초기화 완료! (기본: {DefaultCapacity}, 최대: {MaxPoolSize})");
    }

    /// <summary>
    /// 풀에서 새 폭탄을 생성할 때 호출됨
    /// </summary>
    private GameObject OnCreateBomb()
    {
        GameObject bomb = Instantiate(BombPrefab, transform);
        bomb.SetActive(false);
        Debug.Log("[BombPool] 새 폭탄 생성됨");
        return bomb;
    }

    /// <summary>
    /// 풀에서 폭탄을 꺼낼 때 호출됨
    /// </summary>
    private void OnGetBomb(GameObject bomb)
    {
        bomb.SetActive(true);
    }

    /// <summary>
    /// 풀에 폭탄을 반환할 때 호출됨
    /// </summary>
    private void OnReleaseBomb(GameObject bomb)
    {
        // 물리 초기화
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        bomb.SetActive(false);
        bomb.transform.SetParent(transform);
    }

    /// <summary>
    /// 풀 크기를 초과해서 폭탄이 삭제될 때 호출됨
    /// </summary>
    private void OnDestroyBomb(GameObject bomb)
    {
        Destroy(bomb);
        Debug.Log("[BombPool] 풀 크기 초과로 폭탄 삭제됨");
    }

    /// <summary>
    /// 풀에서 폭탄 하나 꺼내기
    /// 사용법: GameObject bomb = BombPool.Instance.GetBomb();
    /// </summary>
    public GameObject GetBomb()
    {
        return _pool.Get();
    }

    /// <summary>
    /// 폭탄을 풀에 반환 (다 쓴 폭탄 돌려주기)
    /// 사용법: BombPool.Instance.ReturnBomb(gameObject);
    /// </summary>
    public void ReturnBomb(GameObject bomb)
    {
        _pool.Release(bomb);
    }

    private void OnDestroy()
    {
        // 풀 정리
        _pool?.Clear();
    }
}
