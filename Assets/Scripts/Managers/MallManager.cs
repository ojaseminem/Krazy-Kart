using System;
using System.Collections.Generic;
using Data;
using Mall;
using UnityEngine;

namespace Managers
{
    public class MallManager : MonoBehaviour
    {
        #region Singleton

        public static MallManager Instance;
        private void Awake() => Instance = this;

        #endregion
        
        // Keep Mall Manager lightweight but parent control
        // Control Mall State
        // - Is the Mall building / constructing
        // - Is the Player past Exit point and in-transition? Helps to async spawn the next floor in-time
        // - Controls Seed for Mart controlled-procedural asset placement
        //
        // Initialize Marts
        // Load Unload each floor / mart
        public LevelData LevelData;

        private void Start()
        {
        }

        private void ChangeMallState(MallState mallState)
        {
            switch (mallState)
            {
                case MallState.Empty:
                    EmptyMall();
                    break;
                case MallState.Constructing:
                    ConstructMall();
                    break;
                case MallState.Transitioning:
                    TransitionMall();
                    break;
                case MallState.ReachedManagerFloor:
                    ReachedManagerFloor();
                    break;
                case MallState.Deconstructing:
                    DeconstructMall();
                    break;
            }
        }

        [ContextMenu("Clear Mall")]
        private void EmptyMall()
        {
            // TODO: Foreach Mart Handlers ensure deconstruct and clear all references / memory

            var floorControllers = gameObject.GetComponentsInChildren<FloorController>();
            foreach (var controller in floorControllers)
            {
                Debug.Log(controller.gameObject.name);
                Destroy(controller.gameObject);
            }
        }

        [ContextMenu("Construct Mall")]
        private void ConstructMall()
        {
            foreach (var controller in LevelData.floorControllers)
            {
                Instantiate(controller, gameObject.transform);
                Debug.Log("Instantiated " + controller.gameObject.name + " inside " + gameObject.transform);
            }
            Debug.Log("Constructed Mall.");
        }

        private void TransitionMall()
        {
            
        }

        private void ReachedManagerFloor()
        {
            
        }

        private void DeconstructMall()
        {
            
        }
    }
}