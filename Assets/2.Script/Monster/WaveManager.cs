using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveData
    {
        public GameObject monsterPrefab;
        public int count;        // ������ ���� ��
        public float spawnRate;  // ���� �� ���� ����
    }

    public WaveData[] waves;         // �ʱ� ������ ���̺� ����Ʈ
    public Transform spawnPoint;     // ���� ���� ��ġ
    public float waveInterval;       // ���̺� �� ��� �ð�
    public int maxNum;               // ���ÿ� Ȱ��ȭ�� �ִ� ���� ��

    private int currentWaveIndex = 0;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Start()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    // ���̺� ������
    private IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            if (currentWaveIndex < waves.Length)
            {
                yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
                currentWaveIndex++;
            }
            else
            {
                // WaveData newWave = GenerateInfiniteWave();
                // yield return StartCoroutine(SpawnWave(newWave));
            }

            yield return new WaitForSeconds(waveInterval);
        }
    }

    //���̺� �ϳ�
    private IEnumerator SpawnWave(WaveData waveData)
    {
        for (int i = 0; i < waveData.count; i++)
        {
            if (GetActiveChildCount() < maxNum)
            {
                GameObject obj = GetPooledObject(waveData.monsterPrefab, spawnPoint.position, Quaternion.identity);
                obj.transform.SetParent(this.transform);

                //�� ���ͺ��� ���̾� 1~3�߿� �ϳ� �������� �����ϱ�
                //3������ ���� �ױ�
                MonsterMove move = obj.GetComponent<MonsterMove>();
                move.layer = Random.Range(0, 3) + 1;
            }

            yield return new WaitForSeconds(1f / waveData.spawnRate);
        }
    }

    private int GetActiveChildCount()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
                count++;
        }
        return count;
    }

    //������Ʈ Ǯ���� �������� Ű������ ��Ȱ���� ���� �ִ��� ���� ������ ��������
    private GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
        }

        if (poolDictionary[prefab].Count > 0)
        {
            GameObject pooledObject = poolDictionary[prefab].Dequeue();
            if (pooledObject != null)
            {
                pooledObject.transform.position = position;
                pooledObject.transform.rotation = rotation;
                pooledObject.SetActive(true);
                return pooledObject;
            }
        }

        //��Ȱ�� ������ ������Ʈ�� ������ ���� ����
        GameObject newObj = Instantiate(prefab, position, rotation);
        PooledObject pooledComp = newObj.AddComponent<PooledObject>();
        pooledComp.prefab = prefab;
        pooledComp.poolManager = this;
        newObj.GetComponent<MonsterManager>().pooledObject = pooledComp;
        return newObj;
    }

    //��Ȱ��� �Լ�
    public void ReturnToPool(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
        }
        poolDictionary[prefab].Enqueue(obj);
    }

    private WaveData GenerateInfiniteWave()
    {
        WaveData wave = new WaveData();
        wave.monsterPrefab = waves[waves.Length - 1].monsterPrefab;
        wave.count = Random.Range(5, 15);
        wave.spawnRate = Random.Range(1f, 3f);
        return wave;
    }
}

//��Ȱ��� Ŭ����
public class PooledObject : MonoBehaviour
{
    public GameObject prefab;
    public WaveManager poolManager;

    public PooledObject(GameObject _prefab, WaveManager _poolManager)
    {
        prefab = _prefab;
        poolManager = _poolManager;
    }

    public void ReturnToPool()
    {
        poolManager.ReturnToPool(gameObject, prefab);
    }
}