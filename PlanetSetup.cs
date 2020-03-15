﻿using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MysteryPlanet
{
    public class MainClass : ModBehaviour
    {
        GameObject _invisiblePlanet;

        public static OWRigidbody OWRB;
        public static Sector SECTOR;

        public static AssetBundle assetBundle;

        private void Start()
        {
            base.ModHelper.Console.WriteLine("[InvisiblePlanet] :");

            base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
            IModEvents events = base.ModHelper.Events;
            events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));

            //GlobalMessenger.AddListener("WakeUp", OnWakeUp);

            SceneManager.sceneLoaded += OnSceneLoaded;

            assetBundle = ModHelper.Assets.LoadBundle("fogsphere");
            
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneIntegrator.isDLCEnabled = true;

            base.ModHelper.Console.WriteLine("Setup finished!");
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            bool flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
            if (flag)
            {
                base.ModHelper.Console.WriteLine(": Started!");

                foreach (var item in GameObject.FindObjectsOfType<ProbeCamera>())
                {
                    base.ModHelper.Console.WriteLine(item.gameObject.name);
                    RenderTexture temp = new RenderTexture(512, 512, 16);
                    temp.Create();
                    item.SetValue("_longExposureSnapshotTexture", temp);
                    temp.SetValue("_origCullingMask", item.GetValue<int>("_origCullingMask") | OWLayerMask.probeLongExposureMask.value);
                }

                _invisiblePlanet = GenerateBody();

                _invisiblePlanet.transform.parent = Locator.GetRootTransform();
                _invisiblePlanet.transform.position = new Vector3(0, 0, 30000);
                _invisiblePlanet.SetActive(true);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                _invisiblePlanet.layer = 28;

                foreach (var item in GameObject.FindObjectsOfType<ProbeCamera>())
                {
                    RenderTexture temp = new RenderTexture(512, 512, 16);
                    temp.Create();
                    item.SetValue("_longExposureSnapshotTexture", temp);
                    temp.SetValue("_origCullingMask", item.GetValue<int>("_origCullingMask") | OWLayerMask.probeLongExposureMask.value);
                }
            }
        }

        private GameObject GenerateBody()
        {
            float groundScale = 500f;
            float waterScale = 501f;
            float topCloudScale = 650f;
            float bottomCloudScale = 600f;

            GameObject body;

            body = new GameObject();
            body.name = "invisibleplanet_body";
            body.SetActive(false);

            MakeGeometry.Make(body, groundScale);

            MakeOrbitingAstroObject.Make(body, 0.02f, 12f, groundScale);
            MakeRFVolume.Make(body);
            MakeMapMarker.Make(body);
            SECTOR = MakeSector.Make(body, topCloudScale);
            MakeClouds.Make(body, topCloudScale, bottomCloudScale);
            MakeAir.Make(body, topCloudScale/2);
            MakeWater.Make(body, waterScale);
            MakeSunOverride.Make(body, topCloudScale, bottomCloudScale, waterScale);
            MakeBaseEffects.Make(body);
            MakeVolumes.Make(body, groundScale, topCloudScale);
            MakeAmbientLight.Make(body);
            MakeAtmosphere.Make(body);

            base.ModHelper.Console.WriteLine(": All components finalized. Returning object...");

            return body;
        }
    }
}