using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeGeometry
    {
        public static void Make(GameObject body, float groundScale)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(groundScale, groundScale, groundScale);
            sphere.transform.parent = body.transform;
        }
    }
}
