using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeAmbientLight
    {
        public static void Make(GameObject body)
        {
            GameObject light = new GameObject();
            light.SetActive(false);
            light.transform.parent = body.transform;
            Light l = light.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 700f;
            l.color = new Color32(0, 75, 50, 128);
            l.intensity = 0.8f;
            l.shadows = LightShadows.None;
            l.cookie = GameObject.Find("AmbientLight_GD").GetComponent<Light>().cookie;

            SectorLightsCullGroup cg = light.AddComponent<SectorLightsCullGroup>();
            cg.SetSector(MainClass.SECTOR);

            light.SetActive(true);

            MainClass.returnedCount++;
        }
    }
}
