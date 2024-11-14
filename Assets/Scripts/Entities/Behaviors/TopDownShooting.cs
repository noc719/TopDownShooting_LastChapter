using UnityEngine;

public class TopDownShooting : MonoBehaviour
{
    private TopDownController contoller;
    private Vector2 aimDirection = Vector2.right;

    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private AudioClip shootingClip;

    private void Awake()
    {
        contoller = GetComponent<TopDownController>();
    }

    void Start()
    {
        contoller.OnAttackEvent += OnShoot;
        // OnLookEvent에 이제 두개가 등록되는 것(하나는 지난 시간에 등록했었죠? TopDownAimRotation.OnAim(Vec2)
        // 한 개의 델리게이트에 여러 개의 함수가 등록되어있는 것을 multicast delegate라고 함.
        // Action이나 Func도 델리게이트의 일종인 것 기억하시죠..?
        contoller.OnLookEvent += OnAim;
    }

    private void OnAim(Vector2 newAimDirection)
    {
        aimDirection = newAimDirection;
    }

    private void OnShoot(AttackSO attackSO)
    {
        RangedAttackSO RangedAttackSO = attackSO as RangedAttackSO;
        float projectilesAngleSpace = RangedAttackSO.multipleProjectilesAngel;
        int numberOfProjectilesPerShot = RangedAttackSO.numberofProjectilesPerShot;

        float minAngle = -(numberOfProjectilesPerShot / 2f) * projectilesAngleSpace + 0.5f * RangedAttackSO.multipleProjectilesAngel;


        for (int i = 0; i < numberOfProjectilesPerShot; i++)
        {
            float angle = minAngle + projectilesAngleSpace * i;
            float randomSpread = Random.Range(-RangedAttackSO.spread, RangedAttackSO.spread);
            angle += randomSpread;
            CreateProjectile(RangedAttackSO, angle);
        }
    }

    private void CreateProjectile(RangedAttackSO RangedAttackSO, float angle)
    {
        // 오브젝트 풀을 활용한 생성으로 변경
        GameObject obj = GameManager.Instance.ObjectPool.SpawnFromPool(RangedAttackSO.bulletNameTag);
        
        // 발사체 기본 세팅
        obj.transform.position = projectileSpawnPosition.position;
        ProjectileController attackController = obj.GetComponent<ProjectileController>();
        attackController.InitializeAttack(RotateVector2(aimDirection, angle), RangedAttackSO);

        obj.SetActive(true);

        if (shootingClip) SoundManager.PlayClip(shootingClip);
    }

    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;
    }
}