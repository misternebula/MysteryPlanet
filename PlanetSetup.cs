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
        public static SpawnPoint SPAWN;
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                MoveTo(SPAWN);
            }
        }

        public void MoveTo(SpawnPoint point)
        {
            if (point != null)
            {
                OWRigidbody playerBody = Locator.GetPlayerBody();

                playerBody.WarpToPositionRotation(point.transform.position, point.transform.rotation);
                playerBody.SetVelocity(point.GetPointVelocity());

                point.AddObjectToTriggerVolumes(Locator.GetPlayerDetector().gameObject);
                point.OnSpawnPlayer();
            }
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

                    makeSpawnPoint = true,

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
                    fogDensity = 0.75f,

                    hasOrbit = true
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
            
            Body.MakeGeometry.Make(body, groundScale);

            General.MakeOrbitingAstroObject.Make(body, planet.primaryBody, 0.02f, planet.hasGravity, planet.surfaceAccel, groundScale, planet.hasOrbit);
            General.MakeRFVolume.Make(body);

            if (planet.hasMapMarker)
            {
                General.MakeMapMarker.Make(body);
            }
            
            SECTOR = Body.MakeSector.Make(body, planet.topCloudSize.Value);

            if (planet.hasClouds)
            {
                Atmosphere.MakeClouds.Make(body, planet.topCloudSize.Value, planet.bottomCloudSize.Value, planet.cloudTint.Value);
                Atmosphere.MakeSunOverride.Make(body, planet.topCloudSize.Value, planet.bottomCloudSize.Value, planet.waterSize.Value);
            }

            Atmosphere.MakeAir.Make(body, planet.topCloudSize.Value / 2, planet.hasRain);

            if (planet.hasWater)
            {
                Body.MakeWater.Make(body, planet.waterSize.Value);
            }

            Atmosphere.MakeBaseEffects.Make(body);
            Atmosphere.MakeVolumes.Make(body, groundScale, planet.topCloudSize.Value);
            General.MakeAmbientLight.Make(body);
            Atmosphere.MakeAtmosphere.Make(body, planet.topCloudSize.Value, planet.hasFog, planet.fogDensity, planet.fogTint);
            Invisible.MakeInvisible.Make(body, planet.topCloudSize.Value + 10f);

            if (planet.makeSpawnPoint)
            {
                SPAWN = General.MakeSpawnPoint.Make(body, groundScale);
            }

            Debug.MakeTempSimButton.Make(body, groundScale);

            return body;
        }
    }
}