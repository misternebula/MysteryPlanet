using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeVolumes
    {
        public static void Make(GameObject body)
        {
            GameObject volumes = new GameObject();
            volumes.SetActive(false);
            volumes.transform.parent = body.transform;

            GameObject ruleset = new GameObject();
            ruleset.transform.parent = volumes.transform;

            SphereShape ss = ruleset.AddComponent<SphereShape>();
            ss.SetCollisionMode(Shape.CollisionMode.Volume);
            ss.SetLayer(Shape.Layer.Sector);
            ss.layerMask = -1;
            ss.pointChecksOnly = true;
            ss.radius = 600f;

            OWTriggerVolume trigvol = ruleset.AddComponent<OWTriggerVolume>();

            PlanetoidRuleset prule = ruleset.AddComponent<PlanetoidRuleset>();
            prule.SetValue("_altitudeFloor", 500f);
            prule.SetValue("_altitudeCeiling", 600f);

            EffectRuleset er = ruleset.AddComponent<EffectRuleset>();
            er.SetValue("_type", EffectRuleset.BubbleType.Underwater);
            er.SetValue("_material", GameObject.Find("RulesetVolumes_GD").GetComponent<RulesetVolume>().GetValue<Material>("_material"));
            er.SetValue("_cloudMaterial", GameObject.Find("RulesetVolumes_GD").GetComponent<RulesetVolume>().GetValue<Material>("_cloudMaterial"));

            volumes.SetActive(true);
        }
    }
}
