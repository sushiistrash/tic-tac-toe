using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TicTacToeSettings : MonoBehaviour
{
    [SerializeField] private TicTacToeController _ticTacToeController;
    [SerializeField] private Text _player1WinsText;
    [SerializeField] private Toggle _player1AiToggle;
    [SerializeField] private Text _player2WinsText;
    [SerializeField] private Toggle _player2AiToggle;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Button _startButton;
    [SerializeField] private float _buttonBlinkSpeed = 4;
    [SerializeField] private float _buttonBlinkDuration = 3.0f;

    private int _player1Score = 0;
    private int _player2Score = 0;

    private void Start()
    {
        _ticTacToeController.OnGameOver = OnGameOver;
        StartCoroutine(StartButtonBlinkCoroutine());
    }

    public void OnPlayer1AiToggled(bool isActive)
    {
        _ticTacToeController.IsPlayer1Ai = isActive;
    }
    public void OnPlayer2AiToggled(bool isActive)
    {
        _ticTacToeController.IsPlayer2Ai = isActive;
    }

    public void OnStartClicked()
    {
        _startButton.interactable = false;
        _ticTacToeController.StartGame();
    }

    private IEnumerator StartButtonBlinkCoroutine()
    {
        float animationCounter = 0;

        float h = 0.35f;
        float s = 1;
        float v;

        while (true)
        {
            ColorBlock colors = _startButton.colors;
            if (animationCounter >= _buttonBlinkDuration)
            {
                colors.normalColor = Color.HSVToRGB(h, s, 0);
                _startButton.colors = colors;
                _gameOverText.text = "";
                yield break;
            }
            animationCounter += Time.deltaTime;
            v = Mathf.Abs(Mathf.Sin(animationCounter * _buttonBlinkSpeed) * 0.5f);
            colors.normalColor = Color.HSVToRGB(h, s, v);
            _startButton.colors = colors;
            yield return null;
        }
    }

    public void OnGameOver(int win)
    {
        if (win == -1)
        {
            _gameOverText.text = "Ничья";
        }
        else if (win == 0)
        {
            _gameOverText.text = "Первый игрок победил";
            _player1Score++;
            _player1WinsText.text = "Побед: " + _player1Score;
        }
        else
        {
            _gameOverText.text = "Второй игрок победил";
            _player2Score++;
            _player2WinsText.text = "Побед: " + _player2Score;
        }

        _startButton.interactable = true;
        StartCoroutine(StartButtonBlinkCoroutine());
    }
}
