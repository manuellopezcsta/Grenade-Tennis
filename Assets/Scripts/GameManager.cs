using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public bool NeedANewGranade;
    public int Player1points = 0;
    public int Botpoints = 0;
    private string _whoScored;

    public GameObject granadePrefab;
    Vector3 _spawnPos;

    public Player Player1;
    public Bot Bot1;

    public Transform P1Transform; // We get their transforms to reset their position after a point.
    public Transform BTransform;
    public Transform P1StartingPos;
    public Transform BStartingPos;

    public string CurrentCourtside; // La creo aca xq sino no podria revisarla luego de q se borro la granada.

    [SerializeField] Text PlayerScoreText;
    [SerializeField] Text BotScoreText;

    public int _PointsToWin = 3;

    // COSAS DE LA STATE MACHINE
    [Header("Panels")]
	[Space]
    public GameObject panelMenu; // Linkeamos los paneles.
    public GameObject panelPlay;
    public GameObject panelPlayerS;
    public GameObject panelBotS;
    public GameObject panelAfterGame;
    public GameObject panelHowTo;

    public enum State { MENU, INIT, PLAY, SCORED, AFTERGAME, HOWTO} // Esto es una state machine, con los diff estados del juego.
    State _state; // Esta variable va a guardar el estado actual del juego
    bool _isSwitchingState;

    public Text VictoryText;

    // MUSICS //

    public GameObject[] musics;
    public GameObject[] GSounds;
    GameObject _currentmusic;
    GameObject _matchmusic;
    GameObject _granadeBoom;


    void Start()
    {
        Player1 = GameObject.Find("Player").GetComponent<Player>();
        //_currentmusic = Instantiate(musics[0]);
        //Bot1 = GameObject.Find("Bot").GetComponent<Bot>(); //This line was not working, so i had to manually asign it in the editor.
        SwitchState(State.MENU); // Asi llamamos al estado.
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedANewGranade)
        {
            AfterExplotion();
            NeedANewGranade = false;
        }
    }
    void AfterExplotion()
    {
        //Debug.Log("GM BOOM");

        // Asignar un punto a un jugador
        AssignPoint();

        // Play the sound of scoring.
        if(_currentmusic != null)
        {
            Destroy(_currentmusic);
        }        
        _currentmusic = Instantiate(musics[7]);

        // Change the Score.
        ChangeScoreboard();

        // Reset Players Position.
        ResetPlayersPosition();

        // Spawnear Pelota
        //_spawnPos = new Vector3(-5f,4.5f,-24f);
        _spawnPos = new Vector3(-5f,15f,-24f);
        GameObject granade = Instantiate(granadePrefab, _spawnPos, Quaternion.identity);
        // o w o B a n a n a        

        // Tell the players about the new ball object.
        Player1.Ball = granade.transform;
        Bot1.Ball = granade.transform;

        // Check for Win condition
        CheckWinCondition();

        // Re-enable Serving.
        Player1.HasServed = false;
    }

    void AssignPoint()
    {
        if(CurrentCourtside == "Player Side")
        {
            Botpoints++;
            Debug.Log("Bot Scores");
            _whoScored = "bot";
        } else if(CurrentCourtside == "Bot Side")
        {
            Player1points++;
            Debug.Log("Player Scores");
            _whoScored = "player";
        }
        if(Player1points <= _PointsToWin - 1 && Botpoints <= _PointsToWin - 1)
        {
            SwitchState(State.SCORED,0f,"e");
        }         
    }

    void ResetPlayersPosition()
    {
        P1Transform.position = P1StartingPos.position;
        BTransform.position = BStartingPos.position;

    }

    void ChangeScoreboard(){
        PlayerScoreText.text = "Player: " + Player1points;
        BotScoreText.text = "Bot: " + Botpoints; 
    }

    void CheckWinCondition(){
        if (Player1points >= _PointsToWin)
        {
            Debug.Log("Player Wins");
            SwitchState(State.AFTERGAME,0f,"d player");
        }
        if (Botpoints >= _PointsToWin)
        {
            Debug.Log("Bot Wins");
            SwitchState(State.AFTERGAME,0f,"d bot");
        }
    }

    // Funciones de Estados
    public void SwitchState(State newState, float delay = 0, string msg = "")
    { // Esto es lo que cambia el estado.
        Debug.Log(msg);
        StartCoroutine(SwitchDelay(newState, delay));
    }
    IEnumerator SwitchDelay(State newState, float delay)
    { // Algo de una corutina q espera, no entendi.
        _isSwitchingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        _state = newState;
        BeginState(newState);
        _isSwitchingState = false;
    }

    void BeginState(State newState)
    {
        switch(_state){
            case State.MENU:
                panelMenu.SetActive(true);
                if(_currentmusic != null) {
                    Destroy(_currentmusic);
                }
                _currentmusic = Instantiate(musics[0]);
                Debug.Log("E Menu");
                break;
            case State.INIT:
                Debug.Log("E INIT");
                panelPlay.SetActive(true);
                Player1.GameStarted = true;
                _matchmusic = Instantiate(musics[6]);
                SwitchState(State.PLAY,0f,"b");
                break;
            case State.PLAY:
                Debug.Log("E PLAY");
                break;
            case State.SCORED:
                Debug.Log("E SCORED");
                StartCoroutine(ShowScorer(2f));
                SwitchState(State.PLAY,2f,"c");
                break;
            case State.AFTERGAME:
                Debug.Log("E AFTERGAME");
                VictoryTextUpd();
                panelAfterGame.SetActive(true);
                //Debug.Log("Aftergame enabled");

                // Destruimos las cosas para evitar problemas
                Player1.GameStarted = false;
                Destroy(Player1);
                Destroy(Bot1);
                Destroy(GameObject.Find("Granada(Clone)"));
                break;
            case State.HOWTO:
                Debug.Log("E HOWTO");
                panelHowTo.SetActive(true);
                break;
        }
    }



    void EndState()
    {
        switch(_state){
            case State.MENU:
                Debug.Log("S MENU");
                panelMenu.SetActive(false);
                break;
            case State.INIT:
                Debug.Log("S INIT");
                break;
            case State.PLAY:
                Debug.Log("S PLAY");
                break;
            case State.SCORED:
                panelPlayerS.SetActive(false);
                panelBotS.SetActive(false);
                Debug.Log("S SCORED");
                if(Player1points <= 2 && Botpoints <=2)
                {
                    SwitchState(State.PLAY,0f,"a");
                }                
                break;
            case State.AFTERGAME:
                panelPlay.SetActive(false);
                panelAfterGame.SetActive(false);
                Player1points = 0;
                Botpoints = 0;
                //Debug.Log("After Game Disabled");
                Debug.Log("S AFTERGAME");
                if(_matchmusic != null)
                {
                    Destroy(_matchmusic);
                }
                break;
            case State.HOWTO:
                panelHowTo.SetActive(false);
                Debug.Log("S HOWTO");
                break;
        }
    }

    // Botones
    public void PlayClicked(){ // Le decimos si se hace click en play cambiame el estado a cargando.
        SwitchState(State.INIT);
        Debug.Log("BOTON PLAY CLICKEADO");
        Destroy(_currentmusic); // Destruimos la musica de menu al iniciar el juego.
    }
    public void HowToClicked() { // Le decimos si se hace click en play cambiame el estado a cargando.
        SwitchState(State.HOWTO);
        Debug.Log("BOTON HOW TO CLICKEADO");
    }
    public void BackClicked() { // Le decimos si se hace click en play cambiame el estado a cargando.
        SwitchState(State.MENU);
        Debug.Log("BOTON BACK CLICKEADO");
    }
    public void Back2Clicked() { // Le decimos si se hace click en play cambiame el estado a cargando.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Relodeando Escena.");
    }

    IEnumerator ShowScorer (float delay) {
        if(_whoScored == "player")
            {
                panelPlayerS.SetActive(true);
            }
            if(_whoScored == "bot")
            {
                panelBotS.SetActive(true);
            }
        yield return new WaitForSeconds(delay);
        panelPlayerS.SetActive(false);
        panelBotS.SetActive(false);
    }
    void VictoryTextUpd()
    {
        if(Player1points >= 3)
        {
            VictoryText.text = "PLAYER WINS";
        } else if(Botpoints >= 3)
        {
            VictoryText.text = "BOT WINS";
        } else {
            VictoryText.text = "GUCCI BANANA NOT FOUND?";
        }        
    }

    public void PlaySound()
    {
        if(_currentmusic != null)
        {
            Destroy(_currentmusic);
        }        
        _currentmusic = Instantiate(musics[Random.Range(1,6)]);
    }

    public void PlayBoom()
    {
        if(_granadeBoom != null)
        {
            Destroy(_currentmusic);
        }        
        _granadeBoom = Instantiate(GSounds[Random.Range(0,3)]);
    }
}