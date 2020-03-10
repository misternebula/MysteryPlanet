using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeAir
    {
        public static void Make(GameObject body)
        {
            GameObject air = new GameObject();
            air.layer = 17;
            air.SetActive(false);
            air.transform.parent = body.transform;

            SphereCollider atmoSC = air.AddComponent<SphereCollider>();
            atmoSC.isTrigger = true;
            atmoSC.radius = 400f;

            SimpleFluidVolume sfv = air.AddComponent<SimpleFluidVolume>();
            sfv.SetValue("_layer", 5);
            sfv.SetValue("_priority", 1);
            sfv.SetValue("_density", 1.2f);
            sfv.SetValue("_fluidType", FluidVolume.Type.AIR);
            sfv.SetValue("_allowShipAutoroll", true);
            sfv.SetValue("_disableOnStart", false);

            VisorRainEffectVolume vref = air.AddComponent<VisorRainEffectVolume>();
            vref.SetValue("_rainDirection", VisorRainEffectVolume.RainDirection.Radial);
            vref.SetValue("_layer", 0);
            vref.SetValue("_priority", 0);

            air.SetActive(true);
        }
    }
}
