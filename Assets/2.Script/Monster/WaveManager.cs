using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveData
    {
        public GameObject monsterPrefab;
        public int count;        // 스폰할 몬스터 수
        public float spawnRate;  // 몬스터 간 스폰 간격
    }

    public WaveData[] waves;     // 초기 설정된 웨이브 리스트
    public Transform spawnPoint; // 몬스터 스폰 위치
    public float waveInterval;   // 웨이브 간 대기 시간

    public int maxNum;

    private int currentWaveIndex = 0;

    private void Start()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    //코루틴으로 웨이브 보내기
    private IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            if (currentWaveIndex < waves.Length)
            {
                // 설정된 웨이브가 남아있으면 그 웨이브 실행
                yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
                currentWaveIndex++;
            }
            else
            {
                //무한 웨이브
                //WaveData newWave = GenerateInfiniteWave();
                //yield return StartCoroutine(SpawnWave(newWave));
            }

            // 다음 웨이브까지 대기
            yield return new WaitForSeconds(waveInterval);
        }
    }

    private IEnumerator SpawnWave(WaveData waveData)
    {
        for (int i = 0; i < waveData.count; i++)
        {
            if (this.transform.childCount < maxNum) //최대 몬스터 수 제한
            {
                GameObject obj = Instantiate(waveData.monsterPrefab, spawnPoint.position, Quaternion.identity);
                obj.transform.SetParent(this.transform);
                //각 몬스터별로 0~2번 번호 임의로 부여, 3겹의 몬스터웨이브 겹치기
                obj.GetComponent<MonsterMove>().layer = Random.Range(0, 3) + 1;
            }

            yield return new WaitForSeconds(1f / waveData.spawnRate);
        }
    }

    // 무한 웨이브 랜덤으로 생성
    private WaveData GenerateInfiniteWave()
    {
        WaveData wave = new WaveData();
        wave.monsterPrefab = waves[waves.Length - 1].monsterPrefab;
        wave.count = Random.Range(5, 15);
        wave.spawnRate = Random.Range(1f, 3f);
        return wave;
    }
}
