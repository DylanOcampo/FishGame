using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;


public class MinigameManager : MonoBehaviour
{

    private static MinigameManager _instance;

    public static MinigameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindAnyObjectByType<MinigameManager>();
            }
            return _instance;
        }
    }
    


    public int progress;
    public int numberOfTargets;
    public float velocity;
    public int progressTarget;

    public float targetRangeTrigger;

    public List<GameObject> currentTargets = new List<GameObject>();
    public List<float> targetPositions = new List<float>();

    public GameObject cursor, cursorTargetContainer, cursorTargetPrefab;

    public System.Action onTriggerTargetArgs;

    private bool colliderEnabled = false;
    public Image progressBar;

    public string currentMiniGame;
    private bool currentDirection;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!colliderEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                StartCoroutine(CheckForCollisions());
            }
        }
    
    
    }

    IEnumerator CheckForCollisions()
    {
        colliderEnabled = true;
        if(currentTargets.Count > 0)
        {
            foreach(GameObject target in currentTargets)
            {
                float targetZ = target.transform.localRotation.eulerAngles.z;
                if(currentMiniGame == "CounterClock")
                {
                    targetZ += cursorTargetContainer.transform.localRotation.eulerAngles.z;
                }
                while(targetZ > 344)
                {
                    targetZ -= 360;
                }

                float cursorZ = cursor.transform.localRotation.eulerAngles.z;
                Debug.Log(targetZ + " " + cursorZ);
                if(Mathf.Abs(targetZ - cursorZ) < targetRangeTrigger)
                {
                    OnTriggerTarget(target);
                    break;
                }
            }
        }
        Debug.Log("Checked for collisions");
        colliderEnabled = false;
        yield return new WaitForSeconds(0.01f);

        
    }

    private void InitializeMiniGame(){
        progress = 0;
        progressBar.fillAmount = 0;
        currentTargets.Clear();
        targetPositions.Clear();
        targetPositions.Add(0);
        cursorTargetContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        DOTween.KillAll();
        currentDirection = false;
        if(numberOfTargets > 7)
        {
            numberOfTargets = 7;
        }

        cursor.transform.localRotation = Quaternion.Euler(0, 0, 0);
        cursor.transform.DORotate(new Vector3(0, 0, -360), velocity, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        for(int i = 0; i < numberOfTargets; i++)
        {
            SpawnTarget();
        }
        targetPositions.Remove(0);
        currentMiniGame = "";
    }


    public void InitializeNormalMiniGame(System.Action onTriggerTargetArgs)
    {
        this.onTriggerTargetArgs = onTriggerTargetArgs;
        InitializeMiniGame();
        currentMiniGame = "Normal";

    }

    public void InitializeBounceMiniGame(System.Action onTriggerTargetArgs)
    {
        this.onTriggerTargetArgs = onTriggerTargetArgs;
        InitializeMiniGame();
        currentMiniGame = "Bounce";
    }

    public void InitializeCounterClockGame(System.Action onTriggerTargetArgs)
    {
        this.onTriggerTargetArgs = onTriggerTargetArgs;
        InitializeMiniGame();
        cursorTargetContainer.transform.DORotate(new Vector3(0, 0, 360), velocity * 2, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        currentMiniGame = "CounterClock";
    }

    public void OnTriggerTarget(GameObject self)
    {
        if(currentMiniGame == "Bounce")
        {
            float angleHelper = 360;
            if(currentDirection)
            {
                currentDirection = false;
            }
            else
            {
                angleHelper *= -1;
                currentDirection = true;
            }
            cursor.transform.DOKill();
            cursor.transform.DORotate(new Vector3(0, 0, cursor.transform.localRotation.eulerAngles.z - angleHelper), velocity, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        }

        if(currentTargets.Count > 0)
        {
            
            
            progress++;
            progressBar.fillAmount = (float)progress / progressTarget;
            if(progress >= progressTarget)
            {
                if(onTriggerTargetArgs != null)
                {
                    foreach(GameObject target in currentTargets)
                    {
                        Destroy(target);
                    }
                    currentTargets.Clear();
                    targetPositions.Clear();
                    cursor.transform.DOKill();
                    onTriggerTargetArgs.Invoke();
                }
            }
            else
            {
                SpawnTarget();
                currentTargets.Remove(self);
                targetPositions.Remove(self.transform.localRotation.eulerAngles.z);
            }
            Destroy(self);
        }
        
    }

    void SpawnTarget()
    {
        GameObject target = Instantiate(cursorTargetPrefab, cursorTargetContainer.transform);
        
        while(true)
        {
            float randomZ = Random.Range(0, 360);
            float randomZHelper = randomZ;
            bool flag = true;
            if(randomZHelper > 344)
            {
                randomZHelper -= 360;
            }

            foreach(float targetZ in targetPositions)
            {
               if(Mathf.Abs(targetZ - randomZHelper) < 45)
                {
                    flag = false;
                    break;
                }
            }
            if(flag)
            {
                target.transform.localRotation = Quaternion.Euler(0, 0, randomZ);
                currentTargets.Add(target);
                targetPositions.Add(target.transform.localRotation.eulerAngles.z);
                break;
            }
        }

        
    }


}
