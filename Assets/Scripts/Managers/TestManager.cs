using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;


public class TestManager : MonoBehaviour
{

    private static TestManager _instance;

    public static TestManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindAnyObjectByType<TestManager>();
            }
            return _instance;
        }
    }

    public GameObject EndMiniGameScreen, MiniGameScreen;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMiniGameNormal()
    {
        MiniGameScreen.SetActive(true);
        EndMiniGameScreen.SetActive(false);
        MinigameManager.instance.InitializeNormalMiniGame(EndMiniGame);
    }

    public void StartMiniGameBounce()
    {
        MiniGameScreen.SetActive(true);
        EndMiniGameScreen.SetActive(false);
        MinigameManager.instance.InitializeBounceMiniGame(EndMiniGame);
    }

    public void StartMiniGameCounterClock()
    {
        MiniGameScreen.SetActive(true);
        EndMiniGameScreen.SetActive(false);
        MinigameManager.instance.InitializeCounterClockGame(EndMiniGame);
    }

    public void EndMiniGame()
    {
        EndMiniGameScreen.SetActive(true);
        MiniGameScreen.SetActive(false);
    }

    public void modifyValuesNoOfTargets(string value)
    {
        MinigameManager.instance.numberOfTargets = int.Parse(value);
    }

    public void modifyValuesVelocity(string value)
    {
        MinigameManager.instance.velocity = float.Parse(value);
    }

    public void modifyValuesTargetsToWin(string value)
    {
        MinigameManager.instance.progressTarget = int.Parse(value);
    }

    public void modifyValuesTargetRangeTrigger(string value)
    {
        MinigameManager.instance.targetRangeTrigger = float.Parse(value);
    }
}
