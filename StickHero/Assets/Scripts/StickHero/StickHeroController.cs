using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StickHeroController : MonoBehaviour
{
    [SerializeField] private StickHeroStick m_Stick;
    [SerializeField] private StickHeroPlayer m_Player;
    [SerializeField] private PlatfromSpawn m_Spawner;
    [SerializeField] private List<StickHeroPlatform> m_Platforms;

    private int counter; //счетчик платформ

    public enum EGameState
    {
        Wait,
        Scaling,
        Rotate,
        Movement,
        Defeat,

    }

    private EGameState currentGameState;
        
    void Start()
    {
        counter = 0;
        m_Stick.ResetStick(m_Platforms[0].GetStickPosition());
        currentGameState = EGameState.Wait;
        m_Spawner.CreatePlatform();
        m_Platforms[counter] = LookForPlatforms();
    }


    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        switch (currentGameState)
        {
       
            //если не осуществлен старт игры
            case EGameState.Wait:
                currentGameState = EGameState.Scaling;
                m_Stick.StartScaling();
                break;
            //если стик увеличивается - прерываем увеличение и запускаем поворот
            case EGameState.Scaling:
                currentGameState = EGameState.Rotate;
                m_Stick.StopScaling();
                break;
            //ничего не делаем
            case EGameState.Rotate:
                break;
            case EGameState.Movement:
                break;
            case EGameState.Defeat:
                print("Defeat");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void StopStickScale()
    {
        currentGameState = EGameState.Rotate;
        m_Stick.StartRotate();
    }

    public void StopStickRotate()
    {
        currentGameState = EGameState.Movement;
    }

    public void StartPlayerMovement(float length)
    {
        currentGameState = EGameState.Movement;
        StickHeroPlatform nextPlatform = m_Platforms[counter];
        
        //находим минимальную длинну стика для успешного перехода
        float targetLength = nextPlatform.transform.position.x - m_Stick.transform.position.x;
        float platformSize = nextPlatform.GetPlatformSize();
        float min = targetLength - platformSize * 0.5f;
        min -= m_Player.transform.localScale.x * 0.9f;
        
        //находим максимальную длинну стика для успешного перехода
        float max = targetLength + platformSize * 0.5f;
        
        //при успехе переходим в центр следующей платформы, иначе падаем

        if (length < min || length > max)
        {
            //будем падать
            float targetPosition = m_Stick.transform.position.x + length + m_Player.transform.localScale.x;
            m_Player.StartMovement(targetPosition, true);
        }
        else
        {
            float targetPosition = nextPlatform.transform.position.x;
            m_Player.StartMovement(targetPosition,false);
        }

    }

    public void StopPlayerMovement()
    {
        m_Stick.ResetStick(m_Platforms[counter].GetStickPosition());
        counter++;
        m_Spawner.CreatePlatform();
        m_Platforms.Add(LookForPlatforms());
        currentGameState = EGameState.Wait;
    }

    public void ShowScores()
    {
        currentGameState = EGameState.Defeat;
        print($"Game over at {counter}");
    }

    public StickHeroPlatform LookForPlatforms()
    {
        GameObject[] allActivePlatforms = GameObject.FindGameObjectsWithTag("Platform");
        return allActivePlatforms[counter+1].GetComponent<StickHeroPlatform>();
    }
}
