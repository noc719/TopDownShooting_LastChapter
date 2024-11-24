using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform Player { get; private set; }
    public ObjectPool ObjectPool { get; private set; }
    public ParticleSystem EffectParicle;
    
    [SerializeField] private string playerTag = "Player";

    private HealthSystem playerHealthSystem;

    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private Slider hpGaugeSlider;
    [SerializeField] private GameObject gameoverUI;

    [SerializeField] private int currentWaveIndex = 0;
    private int currentSpawnCount = 0;
    private int waveSpawnCount = 0;
    private int waveSpawnPosCount = 0;

    public float spawnInterval = .5f;
    public List<GameObject> enemyPrefebs = new List<GameObject>();

    [SerializeField] private Transform spawnPositionsRoot; 
    private List<Transform> spawnPositions = new List<Transform>(); //�ڽĵ��� ������ġ�� ���� ����Ʈ

    [SerializeField] private List<GameObject> rewards = new List<GameObject>(); //�������� ���� ����Ʈ

    private void Awake()
    {
        Instance = this;
        Player = GameObject.FindGameObjectWithTag(playerTag).transform;

        ObjectPool = GetComponent<ObjectPool>();
        EffectParicle = GameObject.FindGameObjectWithTag("Particle").GetComponent<ParticleSystem>(); //�ϴ� �ѹ����� ���ϱ� Find �Լ��� ������ ��

        playerHealthSystem = Player.GetComponent<HealthSystem>();
        playerHealthSystem.OnDamage += UpdateHealthUI;
        playerHealthSystem.OnHeal += UpdateHealthUI;
        playerHealthSystem.OnDeath += GameOver;

        for (int i = 0; i < spawnPositionsRoot.childCount; i++)
        {
            spawnPositions.Add(spawnPositionsRoot.GetChild(i)); //GetChild �� Transform�� ��ȯ
        }
    }

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        while (true)
        {
            if(currentSpawnCount == 0) //������ ���� ���������
            {
                UpdateWaveUI();

                yield return new WaitForSeconds(2f);

                ProcessWaveConditions();

                yield return StartCoroutine(SpawnEnemiesWave()); //�̹� ���̺긦 ���� ������ ������ ����

                currentWaveIndex++;
            }
            yield return null;
        }
    }

    
    IEnumerator SpawnEnemiesWave()
    {
        for (int i = 0; i < waveSpawnPosCount; i++)
        {
            int posIdx =Random.Range(0,spawnPositions.Count);
            for (int j = 0; j< waveSpawnCount; j++)
            {
                SpawnEnemyAtPosition(posIdx);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private void SpawnEnemyAtPosition(int posIdx)
    {
        int prefabsIdx = Random.Range(0,enemyPrefebs.Count);
        GameObject enemy = Instantiate(enemyPrefebs[prefabsIdx], spawnPositions[posIdx].position, Quaternion.identity);
        //ȸ������ ������ ���ؼ� Quternion.Identity
        enemy.GetComponent<HealthSystem>().OnDeath += OnEnemyDeath; //Enemy�� HealthSystem�� �ο��ϰ� �̺�Ʈ ���
        currentSpawnCount++;
    }

    private void OnEnemyDeath()
    {
        currentSpawnCount--; //�׾��� �� �����ǵ��� OnDeath�̺�Ʈ�� ���� �༷ ���Ͽ� �ش���
    }

    void ProcessWaveConditions()
    {
        // % �� ������ ��������?
        // ������ ���� ���� ���ǹ��� �־, �ֱ⼺�� �ִ� ��� Ȱ���ϱ⵵ �ؿ�.

        // 20 ������������ �̺�Ʈ�� �߻��ؿ�.
        if (currentWaveIndex % 20 == 0)
        {
            RandomUpgrade();
        }

        if (currentWaveIndex % 10 == 0)
        {
            IncreaseSpawnPositions();
        }

        if (currentWaveIndex % 5 == 0)
        {
            CreateReward();
        }

        if (currentWaveIndex % 3 == 0)
        {
            IncreaseWaveSpawnCount();
        }
    }

    private void IncreaseWaveSpawnCount()
    {
        waveSpawnCount++;
    }

    private void CreateReward()
    {
        int selectedRewardIndex = Random.Range(0,rewards.Count);
        int randomPositionIndex = Random.Range(0,spawnPositions.Count);

        GameObject obj = rewards[selectedRewardIndex];
        Instantiate(obj,spawnPositions[randomPositionIndex].position, Quaternion.identity);
    }

    private void IncreaseSpawnPositions()
    {
        waveSpawnPosCount = waveSpawnCount + 1 > spawnPositions.Count ? waveSpawnCount : waveSpawnCount + 1;
        waveSpawnCount = 0;
    }

    private void RandomUpgrade()
    {
        Debug.Log("RandomUpgrade ȣ��");
    }

    private void GameOver()
    {
        //UI ����
        gameoverUI.SetActive(true);
    }

    private void UpdateHealthUI()
    {
        hpGaugeSlider.value = playerHealthSystem.CurrentHealth / playerHealthSystem.MaxHealth;
    }

    private void UpdateWaveUI()
    {
        waveTxt.text = (currentWaveIndex + 1).ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //���� ���� �ٽúҷ���
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}