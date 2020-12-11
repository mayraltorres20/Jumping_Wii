using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState{Idle, Playing, Fin, Replay};

public class JuegoControl : MonoBehaviour{

    [Range(0f, 0.20f)]
    public float parallaxSpeed = 0.02f;
    public RawImage background;
    public RawImage platform;
    public GameObject uiIdle;
    public GameObject uiScore;
    public Text pointsText;
    public Text recordText;
    public GameState gameState = GameState.Idle;
    public GameObject uiIdel;
    
    
    //public Text recordText;
       
    public GameObject Player;
    public GameObject enemyGenerator;

    public float scaleTime = 6f;
    public float scaleInc = .25f;
    
    private AudioSource musicPlayer;
    private int points = 0;

    // Start is called before the first frame update
    void Start()
    {
       musicPlayer = GetComponent<AudioSource>();
       recordText.text = "BEST: " + GetMaxScore().ToString(); 
    }

    // Update is called once per frame
    void Update()
    {   
        bool userAction = Input.GetKeyDown("up") || Input.GetMouseButtonDown(0);

        //comienza el juego
        if(gameState == GameState.Idle && userAction){
            gameState = GameState.Playing;
            uiIdle.SetActive(false);
            uiScore.SetActive(true);
            uiIdle.SetActive(false);
            Player.SendMessage("UpdateState", "Player_run");
            Player.SendMessage("DustPlay");
            enemyGenerator.SendMessage("StartGenerator");
            musicPlayer.Play();
            InvokeRepeating("GameScale", scaleTime, scaleTime);
        }

        //Juego corriendo
        else if(gameState == GameState.Playing){
            Parallax();
        } 
        //Juego preparado para reinicio
        else if(gameState == GameState.Replay){
            if(userAction){
                RestartGame();
            }
        } 
    }
    //movimiento paisaje
    void Parallax()
    {        
            float finalSpeed = parallaxSpeed * Time.deltaTime;
            background.uvRect = new Rect(background.uvRect.x + finalSpeed, 0f, 1f, 1f);
            platform.uvRect = new Rect(platform.uvRect.x + finalSpeed * 4, 0f, 1f, 1f); 
    }

    //reinicio juego puntos
    public void RestartGame(){
        ResetTimeScale();
        SceneManager.LoadScene("Principal");
    }

    void GameScale(){
        Time.timeScale += scaleInc;
        Debug.Log("Ritmo incrementado:" + Time.timeScale.ToString());
    }
    
    public void ResetTimeScale(float newTimeScale = 1f){
        CancelInvoke("GameTimeScale");
        Time.timeScale = newTimeScale;
        Debug.Log("Ritmo restablecido:" + Time.timeScale.ToString());
    }

    public void IncreasePoints(){
        pointsText.text = (++points).ToString();
        if(points >= GetMaxScore()){
            recordText.text = "BEST: " + points.ToString();
            SaveScore(points);
        }
    }

    public int GetMaxScore(){
        return PlayerPrefs.GetInt("Max Points", 0);
    }

    public void SaveScore(int currentPoints){
        PlayerPrefs.SetInt("Max Points", currentPoints);
    }
}
