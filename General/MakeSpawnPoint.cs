using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet.General
{
    static class MakeSpawnPoint
    {
        public static SpawnPoint Make(GameObject body, float groundSize)
        {
            groundSize /= 2;

            GameObject spawn = new GameObject();
            spawn.transform.parent = body.transform;
            spawn.layer = 8;

            spawn.transform.localPosition = new Vector3(0, groundSize + 10, 0);

            return spawn.AddComponent<SpawnPoint>();
        }
    }
}
