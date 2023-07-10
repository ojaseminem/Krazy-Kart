using System;
using UnityEngine;

namespace UI.Parent
{
    public class Popups : MonoBehaviour
    {
        protected Action AppearAction;
        protected Action DisappearAction;
        
        protected virtual void OnOpen()
        {
            GetComponent<Animator>().Play("OnAppear");
        }

        protected virtual void OnAppear()
        {
            // Called from anim event
            print($"Popups Appeared :: {gameObject.name}");
            AppearAction?.Invoke();
        }

        protected virtual void OnDisappear()
        {
            // Called from anim event
            print($"Popups Disappeared :: {gameObject.name}");
            gameObject.SetActive(false);
        }
        
        protected virtual void OnClose()
        {
            DisappearAction?.Invoke();
            GetComponent<Animator>().Play("OnDisappear");
        }
    }
}
