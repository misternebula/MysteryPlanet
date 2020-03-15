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
            sphere.transform.localScale = new Vector3(groundScale / 2, groundScale / 2, groundScale / 2);
            sphere.transform.parent = body.transform;
            sphere.GetComponent<MeshFilter>().mesh = GameObject.Find("CloudsTopLayer_GD").GetComponent<MeshFilter>().mesh;
            sphere.GetComponent<SphereCollider>().radius = 1f;
        }
    }
}
