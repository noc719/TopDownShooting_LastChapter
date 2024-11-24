using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound; //주웠을 때 소리

    private void OnTriggerEnter2D(Collider2D other) //충돌했을 때 효과
    {
        OnPickedUp(other.gameObject);

        if (pickupSound != null ) SoundManager.PlayClip(pickupSound);

        Destroy(gameObject);
    }

    protected abstract void OnPickedUp(GameObject gameObject); //주웠을 때 기능
}
