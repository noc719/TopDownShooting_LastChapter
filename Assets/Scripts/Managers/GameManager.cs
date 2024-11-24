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
    private List<Transform> spawnPositions = new List<Transform>(); //자식들의 스폰위치를 담은 리스트

    [SerializeField] private List<GameObject> rewards = new List<GameObject>(); //아이템을 담을 리스트

    private void Awake()
    {
        Instance = this;
        Player = GameObject.FindGameObjectWithTag(playerTag).transform;

        ObjectPool = GetComponent<ObjectPool>();
        EffectParicle = GameObject.FindGameObjectWithTag("Particle").GetComponent<ParticleSystem>(); //일단 한번쓰고 마니까 Find 함수로 가져온 것

        playerHealthSystem = Player.GetComponent<HealthSystem>();
        playerHealthSystem.OnDamage += UpdateHealthUI;
        playerHealthSystem.OnHeal += UpdateHealthUI;
        playerHealthSystem.OnDeath += GameOver;

        for (int i = 0; i < spawnPositionsRoot.childCount; i++)
        {
            spawnPositions.Add(spawnPositionsRoot.GetChild(i)); //GetChild 는 Transform을 반환
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
            if(currentSpawnCount == 0) //생성된 적이 모두죽으면
            {
                UpdateWaveUI();

                yield return new WaitForSeconds(2f);

                ProcessWaveConditions();

                yield return StartCoroutine(SpawnEnemiesWave()); //이번 웨이브를 전부 생성할 때까진 보류

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
        //회전없이 나오기 위해선 Quternion.Identity
        enemy.GetComponent<HealthSystem>().OnDeath += OnEnemyDeath; //Enemy에 HealthSystem을 부여하고 이벤트 등록
        currentSpawnCount++;
    }

    private void OnEnemyDeath()
    {
        currentSpawnCount--; //죽었을 때 차감되도록 OnDeath이벤트에 구독 펍섭 패턴에 해당함
    }

    void ProcessWaveConditions()
    {
        // % 는 나머지 연산자죠?
        // 나머지 값에 따라 조건문을 주어서, 주기성이 있는 대상에 활용하기도 해요.

        // 20 스테이지마다 이벤트가 발생해요.
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
        Debug.Log("RandomUpgrade 호출");
    }

    private void GameOver()
    {
        //UI 켜줌
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //현재 씬을 다시불러옴
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}