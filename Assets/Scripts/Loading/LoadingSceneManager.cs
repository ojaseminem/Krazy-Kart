using UnityEngine;

namespace Loading
{
    public class LoadingSceneManager : MonoBehaviour
    {
        [SerializeField] private Transform splashScreenParent;
        
        private static GameObject SplashScreen => Resources.Load<GameObject>("LoadingScenePrefabs/SplashScreen");

        private void Start()
        {
            Instantiate(SplashScreen, splashScreenParent);
        }
    }
}