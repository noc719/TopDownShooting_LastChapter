using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterStatHandler : MonoBehaviour
{
    // �⺻ ���Ȱ� ���� ���ȵ��� �ɷ�ġ�� �����ؼ� ������ ����ϴ� ������Ʈ
    [SerializeField] private CharacterStat baseStats;
    [field:SerializeField]public CharacterStat CurrentStat { get; private set; } = new(); //baseStat���� �⺻���� �Ҵ����ֱ� ���� ������ �����ϱ� ���� new Ű����� �ʱ�ȭ�� �����ش�.
    public List<CharacterStat> statsModifiers = new List<CharacterStat>();

    //�ɷ�ġ �ּҰ�//

    //���ݺκ�
    private readonly float MinAttackDelay = 0.03f;
    private readonly float MinAttackPower = 0.5f;
    private readonly float MinAttackSize = 0.4f;
    private readonly float MinAttackSpeed = 0.1f;
    //�̵��ӵ�
    private readonly float MinSpeed = 0.8f;
    //ü��
    private readonly int MinMaxHealth = 5;

    private void Awake()
    {
        if(baseStats.attackSO != null)
        {
            Debug.Log("�����");
            baseStats.attackSO = Instantiate(baseStats.attackSO); //����� �� baseStats�� attackSO��  ���ٸ� SO�� �Էµ� ����� ���� ����
            CurrentStat.attackSO = Instantiate(baseStats.attackSO);//���� ���ȿ��� baseStats�� ��ϵ� attackSO�� �����ϰ� ����
        }

        UpdateCharacterStat();
    }

    private void UpdateCharacterStat()
    {
        ApplyStatModifier(baseStats); //statsChangeType�� ����Ʈ�� ���� ó�����ְ���

        foreach (CharacterStat stat in statsModifiers.OrderBy(o => o.statsChangeType))//orderby�� ������ �� �ڿ� ����
        {
            ApplyStatModifier(stat);
        } 
    }

    public void AddStatModifier(CharacterStat modifier)
    {
        statsModifiers.Add(modifier); //statModifier����Ʈ�� �߰� �ణ ���������� ���䰰��
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
            StatsChangeType.Add => (current, change) => current + change, //���ϴ� ������ ��ȭ
            StatsChangeType.Multiple => (current, change) => current * change, //���ϴ� ������ ��ȭ
            _ => (current, change) => current //�⺻ override�� ����Ʈ�� �����Ͽ� ó���� ������ ������ �� ���� ȣ��
        };

        //�޼��带 �з�����ó�� �Ѱ��ִ� �� 
        UpdateBasicStats(operation,modifier);
        UpdateAttackStats(operation,modifier);

        //CurrentStat�� attackSO�� RangedAttackSO���� Ȯ���ؼ� ���� ���
        //�̸� currentRanged�� �����ϴ� ����
        if(CurrentStat.attackSO is RangedAttackSO currentRanged && modifier.attackSO is RangedAttackSO newRanged) //is ������ ���ؼ� attackSO�� Ÿ���� RangedAttackSO���� ���� Ȯ���غ��� ����
        {
            UpdateRangedAttackStats(operation,currentRanged,newRanged);
        }
    }


    private void UpdateBasicStats(Func<float, float, float> operation, CharacterStat modifier) //���⼱ �⺻ ������ �Ǵ� ü�°� ���ǵ常 ó������
    {
        CurrentStat.maxHealth = Mathf.Max((int)operation(CurrentStat.maxHealth, modifier.maxHealth),MinMaxHealth); //���� ������ �ִ� ü���� Func �Լ����� ó���ؼ� ��ȯ�� maxHealth �� 5�� ���ѵ� �ּ�ü�� �� ���� ū ���� �ȴ�.
        CurrentStat.speed = Mathf.Max((int)operation(CurrentStat.speed,modifier.speed),MinSpeed); //���������� ���� ������ ���ǵ带 ���� ū ������ ó���Ѵ�.
    }

    private void UpdateAttackStats(Func<float, float, float> operation, CharacterStat modifier)
    {
        if (CurrentStat.attackSO == null || modifier.attackSO == null) return; //���� ���罺���� attackSO�� ����ų� modifier�� attackSO�� ��������� return�Ѵ�.

        var currentAttack = CurrentStat.attackSO; //������ ������⿡ ĳ�����ش�. �̰��� ���� ����ȭ ���ϰ�
        var newAttack = modifier.attackSO;

        currentAttack.delay = MathF.Max(operation(currentAttack.delay,newAttack.delay),MinAttackDelay); //���������� ���� ���ȿ� �ݿ�
        currentAttack.power = MathF.Max(operation(currentAttack.power, newAttack.power), MinAttackPower);
        currentAttack.size = MathF.Max(operation(currentAttack.size, newAttack.size), MinAttackSize);
        currentAttack.speed = MathF.Max(operation(currentAttack.speed, newAttack.speed), MinAttackSpeed);
    }

    private void UpdateRangedAttackStats(Func<float, float, float> operation, RangedAttackSO currentRanged, RangedAttackSO newRanged)
    {
        currentRanged.multipleProjectilesAngle = operation(currentRanged.multipleProjectilesAngle, newRanged.multipleProjectilesAngle); //attackSO�� Ÿ���� ���Ÿ��� ���� ����Ǵ� ���ȹݿ�
        currentRanged.spread = operation(currentRanged.spread, newRanged.spread);
        currentRanged.duration = operation(currentRanged.duration, newRanged.duration);
        currentRanged.numberofProjectilesPerShot = Mathf.CeilToInt(operation(currentRanged.numberofProjectilesPerShot, newRanged.numberofProjectilesPerShot));
        currentRanged.projectileColor = UpdateColor(operation, currentRanged.projectileColor, newRanged.projectileColor); //���� �Ȱ��� ���ش�.
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