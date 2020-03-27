using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet.Invisible
{
    static class MakeInvisible
    {
        static public void Make(GameObject body, float size)
        {
            GameObject cloak = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cloak.transform.parent = body.transform;
            cloak.transform.localScale = new Vector3(size, size, size);
            cloak.GetComponent<SphereCollider>().enabled = false;

            cloak.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            cloak.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 255);
            cloak.AddComponent<MysteryPlanetCloakSphere>();

            GameObject cloudsTop = new GameObject();
            cloudsTop.SetActive(false);
            cloudsTop.layer = 28;
            cloudsTop.transform.parent = body.transform;
            cloudsTop.transform.localScale = new Vector3((size + 5) / 2, (size + 5) / 2, (size + 5) / 2);

            MeshFilter MF = cloudsTop.AddComponent<MeshFilter>();
            MF.mesh = GameObject.Find("CloudsTopLayer_GD").GetComponent<MeshFilter>().mesh;

            MeshRenderer MR = cloudsTop.AddComponent<MeshRenderer>();
            MR.materials = GameObject.Find("CloudsTopLayer_GD").GetComponent<MeshRenderer>().materials;

            RotateTransform RT = cloudsTop.AddComponent<RotateTransform>();
            RT.SetValue("_localAxis", Vector3.up);
            RT.SetValue("degreesPerSecond", 10);
            RT.SetValue("randomizeRotationRate", false);

            cloudsTop.SetActive(true);
        }
    }
}
