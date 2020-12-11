using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public GameObject game;
    public GameObject enemyGenerator;
    public AudioClip saltaClip;
    public AudioClip muereClip;
    public AudioClip puntoClip;
    public ParticleSystem dust;
    private Animator animator;
    private AudioSource audioPlayer;
    private float startY;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        startY = transform.position.y;
    }
    // Update is called once per frame
    void Update()
    {   //controla que muera el jugador solo si esta en play
        bool gamePlaying = game.GetComponent<JuegoControl>().gameState == GameState.Playing;
        bool isGraunded = transform.position.y == startY;
        bool userAction = Input.GetKeyDown("up") || Input.GetMouseButtonDown(0);
       
        //detecta el choque contra un enemigo
       if(isGraunded && gamePlaying && userAction){
           UpdateState("Player_Saltar");
           audioPlayer.clip = saltaClip;
           audioPlayer.Play();
       }
    }

    public void UpdateState(string state = null){
        if(state != null){
            animator.Play(state);
        }
    }
    //detecta colicion entre jugador y enemigo y actua en consecuencia
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Enemy"){
           UpdateState("Player_Muere");
           game.GetComponent<JuegoControl>().gameState = GameState.Fin;
           enemyGenerator.SendMessage("CancelGenerator", true);
           game.SendMessage("ResetTimeScale", 0.5f);
          
           //ref audio
           game.GetComponent<AudioSource>().Stop();
           audioPlayer.clip = muereClip;
           audioPlayer.Play();

            DustStop();
        } 
        else if(other.gameObject.tag == "Point"){
           game.SendMessage("IncreasePoints"); 
           audioPlayer.clip = puntoClip;
           audioPlayer.Play();
        }
    } 

    void GameReplay(){
       game.GetComponent<JuegoControl>().gameState = GameState.Replay; 
    } 

    void DustPlay(){
        dust.Play();
    } 

    void DustStop(){
        dust.Stop();
    }
}
