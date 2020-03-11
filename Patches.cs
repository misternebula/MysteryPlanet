using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    public static class Patches
    {
        static bool VisorEffectVolumeEnterPre()
        {
            Debug.LogError("ENTER EFFECT VOLUME");
            return true;
        }

        static bool VisorEffectVolumeExitPre()
        {
            Debug.LogError("EXIT EFFECT VOLUME");
            return true;
        }
    }
}
