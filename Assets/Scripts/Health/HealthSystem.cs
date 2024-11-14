using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float healthChangeDelay = .5f;
    [SerializeField] private AudioClip damageClip;

    private CharacterStatHandler _statsHandler;
    private float _timeSinceLastChange = float.MaxValue;

    // 체력이 변했을 때 할 행동들을 정의하고 적용 가능
    public event Action OnDamage;
    public event Action OnHeal;
    public event Action OnDeath;
    public event Action OnInvincibilityEnd;

    public float CurrentHealth { get; private set; }

    // get만 구현된 것처럼 프로퍼티를 사용하는 것
    // 이렇게 하면 데이터의 복제본이 여기저기 돌아다니다가 싱크가 깨지는 문제를 막을 수 있어요!
    public float MaxHealth => _statsHandler.CurrentStat.maxHealth;

    private void Awake()
    {
        _statsHandler = GetComponent<CharacterStatHandler>();
    }

    private void Start()
    {
        CurrentHealth = _statsHandler.CurrentStat.maxHealth;
    }

    private void Update()
    {
        if (_timeSinceLastChange < healthChangeDelay)
        {
            _timeSinceLastChange += Time.deltaTime;
            if (_timeSinceLastChange >= healthChangeDelay)
            {
                OnInvincibilityEnd?.Invoke();
            }
        }
    }

    public bool ChangeHealth(float change)
    {
        if (change == 0 || _timeSinceLastChange < healthChangeDelay)
        {
            return false;
        }

        _timeSinceLastChange = 0f;
        CurrentHealth += change;
        // 최솟값을 0, 최댓값을 MaxHealth로 하는 구문.
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        // CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        // CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth; 와 같아요!

        if (CurrentHealth <= 0f)
        {
            CallDeath();
            return true;
        }
        
        if (change > 0)
        {
            OnHeal?.Invoke();
        }
        else
        {
            OnDamage?.Invoke();

            if(damageClip) SoundManager.PlayClip(damageClip);
        }
        return true;
    }

    private void CallDeath()
    {
        OnDeath?.Invoke();
    }
}