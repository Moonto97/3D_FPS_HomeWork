using System;
using UnityEngine;

/// <summary>
/// 폭탄 스크립트 - 충돌하면 폭발!
/// ★ 오브젝트 풀링 적용: Destroy 대신 풀로 반환! ★
/// </summary>
public class Bomb : MonoBehaviour
{
    [Header("폭발 설정")]
    public GameObject ExplosionEffectPrefab;
    
    [Tooltip("폭발 후 풀로 반환되기까지 대기 시간")]
    public float ReturnDelay = 0.1f;
    
    private bool _hasExploded = false;  // 중복 폭발 방지

    /// <summary>
    /// 풀에서 꺼내질 때마다 호출됨 (활성화될 때)
    /// </summary>
    private void OnEnable()
    {
        _hasExploded = false;  // 상태 초기화!
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 이미 폭발했으면 무시 (중복 방지)
        if (_hasExploded) return;
        
        _hasExploded = true;
        Explode();
    }

    private void Explode()
    {
        // 1. 폭발 이펙트 생성
        if (ExplosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(ExplosionEffectPrefab, transform.position, Quaternion.identity);
            // 이펙트는 일정 시간 후 삭제 (이펙트는 풀링 안 해도 됨)
            Destroy(effect, 2f);
        }
        
        // 2. 풀로 반환 (Destroy 대신!)
        if (BombPool.Instance != null)
        {
            // 약간의 딜레이 후 반환 (이펙트가 보이도록)
            Invoke(nameof(ReturnToPool), ReturnDelay);
        }
        else
        {
            // BombPool이 없으면 그냥 삭제 (비상용)
            Destroy(gameObject);
        }
    }

    private void ReturnToPool()
    {
        BombPool.Instance.ReturnBomb(gameObject);
    }
}
