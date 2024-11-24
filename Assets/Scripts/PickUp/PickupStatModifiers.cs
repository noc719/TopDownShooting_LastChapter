using System.Collections.Generic;
using UnityEngine;

public class PickupStatModifiers : PickupItem
{
    [SerializeField] List<CharacterStat> statsModifier = new List<CharacterStat>(); //아이템에 추가할 강화 로직

    protected override void OnPickedUp(GameObject gameObject)
    {
        CharacterStatHandler statHandler = gameObject.GetComponent<CharacterStatHandler>(); //충돌한 오브젝트의 캐릭터 스탯핸들러를 가져옴

        foreach(CharacterStat modifier in statsModifier)
        {
            statHandler.AddStatModifier(modifier); //가지고 있는 강화로직을 추가시켜줌
        }

        //최대 체력을 올리는 경우
        HealthSystem healthSystem = gameObject.GetComponent<HealthSystem>();
        healthSystem.ChangeHealth(0); //능력치만 바꾸기 때문에 0으로 해준다.
    }
}