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
        public int count;        // ������ ���� ��
        public float spawnRate;  // ���� �� ���� ����
    }

    public WaveData[] waves;     // �ʱ� ������ ���̺� ����Ʈ
    public Transform spawnPoint; // ���� ���� ��ġ
    public float waveInterval;   // ���̺� �� ��� �ð�

    public int maxNum;

    private int currentWaveIndex = 0;

    private void Start()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    //�ڷ�ƾ���� ���̺� ������
    private IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            if (currentWaveIndex < waves.Length)
            {
                // ������ ���̺갡 ���������� �� ���̺� ����
                yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
                currentWaveIndex++;
            }
            else
            {
                //���� ���̺�
                //WaveData newWave = GenerateInfiniteWave();
                //yield return StartCoroutine(SpawnWave(newWave));
            }

            // ���� ���̺���� ���
            yield return new WaitForSeconds(waveInterval);
        }
    }

    private IEnumerator SpawnWave(WaveData waveData)
    {
        for (int i = 0; i < waveData.count; i++)
        {
            if (this.transform.childCount < maxNum) //�ִ� ���� �� ����
            {
                GameObject obj = Instantiate(waveData.monsterPrefab, spawnPoint.position, Quaternion.identity);
                obj.transform.SetParent(this.transform);
                //�� ���ͺ��� 0~2�� ��ȣ ���Ƿ� �ο�, 3���� ���Ϳ��̺� ��ġ��
                obj.GetComponent<MonsterMove>().layer = Random.Range(0, 3) + 1;
            }

            yield return new WaitForSeconds(1f / waveData.spawnRate);
        }
    }

    // ���� ���̺� �������� ����
    private WaveData GenerateInfiniteWave()
    {
        WaveData wave = new WaveData();
        wave.monsterPrefab = waves[waves.Length - 1].monsterPrefab;
        wave.count = Random.Range(5, 15);
        wave.spawnRate = Random.Range(1f, 3f);
        return wave;
    }
}
