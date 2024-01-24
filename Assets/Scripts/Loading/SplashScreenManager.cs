using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loading
{
    public class SplashScreenManager : MonoBehaviour
    {
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Additive);
        }
    }
}