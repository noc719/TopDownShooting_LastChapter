using UnityEngine;

public class PickupHeal : PickupItem
{
    [SerializeField] int healValue = 10; //회복 밸류 일단 기본값 10

    protected override void OnPickedUp(GameObject gameObject)
    {
        HealthSystem healthSystem = gameObject.GetComponent<HealthSystem>();
        healthSystem.ChangeHealth(healValue); //회복에만 영향을 줄 예정
    }
}
