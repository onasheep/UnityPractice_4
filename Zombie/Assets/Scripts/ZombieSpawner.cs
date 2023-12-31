using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public Zombie ZombiePrefab;

    public ZombieData[] zombieDatas;
    public Transform[] spawnPoints;

    private List<Zombie> zombies = new List<Zombie>();
    private int wave;

    private void Awake()
    {
        zombieDatas[0] = ResourceManager.instance.zombieData_default;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        if(zombies.Count <= 0)
        {
            SpawnWave();
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
    }

    private void SpawnWave()
    {
        wave++;

        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie();
        }
    }

    private void CreateZombie()
    {
        ZombieData zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Zombie zombie = Instantiate(ZombiePrefab, spawnPoint.position, spawnPoint.rotation);
        zombie.SetUp(zombieData);
        zombies.Add(zombie);

        zombie.onDeath += () => zombies.Remove(zombie);
        zombie.onDeath += () => Destroy(zombie.gameObject, 10f);
        zombie.onDeath += () => GameManager.instance.AddScore(100);
    }

    private void OnDeathFunc()
    {

    }
}
