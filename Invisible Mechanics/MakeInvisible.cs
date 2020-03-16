using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeInvisible
    {
        static public void Make(GameObject body)
        {
            GameObject cloak = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cloak.transform.parent = body.transform;
            cloak.transform.localScale = new Vector3(700, 700, 700);
            cloak.GetComponent<SphereCollider>().enabled = false;

            cloak.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            cloak.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 255);
            cloak.AddComponent<MysteryPlanetCloakSphere>();

            MainClass.returnedCount++;
        }
    }
}
