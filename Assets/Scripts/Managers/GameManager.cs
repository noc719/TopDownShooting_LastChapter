using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform Player { get; private set; }
    public ObjectPool ObjectPool { get; private set; }
    public ParticleSystem EffectParicle;

    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        Instance = this;
        Player = GameObject.FindGameObjectWithTag(playerTag).transform;

        ObjectPool = GetComponent<ObjectPool>();
        EffectParicle = GameObject.FindGameObjectWithTag("Particle").GetComponent<ParticleSystem>(); //�ϴ� �ѹ����� ���ϱ� Find �Լ��� ������ ��
    }
}