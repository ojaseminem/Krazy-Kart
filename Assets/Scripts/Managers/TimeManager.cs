using System.Collections;
using UnityEngine;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        public void StartCountDown(float seconds)
        {
            StartCoroutine(CountDown(seconds));
        }

        private IEnumerator CountDown(float seconds)
        {
            var time = seconds;
            var text = GameManager.instance.uiManager.gameUiWindow.countDownText;
            
            while (time > 0)
            {
                var minutesRemaining = Mathf.FloorToInt(time / 60f);
                var secondsRemaining = Mathf.FloorToInt(time % 60f);
                
                text.text = $"{minutesRemaining:00}:{secondsRemaining:00}";
                
                yield return new WaitForSeconds(1f);

                time -= 1f;
            }
            
            //Countdown finished
            text.text = "CountDown Finished";
        }
    }
}