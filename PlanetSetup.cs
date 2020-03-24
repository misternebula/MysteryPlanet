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
        public static Mesh planetmesh;

        public static int componentCount;
        public static int returnedCount;

        private void Start()
        {
            base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
            IModEvents events = base.ModHelper.Events;
            events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));

            //GlobalMessenger.AddListener("WakeUp", OnWakeUp);

            SceneManager.sceneLoaded += OnSceneLoaded;

            assetBundle = ModHelper.Assets.LoadBundle("fogsphere");

            var planetmesh = ModHelper.Assets.LoadMesh("body.asset");
            planetmesh.OnLoaded += OnPlanetMeshLoaded;
        }

        void OnPlanetMeshLoaded(MeshFilter mesh)
        {
            planetmesh = mesh.mesh;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneIntegrator.isDLCEnabled = true;
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            bool flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
            if (flag)
            {
                foreach (var item in GameObject.FindObjectsOfType<ProbeCamera>())
                {
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
            float groundScale = 400f;
            float waterScale = 401f;
            float topCloudScale = 650f;
            float bottomCloudScale = 600f;

            componentCount = 16; // How many components are in the project.

            GameObject body;

            body = new GameObject();
            body.name = "invisibleplanet_body";
            body.SetActive(false);
            
            MakeGeometry.Make(body, groundScale, assetBundle, planetmesh);

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
            MakeAtmosphere.Make(body, topCloudScale, 0.75f, new Color32(0, 75, 15, 128));
            MakeInvisible.Make(body, topCloudScale + 10f);

            if (returnedCount != componentCount)
            {
                base.ModHelper.Console.WriteLine("ERROR! Expected [" + componentCount + "] components but only [" + returnedCount + "] were activated.");
            }

            return body;
        }
    }
}