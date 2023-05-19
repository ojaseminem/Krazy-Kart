using System;
using System.Collections;
using Misc;
using UnityEngine;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        public void StartCountDown(float seconds) => StartCoroutine(CountDown(seconds));

        private IEnumerator CountDown(float seconds)
        {
            var time = seconds;
            var text = GameManager.Instance.uiManager.gameUiWindow.countDownText;
            
            while (time > 0)
            {
                var minutesRemaining = Mathf.FloorToInt(time / 60f);
                var secondsRemaining = Mathf.FloorToInt(time % 60f);
                
                text.text = $"{minutesRemaining:00}:{secondsRemaining:00}";
                
                yield return new WaitForSeconds(1f);

                if (Math.Abs(time - 10f) < .5f) 
                    StartCoroutine(SkyboxController.SkyboxCountDown(0f, 1f, true, 10f));
                
                time -= 1f;
            }
            
            //Countdown finished
            text.text = "CountDown Finished";
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
    }
}