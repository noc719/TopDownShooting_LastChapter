using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    private HealthSystem healthSystem;
    private Rigidbody2D rigidbody;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        rigidbody = GetComponent<Rigidbody2D>();
        // 실제 실행 주체는 healthSystem임
        healthSystem.OnDeath += OnDeath;
    }

    void OnDeath()
    {
        // 멈추도록 수정
        rigidbody.velocity = Vector3.zero;

        // 약간 반투명한 느낌으로 변경
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        // 스크립트 더이상 작동 안하도록 함
        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }

        // 2초뒤에 파괴
        Destroy(gameObject, 2f);
    }
}