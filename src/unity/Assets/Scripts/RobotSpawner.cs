using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotSpawner : MonoBehaviour
{
    [Header("Line Pos")]
    [SerializeField] private List<Transform> line1;
    [SerializeField] private List<Transform> line2;
    [SerializeField] private List<Transform> line3;

    [Header("Wave Info")]
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private int waveSize;
    [SerializeField] private float waveTerm;

    private int robotNum = 0;

    private IEnumerator SpawnRobots()
    {
        for (int i = 0; i < waveSize; i++)
        {
            Instantiate(robotPrefab).GetComponent<Robot>().InitialSetting(robotNum, true, LinePos(line1));
            Instantiate(robotPrefab).GetComponent<Robot>().InitialSetting(robotNum, false, LinePos(line1));
            robotNum += 1;
            Instantiate(robotPrefab).GetComponent<Robot>().InitialSetting(robotNum, true, LinePos(line2));
            Instantiate(robotPrefab).GetComponent<Robot>().InitialSetting(robotNum, false, LinePos(line2));
            robotNum += 1;
            Instantiate(robotPrefab).GetComponent<Robot>().InitialSetting(robotNum, true, LinePos(line3));
            Instantiate(robotPrefab).GetComponent<Robot>().InitialSetting(robotNum, false, LinePos(line3));
            robotNum += 1;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private List<Vector3> LinePos(List<Transform> line)
    {
        List<Vector3> linePos = new List<Vector3>();
        for (int i = 0; i < line.Count; i++)
        {
            linePos.Add(line[i].position);
        }

        return linePos;
    }
}
