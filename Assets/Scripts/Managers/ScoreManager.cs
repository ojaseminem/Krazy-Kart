using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public GameObject[] taskObjects;

    public static int totalScore;
    public static int currentScore;

    void Start()
    {
        currentScore = 0;
        totalScore = taskObjects.Length;
    }
}
