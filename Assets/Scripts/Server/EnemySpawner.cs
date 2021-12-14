using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Time between spawns, in seconds")]
    private float spawnFrequency = 3.0f;

    [SerializeField]
    private GameObject enemy;

    [SerializeField] private int maxSpawnedEnemies;
    private List<int> activeEnemies;
    public int activeEnemyCount;

    public override void OnStartServer()
    {
        activeEnemies = new List<int>();
        StartCoroutine(ShouldSpawn());
    }

    private void Update()
    {
        if (isServer)
            activeEnemyCount = activeEnemies.Count;
    }

    private void Spawn()
    {
        var offset = Random.insideUnitCircle;
        var spawnPosition = transform.position + new Vector3(offset.x, 0f, offset.y);
        var obj = Instantiate(enemy, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(obj.gameObject);
        
        activeEnemies.Add(obj.gameObject.GetInstanceID());
        
        obj.GetComponent<Enemy>()?.OnDeath.AddListener((objId) =>
        {
            activeEnemies.Remove(objId);
        });
        
    }

    IEnumerator ShouldSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnFrequency);
            if (activeEnemies.Count < maxSpawnedEnemies) Spawn();
        }
    }
}
