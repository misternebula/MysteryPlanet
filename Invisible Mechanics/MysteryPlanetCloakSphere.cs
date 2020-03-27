using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet.Invisible
{
    class MysteryPlanetCloakSphere : MonoBehaviour
    {
        MeshRenderer _renderer;
        void Start()
        {
            _renderer = gameObject.GetComponent<MeshRenderer>();
        }

        void Update()
        {
            float solidDistance = 500f;
            float dist = Vector3.Distance(Locator.GetPlayerTransform().position, gameObject.transform.position);

            if (dist < solidDistance) dist = solidDistance;

            if (1 - (solidDistance / dist) > 0.96f) dist = 100000000f;

            _renderer.material.SetAlpha(1 - (solidDistance / dist));
        }
    }
}
