using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // 원거리 공격 발사체
    [SerializeField] private LayerMask levelCollisionLayer;

    private RangedAttackSO _attackData;
    private float _currentDuration;
    private Vector2 _direction;
    private bool _isReady;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;

    public bool fxOnDestory = true;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        if (!_isReady)
        {
            return;
        }

        _currentDuration += Time.deltaTime;

        if (_currentDuration > _attackData.duration)
        {
            DestroyProjectile(transform.position, false);
        }

        _rigidbody.velocity = _direction * _attackData.speed;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // levelCollisionLayer에 포함되는 레이어인지 확인합니다.
        if (IsLayerMatched(levelCollisionLayer.value, collision.gameObject.layer))
        {
            // 벽에서는 충돌한 지점으로부터 약간 앞 쪽에서 발사체를 파괴합니다.
            Vector2 destroyPosition = collision.ClosestPoint(transform.position) - _direction * .2f;
            DestroyProjectile(destroyPosition, fxOnDestory);
        }
        // _attackData.target에 포함되는 레이어인지 확인합니다.
        else if (IsLayerMatched(_attackData.target.value, collision.gameObject.layer))
        {
            // 충돌한 오브젝트에서 HealthSystem 컴포넌트를 가져옵니다.
            HealthSystem healthSystem = collision.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // 충돌한 오브젝트의 체력을 감소시킵니다.
                healthSystem.ChangeHealth(-_attackData.power);

                // 넉백이 활성화된 경우, ★드★디★어★ 넉백을 적용합니다.
                if (_attackData.isOnKnockback)
                {
                    ApplyKnockback(collision);
                }
            }
            // 충돌한 지점에서 발사체를 파괴합니다.
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestory);
        }
    }

    // 레이어가 일치하는지 확인하는 메소드입니다.
    private bool IsLayerMatched(int layerMask, int objectLayer)
    {
        return layerMask == (layerMask | (1 << objectLayer));
    }

    // 넉백을 적용하는 메소드입니다.
    private void ApplyKnockback(Collider2D collision)
    {
        TopDownMovement movement = collision.GetComponent<TopDownMovement>();
        if (movement != null)
        {
            movement.ApplyKnockback(transform, _attackData.knockbackPower, _attackData.knockbackTime);
        }
    }

    public void InitializeAttack(Vector2 direction, RangedAttackSO attackData)
    {
        _attackData = attackData;
        _direction = direction;

        UpdateProjectilSprite();
        _trailRenderer.Clear();
        _currentDuration = 0;
        _spriteRenderer.color = attackData.projectileColor;

        transform.right = _direction;

        _isReady = true;
    }

    private void UpdateProjectilSprite()
    {
        transform.localScale = Vector3.one * _attackData.size;
    }

    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx)
        {
            // TODO : ParticleSystem에 대해서 배우고, 무기 NameTag로 해당하는 FX가져오기
        }
        gameObject.SetActive(false);
    }
}