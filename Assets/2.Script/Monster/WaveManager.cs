using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveData
    {
        public GameObject monsterPrefab;
        public int count;        // 스폰할 몬스터 수
        public float spawnRate;  // 몬스터 간 스폰 간격
    }

    public WaveData[] waves;         // 초기 설정된 웨이브 리스트
    public Transform spawnPoint;     // 몬스터 스폰 위치
    public float waveInterval;       // 웨이브 간 대기 시간
    public int maxNum;               // 동시에 활성화할 최대 몬스터 수

    private int currentWaveIndex = 0;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Start()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    // 웨이브 보내기
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

    //웨이브 하나
    private IEnumerator SpawnWave(WaveData waveData)
    {
        for (int i = 0; i < waveData.count; i++)
        {
            if (GetActiveChildCount() < maxNum)
            {
                GameObject obj = GetPooledObject(waveData.monsterPrefab, spawnPoint.position, Quaternion.identity);
                obj.transform.SetParent(this.transform);

                //각 몬스터별로 레이어 1~3중에 하나 랜덤으로 지정하기
                //3겹으로 만들어서 쌓기
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

    //오브젝트 풀에서 프리팹을 키값으로 재활용할 몬스터 있는지 보고 없으면 가져오기
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

        //재활용 가능한 오브젝트가 없으면 새로 생성
        GameObject newObj = Instantiate(prefab, position, rotation);
        PooledObject pooledComp = newObj.AddComponent<PooledObject>();
        pooledComp.prefab = prefab;
        pooledComp.poolManager = this;
        newObj.GetComponent<MonsterManager>().pooledObject = pooledComp;
        return newObj;
    }

    //재활용용 함수
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

//재활용용 클래스
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