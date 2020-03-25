using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MysteryPlanet
{
    public class MainClass : ModBehaviour
    {
        GameObject generatedPlanet;

        public static OWRigidbody OWRB;
        public static Sector SECTOR;
        private void Start()
        {
            base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
            IModEvents events = base.ModHelper.Events;
            events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));

            //GlobalMessenger.AddListener("WakeUp", OnWakeUp);

            SceneManager.sceneLoaded += OnSceneLoaded;
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

                PlanetStructure inputStructure = new PlanetStructure
                {
                    name = "invisibleplanet",

                    primaryBody = Locator.GetAstroObject(AstroObject.Name.Sun),
                    aoType = AstroObject.Type.Planet,
                    aoName = AstroObject.Name.InvisiblePlanet,

                    position = new Vector3(0, 0, 30000),

                    hasClouds = true,
                    topCloudSize = 650f,
                    bottomCloudSize = 600f,
                    cloudTint = new Color32(0, 75, 15, 128),

                    hasWater = true,
                    waterSize = 401f,

                    hasRain = true,

                    hasGravity = true,
                    surfaceAccel = 12f,

                    hasMapMarker = true,

                    hasFog = true,
                    fogTint = new Color32(0, 75, 15, 128),
                    fogDensity = 0.75f
                };

                generatedPlanet = GenerateBody(inputStructure);

                if (inputStructure.primaryBody = Locator.GetAstroObject(AstroObject.Name.Sun))
                {
                    generatedPlanet.transform.parent = Locator.GetRootTransform();
                }
                else
                {
                    generatedPlanet.transform.parent = inputStructure.primaryBody.transform;
                }

                generatedPlanet.transform.position = inputStructure.position;
                generatedPlanet.SetActive(true);
            }
        }
        private GameObject GenerateBody(PlanetStructure planet)
        {
            float groundScale = 400f;

            GameObject body;

            body = new GameObject();
            body.name = planet.name;
            body.SetActive(false);
            
            MakeGeometry.Make(body, groundScale);

            MakeOrbitingAstroObject.Make(body, planet.primaryBody, 0.02f, planet.surfaceAccel, groundScale);
            MakeRFVolume.Make(body);

            if (planet.hasMapMarker)
            {
                MakeMapMarker.Make(body);
            }
            
            SECTOR = MakeSector.Make(body, planet.topCloudSize.Value);

            if (planet.hasClouds)
            {
                MakeClouds.Make(body, planet.topCloudSize.Value, planet.bottomCloudSize.Value, planet.cloudTint.Value);
                MakeSunOverride.Make(body, planet.topCloudSize.Value, planet.bottomCloudSize.Value, planet.waterSize.Value);
            }

            MakeAir.Make(body, planet.topCloudSize.Value / 2, planet.hasRain);

            if (planet.hasWater)
            {
                MakeWater.Make(body, planet.waterSize.Value);
            }
            
            MakeBaseEffects.Make(body);
            MakeVolumes.Make(body, groundScale, planet.topCloudSize.Value);
            MakeAmbientLight.Make(body);
            MakeAtmosphere.Make(body, planet.topCloudSize.Value, planet.hasFog, planet.fogDensity, planet.fogTint);
            MakeInvisible.Make(body, planet.topCloudSize.Value + 10f);

            return body;
        }
    }
}