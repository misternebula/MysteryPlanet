using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet.Simulation
{
    static class MakeSimulationBody
    {
        public static SpawnPoint simSpawn;
        public static void Make()
        {
            GameObject simRoot = new GameObject();
            simSpawn = General.MakeSpawnPoint.Make(simRoot, -20f);
        }
    }
}
