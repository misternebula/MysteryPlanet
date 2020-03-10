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
        GameObject _invisiblePlanet;

        public static OWRigidbody OWRB;
        public static Sector SECTOR;

        private void Start()
        {
            base.ModHelper.Console.WriteLine("[InvisiblePlanet] :");

            base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
            IModEvents events = base.ModHelper.Events;
            events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));

            //GlobalMessenger.AddListener("WakeUp", OnWakeUp);

            SceneManager.sceneLoaded += OnSceneLoaded;
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
            GameObject body;

            body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.transform.localScale = new Vector3(500, 500, 500);
            body.name = "invisibleplanet_body";
            body.SetActive(false);

            MakeOrbitingAstroObject.Make(body, 0.02f, 12f);
            MakeRFVolume.Make(body);
            MakeMapMarker.Make(body);
            SECTOR = MakeSector.Make(body);
            MakeClouds.Make(body);
            MakeAir.Make(body);
            MakeWater.Make(body);
            MakeSunOverride.Make(body);

            base.ModHelper.Console.WriteLine(": All components finalized. Returning object...");

            return body;
        }
    }
}