using UnityEngine;
using Utils;

namespace Managers
{
    public class ControlsManager : MonoBehaviour
    {
        public void SetLeftRight(int value) => GameManager.Instance.playerManager.player.SetLeftRight(value);
        public void SetVerticalInput(float value) => CustomInput.VerticalInput = value;
    }
}