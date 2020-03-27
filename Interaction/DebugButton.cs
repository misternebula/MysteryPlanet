using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet.Interaction
{
    class DebugButton : MonoBehaviour
    {
        public InteractReceiver receiver;
        BoxCollider _collider;

        void Start()
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.isTrigger = true;

            receiver = gameObject.AddComponent<InteractReceiver>();
            receiver.SetInteractRange(2);
            receiver.SetPromptText((UITextType)UI.AddToUITable.Add("bazinga"));
            receiver.OnPressInteract += OnPress;
            receiver.OnReleaseInteract += OnRelease;
        }

        void OnPress()
        {
            gameObject.GetComponent<Simulation.SimulationController>().EnterSimulation();
        }

        void OnRelease()
        {
            receiver.ResetInteraction();
        }

        void OnDisable()
        {
            if (_collider != null)
            {
                _collider.enabled = false;
            }
        }

        void OnEnable()
        {
            if (_collider != null)
            {
                _collider.enabled = true;
            }
        }
    }
}
