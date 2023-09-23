using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] float waitForStart;
    [SerializeField] bool loadScene;
    [SerializeField] public bool isAlive;
    Rect textPosition = new Rect(0, 0, 0, 0);
    private GUIStyle countStyle = new GUIStyle();
    private GUIStyle restartStyle = new GUIStyle();
    private int waitingCount;
    private Transform playerTransform;
    private Vector3 actualPosition;
    private Vector3 targetTransform;

    private void Awake()
    {
        Instance = this;
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void Start()
    {
        loadScene = true;
        isAlive = true;
        GameObject.Find("Player").GetComponent<Animator>().enabled = false;
        countStyle.fontSize = 50;
        restartStyle.fontSize = 50;
        restartStyle.fontStyle = FontStyle.BoldAndItalic; 
    }

    private void LateUpdate()
    {
        TargetPosition();
        if (loadScene)
        {
            WaitForStart();
        }
    }

    public void StartGame()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().CanMove = true;
        GameObject.Find("Player").GetComponent<Animator>().enabled = true;
        GameObject.Find("CurveLevel").GetComponent<ShaderValueModifier>().enabled = true;
    }

    public void GameOver()
    {
        isAlive = false;
        GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        GameObject.Find("CurveLevel").GetComponent<ShaderValueModifier>().StopAllCoroutines();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Prototype");
    }

    public void WaitForStart()
    {
        if (isAlive)
        {
            if (waitForStart >= 0)
            {
                waitForStart -= 1 * Time.deltaTime;
            }
            if (waitForStart <= 0)
            {
                StartGame();
                isAlive = false;
            }
        }
    }

    private void OnGUI()
    {
        waitingCount = (int)Math.Round(waitForStart);
        if (waitForStart >= 0)
            GUI.Label(new Rect(Screen.width -150,10,100,50),waitingCount.ToString(), countStyle);
        if (GUI.Button(new Rect(Screen.width -250,60,220,60),"RESTART", restartStyle))
        {
           LoadScene();
        }
    }

    private void TargetPosition()
    {
        if (GameObject.Find("Player").GetComponent<PlayerController>().enabled == false)
        {
            actualPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
            targetTransform = new Vector3(playerTransform.position.x, .07f, playerTransform.position.z);
            playerTransform.position = Vector3.Lerp(actualPosition, targetTransform, 7f * Time.deltaTime);
        }
    }
}
