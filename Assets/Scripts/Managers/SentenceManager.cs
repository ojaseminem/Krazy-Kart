using System;
using TMPro;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Managers
{
    public class SentenceManager : MonoBehaviour
    {
        private bool _checkForInput;
        public TextMeshProUGUI sentenceOutput;
    
        [HideInInspector] public string currentSentence;
    
        private UiManager _uiManager;
        private string _remainingSentence;

        #region Private

        private void Start()
        {
            _uiManager = GameManager.Instance.uiManager;
            sentenceOutput = _uiManager.taskWindow.taskText;
        }

        private void Update()
        {
            if (!_checkForInput) return;
            CheckInput();
        }

        private void SetRemainingSentence(string newString)
        {
            _remainingSentence = newString;
            sentenceOutput.text = _remainingSentence;
        }

        private void CheckInput()
        {
            if (!Input.anyKeyDown) return;
        
            string keysPressed = Input.inputString;
            if (keysPressed.Length == 1) EnterLetter(keysPressed);
        }
    
        // This Function gets called after the sentence is complete.
        private void EnterLetter(string typedLetter)
        {
            if (!IsCorrectLetter(typedLetter)) return;
            RemoveLetter();

            if (!IsSentenceComplete()) return;
            _checkForInput = false;
        
            _uiManager.CloseWindow(Windows.Task);
            GameManager.Instance.ChangeState(GameState.Playing);
        }
    
        private bool IsCorrectLetter(string letter) => _remainingSentence.IndexOf(letter, StringComparison.Ordinal) == 0;

        private void RemoveLetter()
        {
            string newString = _remainingSentence.Remove(0, 1);
            SetRemainingSentence(newString);
        }

        private bool IsSentenceComplete() => _remainingSentence.Length == 0;

        private string RandomSentence()
        {
            int randomSentenceIndex = Random.Range(0, JsonReader.GetSentenceBank().Length);
            string randomSentence = JsonReader.GetSentenceBank()[randomSentenceIndex];

            return randomSentence;
        }

        #endregion

        #region Public

        public void SetCurrentSentence()
        {
            _checkForInput = true;
            currentSentence = RandomSentence();
        
            _uiManager.InitializeWindow(Windows.Task);
        
            SetRemainingSentence(currentSentence);
        }

        #endregion
    }
}
