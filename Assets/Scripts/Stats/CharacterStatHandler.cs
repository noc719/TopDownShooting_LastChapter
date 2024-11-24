using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterStatHandler : MonoBehaviour
{
    // 기본 스탯과 버프 스탯들의 능력치를 종합해서 스탯을 계산하는 컴포넌트
    [SerializeField] private CharacterStat baseStats;
    [field:SerializeField]public CharacterStat CurrentStat { get; private set; } = new(); //baseStat으로 기본값을 할당해주기 전에 오류를 방지하기 위해 new 키워드로 초기화를 시켜준다.
    public List<CharacterStat> statsModifiers = new List<CharacterStat>();

    //능력치 최소값//

    //공격부분
    private readonly float MinAttackDelay = 0.03f;
    private readonly float MinAttackPower = 0.5f;
    private readonly float MinAttackSize = 0.4f;
    private readonly float MinAttackSpeed = 0.1f;
    //이동속도
    private readonly float MinSpeed = 0.8f;
    //체력
    private readonly int MinMaxHealth = 5;

    private void Awake()
    {
        if(baseStats.attackSO != null)
        {
            Debug.Log("비었다");
            baseStats.attackSO = Instantiate(baseStats.attackSO); //실행될 때 baseStats에 attackSO가  없다면 SO에 입력된 값대로 새로 생성
            CurrentStat.attackSO = Instantiate(baseStats.attackSO);//현재 스탯에도 baseStats에 등록된 attackSO와 동일하게 생성
        }

        UpdateCharacterStat();
    }

    private void UpdateCharacterStat()
    {
        ApplyStatModifier(baseStats); //statsChangeType의 디폴트에 먼저 처리해주고나서

        foreach (CharacterStat stat in statsModifiers.OrderBy(o => o.statsChangeType))//orderby로 정렬을 그 뒤에 해줌
        {
            ApplyStatModifier(stat);
        } 
    }

    public void AddStatModifier(CharacterStat modifier)
    {
        statsModifiers.Add(modifier); //statModifier리스트에 추가 약간 상태패턴의 개념같음
        UpdateCharacterStat();
    }

    public void RemoveStatModifier(CharacterStat modifier)
    {
        statsModifiers.Remove(modifier);
        UpdateCharacterStat();
    }

    private void ApplyStatModifier(CharacterStat modifier)
    {
        Func<float, float, float> operation = modifier.statsChangeType switch
        {
            StatsChangeType.Add => (current, change) => current + change, //더하는 개념의 강화
            StatsChangeType.Multiple => (current, change) => current * change, //곱하는 개념의 강화
            _ => (current, change) => current //기본 override를 디폴트로 지정하여 처음에 스탯을 적용할 때 먼저 호출
        };

        //메서드를 패러미터처럼 넘겨주는 것 
        UpdateBasicStats(operation,modifier);
        UpdateAttackStats(operation,modifier);

        //CurrentStat의 attackSO가 RangedAttackSO인지 확인해서 맞을 경우
        //이를 currentRanged로 저장하는 문법
        if(CurrentStat.attackSO is RangedAttackSO currentRanged && modifier.attackSO is RangedAttackSO newRanged) //is 구문을 통해서 attackSO의 타입이 RangedAttackSO인지 각각 확인해본후 실행
        {
            UpdateRangedAttackStats(operation,currentRanged,newRanged);
        }
    }


    private void UpdateBasicStats(Func<float, float, float> operation, CharacterStat modifier) //여기선 기본 스탯이 되는 체력과 스피드만 처리해줌
    {
        CurrentStat.maxHealth = Mathf.Max((int)operation(CurrentStat.maxHealth, modifier.maxHealth),MinMaxHealth); //현재 스탯의 최대 체력은 Func 함수에서 처리해서 반환된 maxHealth 와 5로 제한된 최소체력 중 값이 큰 것이 된다.
        CurrentStat.speed = Mathf.Max((int)operation(CurrentStat.speed,modifier.speed),MinSpeed); //마찬가지로 현재 스탯의 스피드를 둘중 큰 값으로 처리한다.
    }

    private void UpdateAttackStats(Func<float, float, float> operation, CharacterStat modifier)
    {
        if (CurrentStat.attackSO == null || modifier.attackSO == null) return; //만약 현재스탯의 attackSO가 비었거나 modifier의 attackSO가 비어있으면 return한다.

        var currentAttack = CurrentStat.attackSO; //문장이 길어지기에 캐싱해준다. 이것을 나도 습관화 들일것
        var newAttack = modifier.attackSO;

        currentAttack.delay = MathF.Max(operation(currentAttack.delay,newAttack.delay),MinAttackDelay); //마찬가지로 현재 스탯에 반영
        currentAttack.power = MathF.Max(operation(currentAttack.power, newAttack.power), MinAttackPower);
        currentAttack.size = MathF.Max(operation(currentAttack.size, newAttack.size), MinAttackSize);
        currentAttack.speed = MathF.Max(operation(currentAttack.speed, newAttack.speed), MinAttackSpeed);
    }

    private void UpdateRangedAttackStats(Func<float, float, float> operation, RangedAttackSO currentRanged, RangedAttackSO newRanged)
    {
        currentRanged.multipleProjectilesAngle = operation(currentRanged.multipleProjectilesAngle, newRanged.multipleProjectilesAngle); //attackSO의 타입이 원거리일 때만 수행되는 스탯반영
        currentRanged.spread = operation(currentRanged.spread, newRanged.spread);
        currentRanged.duration = operation(currentRanged.duration, newRanged.duration);
        currentRanged.numberofProjectilesPerShot = Mathf.CeilToInt(operation(currentRanged.numberofProjectilesPerShot, newRanged.numberofProjectilesPerShot));
        currentRanged.projectileColor = UpdateColor(operation, currentRanged.projectileColor, newRanged.projectileColor); //색도 똑같이 해준다.
    }

    private Color UpdateColor(Func<float, float, float> operation, Color curStat, Color modifierStat)
    {
        return new Color 
        (
            operation(curStat.r,modifierStat.r),
            operation(curStat.g,modifierStat.g),
            operation(curStat.b,modifierStat.b),
            operation(curStat.a,modifierStat.a)
        )
        ;
    }
}