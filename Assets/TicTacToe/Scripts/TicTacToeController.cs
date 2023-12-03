using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeController : MonoBehaviour {

    [HideInInspector] public bool IsPlayer1Ai = false;
    [HideInInspector] public bool IsPlayer2Ai = true;
    [HideInInspector] public float StepDuration = 0;
    [SerializeField] private List<Button> _buttons = new List<Button>();

    public delegate void OnGameOverDelegate(int win);
    public OnGameOverDelegate OnGameOver;

    private bool _turn;
    private int _fieldsLeft;
    private bool _isGameOver = true;

    private int _depth;
    private int _optimalScoreButtonIndex = -1;

    public void StartGame()
    {
        _turn = Mathf.Round(UnityEngine.Random.Range(0, 1)) == 1;
        Reset();
    }

    private void EnableButtons(bool enabled, bool ignoreEmpty = false)
    {
        foreach (Button button in _buttons)
        {
            if (!enabled || ignoreEmpty || IsFieldEmpty(button))
            {
                button.interactable = enabled;
            }
        }
    }

    private bool IsFieldEmpty(Button button)
    {
        return GetText(button).text == "";
    }

    private Text GetText(Button button)
    {
        return button.GetComponentInChildren<Text>();
    }

    private bool SetMarkAndCheckForWin(Button button, bool colorate = false)
    {
        Text text = GetText(button);
        if (text.text != "")
        {
            return false;
        }
        text.text = _turn ? "O" : "X";
        _fieldsLeft--;

        return CheckForWin(text.text, colorate);
    }

    public void OnButtonClick(Button button)
    {
        if (_isGameOver)
        {
            Reset();
            return;
        }
        if (_fieldsLeft <= 0)
        {
            return;
        }

        if (SetMarkAndCheckForWin(button, true))
        {
            Win();
        }
        button.interactable = false;

        if (_fieldsLeft <= 0)
        {
            GameOverDraw();
        }

        _turn = !_turn;

        if (!_isGameOver && _fieldsLeft > 0 && IsAiTurn())
        {
            StartCoroutine(AiTurnCoroutine());
        }
    }

    private bool IsAiTurn()
    {
        return (_turn && IsPlayer1Ai) || (!_turn && IsPlayer2Ai);
    }

    private IEnumerator AiTurnCoroutine()
    {
        EnableButtons(false);
        IEnumerator minMaxEnumerator = MinMaxCoroutine(1);
        
        while (minMaxEnumerator.MoveNext()) {}

        yield return new WaitForSeconds(1.0f);

        Button button = _buttons[_optimalScoreButtonIndex];
        EnableButtons(true);
        OnButtonClick(button);
    }

    private IEnumerator MinMaxCoroutine(int depth)
    {

        int currentBestScore = _turn ? Int32.MinValue : Int32.MaxValue;
        int currentOptimalScoreButtonIndex = -1;

        int fieldIndex = 0;
        while (fieldIndex < _buttons.Count)
        {
            if (IsFieldFree(fieldIndex))
            {
                Button button = _buttons[fieldIndex];
                int currentScore = 0;

                bool endRecursion = false;

                if (StepDuration > 0)
                {
                    yield return new WaitForSeconds(StepDuration);
                }

                if (SetMarkAndCheckForWin(button))
                {
                    currentScore = (_turn ? 1 : -1) * (10 - depth);
                    endRecursion = true;
                }
                else if (_fieldsLeft > 0)
                {
                    _turn = !_turn;
                    IEnumerator minMaxEnumerator = MinMaxCoroutine(depth + 1);
                    while (minMaxEnumerator.MoveNext()) { }
                    currentScore = _depth;
                    _turn = !_turn;
                }

                if ((_turn && currentScore > currentBestScore) || (!_turn && currentScore < currentBestScore))
                {
                    currentBestScore = currentScore;
                    currentOptimalScoreButtonIndex = fieldIndex;
                }
                
                GetText(button).text = "";
                _fieldsLeft++;

                if (endRecursion)
                {
                    break;
                }
            }
            fieldIndex++;
        }

        _depth = currentBestScore;
        _optimalScoreButtonIndex = currentOptimalScoreButtonIndex;
    }

    private bool CheckForWin(string mark, bool colorate = false)
    {
        if (_fieldsLeft > 6) {
            return false;
        }
        if (CompareButtons(0, 1, 2, mark, colorate) || CompareButtons(3, 4, 5, mark, colorate) || CompareButtons(6, 7, 8, mark, colorate) || CompareButtons(0, 3, 6, mark, colorate) || CompareButtons(1, 4, 7, mark, colorate) || CompareButtons(2, 5, 8, mark, colorate) || CompareButtons(0, 4, 8, mark, colorate) || CompareButtons(6, 4, 2, mark, colorate))
        {
            return true;
        }
        return false;
    }

    private bool CompareButtons(int index1, int index2, int index3, string mark, bool colorate = false)
    {
        Text text1 = GetText(_buttons[index1]);
        Text text2 = GetText(_buttons[index2]);
        Text text3 = GetText(_buttons[index3]);
        bool equal = text1.text == mark && text2.text == mark && text3.text == mark;
        if (colorate && equal)
        {
            Color color = _turn ? Color.green : Color.red;
            text1.color = color;
            text2.color = color;
            text3.color = color;
        }
        return equal;
    }

    private bool IsFieldFree(int index) => GetText(_buttons[index]).text.Length == 0;

    private void Win()
    {
        _isGameOver = true;
        EnableButtons(false);
        OnGameOver?.Invoke(_turn ? 0 : 1);
    }
    private void GameOverDraw()
    {
        _isGameOver = true;
        EnableButtons(false);
        OnGameOver?.Invoke(-1);
    }

    private void Reset()
    {
        foreach (Button button in _buttons)
        {
            Text text = GetText(button);
            text.color = Color.white;
            text.text = "";
            button.interactable = true;
        }
        _fieldsLeft = 9;
        _isGameOver = false;
        if (IsAiTurn())
        {
            StartCoroutine(AiTurnCoroutine());
        }
    }
}
