﻿using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeBaseEffects
    {
        public static void Make(GameObject body)
        {
            GameObject main = new GameObject();
            main.SetActive(false);
            main.transform.parent = body.transform;

            SectorCullGroup maincull = main.AddComponent<SectorCullGroup>();
            maincull.SetValue("_sector", MainClass.SECTOR);
            maincull.SetValue("_particleSystemSuspendMode", CullGroup.ParticleSystemSuspendMode.Stop);
            maincull.SetValue("_occlusionCulling", false);
            maincull.SetValue("_dynamicCullingBounds", false);
            maincull.SetValue("_waitForStreaming", false);

            GameObject rain = new GameObject();
            rain.SetActive(false);
            rain.transform.parent = main.transform;

            ParticleSystem ps = GameObject.Instantiate(GameObject.Find("Effects_GD_Rain").GetComponent<ParticleSystem>());

            VectionFieldEmitter vfe = rain.AddComponent<VectionFieldEmitter>();
            vfe.fieldRadius = 20f;
            vfe.particleCount = 10;
            vfe.emitOnLeadingEdge = false;
            vfe.emitDirection = VectionFieldEmitter.EmitDirection.Radial;
            vfe.reverseDir = true;
            vfe.SetValue("_affectingForces", new ForceVolume[0]);
            vfe.SetValue("_applyForcePerParticle", false);

            PlanetaryVectionController pvc = rain.AddComponent<PlanetaryVectionController>();
            pvc.SetValue("_followTarget", pvc.GetType().GetNestedType("FollowTarget", BindingFlags.NonPublic).GetField("Player").GetValue(pvc));
            pvc.SetValue("_activeInSector", MainClass.SECTOR);

            rain.GetComponent<Renderer>().material = GameObject.Find("Effects_GD_Rain").GetComponent<Renderer>().material;

            main.SetActive(true);

            MainClass.returnedCount++;
        }
    }
}
