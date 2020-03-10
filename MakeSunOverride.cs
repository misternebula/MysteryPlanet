using OWML.ModHelper.Events;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeSunOverride
    {
        public static void Make(GameObject body)
        {
            GameObject sunov = new GameObject();
            sunov.SetActive(false);
            sunov.transform.parent = body.transform;

            GiantsDeepSunOverrideVolume vol = sunov.AddComponent<GiantsDeepSunOverrideVolume>();
            vol.SetValue("_sector", MainClass.SECTOR);
            vol.SetValue("_cloudsOuterRadius", 400f);
            vol.SetValue("_cloudsInnerRadius", 380f);
            vol.SetValue("_waterOuterRadius", 502.5f);
            vol.SetValue("_waterInnerRadius", 402.5f);

            sunov.SetActive(true);
        }
    }
}
