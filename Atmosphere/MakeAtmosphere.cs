using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeAtmosphere
    {
        public static void Make(GameObject body, float topCloudScale)
        {
            topCloudScale /= 2;

            GameObject atmo = new GameObject();
            atmo.SetActive(false);
            atmo.transform.parent = body.transform;
            atmo.transform.localScale = new Vector3(topCloudScale + 10, topCloudScale + 10, topCloudScale + 10);

            MeshFilter mf = atmo.AddComponent<MeshFilter>();
            mf.mesh = GameObject.Find("Atmosphere_GD/FogSphere").GetComponent<MeshFilter>().mesh;

            MeshRenderer mr = atmo.AddComponent<MeshRenderer>();
            mr.materials = GameObject.Find("Atmosphere_GD/FogSphere").GetComponent<MeshRenderer>().materials;
            mr.allowOcclusionWhenDynamic = true;

            PlanetaryFogController pfc = atmo.AddComponent<PlanetaryFogController>();
            pfc.fogLookupTexture = GameObject.Find("Atmosphere_GD/FogSphere").GetComponent<PlanetaryFogController>().fogLookupTexture;
            pfc.fogRadius = topCloudScale + 10;
            pfc.fogDensity = 0.3f;
            pfc.fogExponent = 1f;
            pfc.fogColorRampTexture = GameObject.Find("Atmosphere_GD/FogSphere").GetComponent<PlanetaryFogController>().fogColorRampTexture;
            pfc.fogColorRampIntensity = 1f;
            pfc.fogTint = Color.white;

            atmo.SetActive(true);

            MainClass.returnedCount++;
        }
    }
}
