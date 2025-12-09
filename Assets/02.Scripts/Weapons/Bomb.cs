using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject BombPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        // 폭탄 생성
        GameObject Bomb = Instantiate(BombPrefab);
        Bomb.transform.position = transform.position;
        // 충돌하면 삭제
        Destroy(gameObject);
    }
}
