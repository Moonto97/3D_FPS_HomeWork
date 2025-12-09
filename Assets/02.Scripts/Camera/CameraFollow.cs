using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    [Header("시점 설정")]
    public Vector3 firstPersonOffset = new Vector3(0f, 0.6f, 0f);
    public Vector3 thirdPersonOffset = new Vector3(0f, 2f, -4f);
    
    [Header("전환 설정")]
    public float transitionDuration = 0.5f;
    public Ease easeType = Ease.OutQuad;
    
    private Vector3 _currentOffset;
    private bool _isFirstPerson = true;
    private bool _isTransitioning = false;

    private void Start()
    {
        _currentOffset = firstPersonOffset;
    }

    private void LateUpdate()
    {
        // T키로 시점 전환
        if (Input.GetKeyDown(KeyCode.T) && !_isTransitioning)
        {
            ToggleView();
        }
        
        // 카메라 위치 업데이트 (로컬 오프셋 적용)
        transform.position = target.position + target.parent.rotation * _currentOffset;
    }

    private void ToggleView()
    {
        _isTransitioning = true;
        _isFirstPerson = !_isFirstPerson;
        
        Vector3 targetOffset = _isFirstPerson ? firstPersonOffset : thirdPersonOffset;
        
        DOTween.To(() => _currentOffset, x => _currentOffset = x, targetOffset, transitionDuration)
            .SetEase(easeType)
            .OnComplete(() => _isTransitioning = false);
        
        Debug.Log(_isFirstPerson ? "1인칭 모드" : "3인칭 모드");
    }
    
    public bool IsFirstPerson => _isFirstPerson;
}