using OWML.ModHelper.Events;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeSunOverride
    {
        public static void Make(GameObject body, float topCloudScale, float bottomCloudScale, float waterSize)
        {
            GameObject sunov = new GameObject();
            sunov.SetActive(false);
            sunov.transform.parent = body.transform;

            GiantsDeepSunOverrideVolume vol = sunov.AddComponent<GiantsDeepSunOverrideVolume>();
            vol.SetValue("_sector", MainClass.SECTOR);
            vol.SetValue("_cloudsOuterRadius", topCloudScale);
            vol.SetValue("_cloudsInnerRadius", bottomCloudScale);
            vol.SetValue("_waterOuterRadius", waterSize);
            vol.SetValue("_waterInnerRadius", 402.5f);

            sunov.SetActive(true);
        }
    }
}
