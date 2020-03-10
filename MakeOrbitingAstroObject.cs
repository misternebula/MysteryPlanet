using OWML.ModHelper.Events;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeOrbitingAstroObject
    {
        public static void Make(GameObject body)
        {
            Rigidbody RB = body.AddComponent<Rigidbody>();
            RB.mass = 10000;
            RB.drag = 0f;
            RB.angularDrag = 0f;
            RB.useGravity = false;
            RB.isKinematic = true;
            RB.interpolation = RigidbodyInterpolation.None;
            RB.collisionDetectionMode = CollisionDetectionMode.Discrete;

            MainClass.OWRB = body.AddComponent<OWRigidbody>();
            MainClass.OWRB.SetValue("_kinematicSimulation", true);
            MainClass.OWRB.SetValue("_autoGenerateCenterOfMass", true);
            MainClass.OWRB.SetIsTargetable(true);
            MainClass.OWRB.SetValue("_maintainOriginalCenterOfMass", true);

            InitialMotion IM = body.AddComponent<InitialMotion>();
            IM.SetPrimaryBody(Locator.GetSunTransform().GetAttachedOWRigidbody());
            IM.SetValue("_orbitAngle", 0f);
            IM.SetValue("_isGlobalAxis", false);
            IM.SetValue("_initAngularSpeed", 0.02f);
            IM.SetValue("_initLinearSpeed", 0f);
            IM.SetValue("_isGlobalAxis", false);

            MakeFieldDetector.Make(body);

            GravityVolume GV = MakeGravityWell.Make(body);

            AstroObject AO = body.AddComponent<AstroObject>();
            AO.SetValue("_type", AstroObject.Type.Planet);
            AO.SetValue("_name", AstroObject.Name.InvisiblePlanet);
            AO.SetPrimaryBody(Locator.GetAstroObject(AstroObject.Name.Sun));
            AO.SetValue("_gravityVolume", GV);
        }
    }
}
