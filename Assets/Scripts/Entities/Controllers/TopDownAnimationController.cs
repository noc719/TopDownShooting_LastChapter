using UnityEngine;

public class TopDownAnimationController : AnimationController
{
    // Animator.StringToHash를 통해 Animator 변수 전환에 활용되는 부분에 대한 최적화를 진행할 수 있습니다.
    // StringToHash는 IsWalking이라는 문자열을 일방향 함수인 해쉬함수를 통해 특정한 값으로 변환합니다.
    // 더 궁금하시다면 C# GetHashCode를 검색해보세요!
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsHit = Animator.StringToHash("IsHit");

    private static readonly int Attack = Animator.StringToHash("Attack");

    private readonly float magnituteThreshold = 0.5f;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        // 공격하거나 움직일 때 애니메이션이 같이 반응하도록 구독
        controller.OnAttackEvent += Attacking;
        controller.OnMoveEvent += Move;
    }

    private void Move(Vector2 obj)
    {
        animator.SetBool(IsWalking, obj.magnitude > magnituteThreshold);
    }

    // OnAttackEvent가 Action<AttackSO>이기 때문에 Attacking이 AttackSO를 사용하지 않아도 매개변수로 가지고 있어야 합니다.
    // 이런 걸 함수(메소드) 시그니처를 맞춘다라고 합니다.
    private void Attacking(AttackSO obj)
    {
        animator.SetTrigger(Attack);
    }

    // 아직 피격부분은 없지만 곧 할 것이기 때문에 일단 둡니다.
    private void Hit()
    {
        animator.SetBool(IsHit, true);
    }

    // 아직 피격부분은 없지만 곧 할 것이기 때문에 일단 둡니다.
    private void InvincibilityEnd()
    {
        animator.SetBool(IsHit, false);
    }
}