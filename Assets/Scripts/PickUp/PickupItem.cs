using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound; //�ֿ��� �� �Ҹ�

    private void OnTriggerEnter2D(Collider2D other) //�浹���� �� ȿ��
    {
        OnPickedUp(other.gameObject);

        if (pickupSound != null ) SoundManager.PlayClip(pickupSound);

        Destroy(gameObject);
    }

    protected abstract void OnPickedUp(GameObject gameObject); //�ֿ��� �� ���
}
