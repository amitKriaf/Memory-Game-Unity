using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public List<Button> buttons = new List<Button>();
    [SerializeField] private Sprite bgImage;
    public List<Sprite> gameCards = new List<Sprite>();
    public Sprite[] cards;
    public Button startButton;
    public GameObject endScreen; 
    public TextMeshProUGUI endScreenText; 
    public Button restartButton; 
    public AudioClip flipSound, clapSound; 
    private AudioSource _audioSource; 

    private int _countGuess, _countCorrectGuess, _gameGuesses, _firstGuessIndex, _secondGuessIndex;
    private bool _firstGuess, _secondGuess;
    private float _timer, _delay = 1f;
    private bool _isProcessing;
    private bool _gameStarted;
    
    
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
        endScreen.SetActive(false);
        restartButton.interactable = false; 
        SetCardsTransparency(0f); 
        HideButtons(); 
        _audioSource = GetComponent<AudioSource>();
    }


    private void HideButtons()
    {// hiding the buttons in the first scene "start game"
        foreach (Button btn in buttons)
        {
            btn.gameObject.SetActive(false); 
            btn.interactable = false; 
        }
    }



    void Update()
    {
        if (_isProcessing)
        {
            _timer += Time.deltaTime;
            if (_timer >= _delay)
            {
                CheckIfCardsMatch();
                _timer = 0f;
                _isProcessing = false;
            }
        }
    }

    private void GetButtons()
    {
        buttons.Clear();
        
        GameObject[] objects = GameObject.FindGameObjectsWithTag("MemoryGame");
        foreach (GameObject obj in objects)
        {
            Button btn = obj.GetComponent<Button>();
            buttons.Add(btn);
            btn.image.sprite = bgImage;
            btn.onClick.RemoveAllListeners(); 
            btn.onClick.AddListener(() => PickACard());
            btn.interactable = false;
        }
    }

    private void AddGameCard()
    {
        gameCards.Clear(); 
        
        int looper = buttons.Count;
        int index = 0;
        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2)
                index = 0;
            gameCards.Add(cards[index]);
            index++;
        }
        // shuffle the cards places
        for (int i = 0; i < gameCards.Count; i++)
        {
            Sprite temp = gameCards[i];
            int randomIndex = Random.Range(0, gameCards.Count);
            gameCards[i] = gameCards[randomIndex];
            gameCards[randomIndex] = temp;
        }
    }

    private void PickACard()
    { // choosing two cards and checking if match
        if (_isProcessing || !_gameStarted) return;

        Button clickedButton =
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        int index = int.Parse(clickedButton.name);
        
        if (_audioSource != null && flipSound != null)
        {
            _audioSource.PlayOneShot(flipSound);
        }

        if (!_firstGuess)
        {
            _firstGuess = true;
            _firstGuessIndex = index;
            clickedButton.image.sprite = gameCards[_firstGuessIndex];
        }
        else if (!_secondGuess)
        {
            _secondGuess = true;
            _secondGuessIndex = index;
            clickedButton.image.sprite = gameCards[_secondGuessIndex];

            _countGuess++;
            _isProcessing = true;
        }
    }

    private void CheckIfCardsMatch()
    {// checking if the cards chosen are  the same
        if (gameCards[_firstGuessIndex].name == gameCards[_secondGuessIndex].name)
        {
            _countCorrectGuess++;
            CheckIfGameFinished();
            buttons[_firstGuessIndex].interactable = false;
            buttons[_secondGuessIndex].interactable = false;
            buttons[_firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            buttons[_secondGuessIndex].image.color = new Color(0, 0, 0, 0);
        }
        else
        {
            buttons[_firstGuessIndex].image.sprite = bgImage;
            buttons[_secondGuessIndex].image.sprite = bgImage;
        }

        _firstGuess = false;
        _secondGuess = false;
    }

    private void CheckIfGameFinished()
    {//checks if there is no cards left
        if (_countCorrectGuess == _gameGuesses)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        endScreen.SetActive(true);
        endScreenText.text = $"Number of tries: {_countGuess}";
        
        if (_audioSource != null && clapSound != null)
        {
            _audioSource.PlayOneShot(clapSound);
        }

        restartButton.interactable = true; 
    }


    private void StartGame()
    {
        GetButtons(); 
        AddGameCard(); 
        _gameGuesses = gameCards.Count / 2;
        _gameStarted = true;

        foreach (Button btn in buttons)
        {
            btn.gameObject.SetActive(true);
            btn.interactable = true;
            btn.image.color = Color.white;
        }

        SetCardsTransparency(1f);
        startButton.gameObject.SetActive(false); 
    }

    private void RestartGame()
    { // if the player chose to restart
        _countGuess = 0;
        _countCorrectGuess = 0;
        _gameStarted = false;
        endScreen.SetActive(false);
        
        buttons.Clear();
        gameCards.Clear();
        
        StartGame();
    }

    private void SetCardsTransparency(float alpha)
    {// setting the cards transparncy - shown or not
        foreach (Button btn in buttons)
        {
            CanvasGroup canvasGroup = btn.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
        }
    }
}
