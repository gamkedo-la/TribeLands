using System.Collections;
using Mirror;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Time between spawns, in seconds")]
    private float spawnFrequency = 3.0f;

    [SerializeField]
    private GameObject enemy;
    
    void Start()
    {
        if (isServer)
        {
            StartCoroutine(ShouldSpawn());
        }
    }

    private void Spawn()
    {
        var offset = Random.insideUnitCircle;
        var spawnPosition = transform.position + new Vector3(offset.x, 0f, offset.y);
        var obj = Instantiate(enemy, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(obj.gameObject);
    }

    IEnumerator ShouldSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnFrequency);
            Spawn();
        }
    }
}
