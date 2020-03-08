using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MysteryPlanet
{
    public class MainClass : ModBehaviour
    {
        GameObject _invisiblePlanet;
        public bool _doMeshLater = false;
        public bool _doMeshOnWakeUp = false;
        public static GameObject sectorGO;

        public MeshFilter MFlater;
        public MeshRenderer MRlater;
        public TessellatedSphereRenderer cloudRender;

        private void Start()
        {
            base.ModHelper.Console.WriteLine("[InvisiblePlanet] :");

            base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
            IModEvents events = base.ModHelper.Events;
            events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));

            GlobalMessenger.AddListener("WakeUp", OnWakeUp);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneIntegrator.isDLCEnabled = true;

            base.ModHelper.Console.WriteLine("Setup finished!");
        }

        void OnWakeUp()
        {
            if (_doMeshOnWakeUp)
            {
                base.ModHelper.Console.WriteLine(": Trying mesh again...");
                MFlater.mesh = GameObject.Find("CloudsTopLayer_QM").GetComponent<MeshFilter>().mesh;
                base.ModHelper.Console.WriteLine(": Trying materials again...");
                MRlater.materials = GameObject.Find("CloudsTopLayer_QM").GetComponent<MeshRenderer>().materials;
                cloudRender.tessellationMeshGroup = GameObject.Find("CloudsBottomLayer_QM").GetComponent<TessellatedSphereRenderer>().tessellationMeshGroup;
                cloudRender.sharedMaterials = GameObject.Find("CloudsBottomLayer_QM").GetComponent<TessellatedSphereRenderer>().sharedMaterials;
                base.ModHelper.Console.WriteLine(": Done!");
            }
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

                Sector sector = _invisiblePlanet.GetComponentInChildren<Sector>();

                /*
                if (sector.GetValue<GameObject>("_triggerRoot") != null)
                {
                    base.ModHelper.Console.WriteLine("TriggerRoot is not null!");
                    base.ModHelper.Console.WriteLine(sector.GetValue<GameObject>("_triggerRoot").name);
                }
                else
                {
                    base.ModHelper.Console.WriteLine("TriggerRoot is null!");
                }

                if (sector.GetValue<ProximityTrigger>("_proximityTrigger") != null)
                {
                    base.ModHelper.Console.WriteLine("ProxTrigger is not null!");
                }
                else
                {
                    base.ModHelper.Console.WriteLine("ProxTrigger is null!");
                }

                if (sector.GetValue<OWTriggerVolume>("_owTriggerVolume") != null)
                {
                    base.ModHelper.Console.WriteLine("OWTrigVol is not null!");
                }
                else
                {
                    base.ModHelper.Console.WriteLine("OWTrigVol is null!");
                }

                if (sector.GetValue<VolumeExcluder>("_volumeExcluder") != null)
                {
                    base.ModHelper.Console.WriteLine("VolEx is not null!");
                }
                else
                {
                    base.ModHelper.Console.WriteLine("VolEx is null!");
                }

                if (sector.GetValue<OWRigidbody>("_attachedOWRigidbody") != null)
                {
                    base.ModHelper.Console.WriteLine("OWRB is not null!");
                }
                else
                {
                    base.ModHelper.Console.WriteLine("OWRB is null!");
                }
                */
                base.ModHelper.Console.WriteLine(sectorGO.GetAttachedOWRigidbody(false).name);
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
            base.ModHelper.Console.WriteLine(": Beginning body generation...");
            GameObject body;

            body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.transform.localScale = new Vector3(500, 500, 500);

            body.name = "invisibleplanet_body";
            body.SetActive(false);

            Rigidbody RB = body.AddComponent<Rigidbody>();
            RB.mass = 10000;
            RB.drag = 0f;
            RB.angularDrag = 0f;
            RB.useGravity = false;
            RB.isKinematic = true;
            RB.interpolation = RigidbodyInterpolation.None;
            RB.collisionDetectionMode = CollisionDetectionMode.Discrete;
            base.ModHelper.Console.WriteLine(": Rigidbody done.");

            OWRigidbody OWRB = body.AddComponent<OWRigidbody>();
            OWRB.SetValue("_kinematicSimulation", true);
            OWRB.SetValue("_autoGenerateCenterOfMass", true);
            OWRB.SetIsTargetable(true);
            OWRB.SetValue("_maintainOriginalCenterOfMass", true);
            base.ModHelper.Console.WriteLine(": OWRigidbody done.");

            InitialMotion IM = body.AddComponent<InitialMotion>();
            IM.SetPrimaryBody(Locator.GetSunTransform().GetAttachedOWRigidbody());
            IM.SetValue("_orbitAngle", 0f);
            IM.SetValue("_isGlobalAxis", false);
            IM.SetValue("_initAngularSpeed", 0.02f);
            IM.SetValue("_initLinearSpeed", 0f);
            IM.SetValue("_isGlobalAxis", false);
            base.ModHelper.Console.WriteLine(": InitialMotion done.");

            GameObject FieldDetector = new GameObject();
            FieldDetector.SetActive(false);
            FieldDetector.name = "FieldDetector";
            FieldDetector.transform.parent = body.transform;
            FieldDetector.layer = 20;
            base.ModHelper.Console.WriteLine(": FieldDetector :");

            ConstantForceDetector CFD = FieldDetector.AddComponent<ConstantForceDetector>();
            ForceVolume[] temp = new ForceVolume[1];
            temp[0] = Locator.GetAstroObject(AstroObject.Name.Sun).GetGravityVolume();
            CFD.SetValue("_detectableFields", temp);
            CFD.SetValue("_inheritElement0", false);
            FieldDetector.SetActive(true);
            base.ModHelper.Console.WriteLine(":     ConstantForceDetector done.");

            GameObject GravityWell = new GameObject();
            GravityWell.transform.parent = body.transform;
            GravityWell.name = "GravityWell";
            GravityWell.layer = 17;
            GravityWell.SetActive(false);
            base.ModHelper.Console.WriteLine(": GravityWell :");

            GravityVolume GV = GravityWell.AddComponent<GravityVolume>();
            GV.SetValue("_cutoffAcceleration", 0.1f);
            GV.SetValue("_falloffType", GV.GetType().GetNestedType("FalloffType", BindingFlags.NonPublic).GetField("linear").GetValue(GV));
            GV.SetValue("_alignmentRadius", 600f);
            GV.SetValue("_upperSurfaceRadius", 500f);
            GV.SetValue("_lowerSurfaceRadius", 500f);
            GV.SetValue("_layer", 3);
            GV.SetValue("_priority", 0);
            GV.SetValue("_alignmentPriority", 0);
            GV.SetValue("_surfaceAcceleration", 12f);
            GV.SetValue("_inheritable", false);
            GV.SetValue("_isPlanetGravityVolume", true);
            GV.SetValue("_cutoffRadius", 55f);
            base.ModHelper.Console.WriteLine(":     GravityVolume done.");

            SphereCollider GV_SC = GravityWell.AddComponent<SphereCollider>();
            GV_SC.isTrigger = true;
            GV_SC.radius = 4000;
            base.ModHelper.Console.WriteLine(":     SphereCollider done.");

            OWCollider GV_OWC = GravityWell.AddComponent<OWCollider>();
            GV_OWC.SetLODActivationMask(DynamicOccupant.Player);
            GravityWell.SetActive(true);
            base.ModHelper.Console.WriteLine(":     OWCollider done.");

            AstroObject AO = body.AddComponent<AstroObject>();
            AO.SetValue("_type", AstroObject.Type.Planet);
            AO.SetValue("_name", AstroObject.Name.InvisiblePlanet);
            AO.SetPrimaryBody(Locator.GetAstroObject(AstroObject.Name.Sun));
            AO.SetValue("_gravityVolume", GV);
            base.ModHelper.Console.WriteLine(": AstroObject done.");

            GameObject RFVolume = new GameObject();
            RFVolume.name = "RFVolume";
            RFVolume.transform.parent = body.transform;
            RFVolume.layer = 19;
            RFVolume.SetActive(false);
            base.ModHelper.Console.WriteLine(": RFVolume :");

            SphereCollider RF_SC = RFVolume.AddComponent<SphereCollider>();
            RF_SC.isTrigger = true;
            RF_SC.radius = 600f;
            base.ModHelper.Console.WriteLine(":     SphereCollider done.");

            ReferenceFrameVolume RF_RFV = RFVolume.AddComponent<ReferenceFrameVolume>();
            ReferenceFrame test = new ReferenceFrame(OWRB);
            test.SetValue("_minSuitTargetDistance", 300);
            test.SetValue("_maxTargetDistance", 0);
            test.SetValue("_autopilotArrivalDistance", 1000);
            test.SetValue("_autoAlignmentDistance", 1000);
            test.SetValue("_hideLandingModePrompt", false);
            test.SetValue("_matchAngularVelocity", true);
            test.SetValue("_minMatchAngularVelocityDistance", 70);
            test.SetValue("_maxMatchAngularVelocityDistance", 400);
            test.SetValue("_bracketsRadius", 300);
            RF_RFV.SetValue("_referenceFrame", test);
            RF_RFV.SetValue("_minColliderRadius", 300);
            RF_RFV.SetValue("_maxColliderRadius", 2000);
            RF_RFV.SetValue("_isPrimaryVolume", true);
            RF_RFV.SetValue("_isCloseRangeVolume", false);
            RFVolume.SetActive(true);
            base.ModHelper.Console.WriteLine(":     ReferenceFrameVolume done.");

            var MM = body.AddComponent<MapMarker>();
            MM.SetValue("_labelID", UITextType.YouAreDeadMessage);
            MM.SetValue("_markerType", MM.GetType().GetNestedType("MarkerType", BindingFlags.NonPublic).GetField("Planet").GetValue(MM));
            base.ModHelper.Console.WriteLine(": MapMarker done.");

            base.ModHelper.Console.WriteLine(": Beginning sector generation...");

            GameObject sectorBase = new GameObject();
            sectorBase.SetActive(false);
            sectorBase.transform.parent = body.transform;
            sectorGO = sectorBase;
            base.ModHelper.Console.WriteLine(": SECTOR :");

            SphereShape sphereshape = sectorBase.AddComponent<SphereShape>();
            sphereshape.SetCollisionMode(Shape.CollisionMode.Volume);
            sphereshape.SetLayer(Shape.Layer.Sector);
            sphereshape.layerMask = -1;
            sphereshape.pointChecksOnly = true;
            sphereshape.radius = 400f;
            sphereshape.center = Vector3.zero;
            base.ModHelper.Console.WriteLine(":     SphereShape done.");

            OWTriggerVolume trigVol = sectorBase.AddComponent<OWTriggerVolume>();
            base.ModHelper.Console.WriteLine(":     OWTriggerVolume done.");

            Sector sector = sectorBase.AddComponent<Sector>();
            sector.SetValue("_name", Sector.Name.InvisiblePlanet);
            sector.SetValue("__attachedOWRigidbody", OWRB);
            sector.SetValue("_subsectors", new List<Sector>());

            sectorBase.SetActive(true);
            base.ModHelper.Console.WriteLine(":     Sector done.");

            base.ModHelper.Console.WriteLine(": Beginning cloud generation...");

            GameObject cloudsMain = new GameObject();
            cloudsMain.SetActive(false);
            cloudsMain.transform.parent = body.transform;
            base.ModHelper.Console.WriteLine(": Clouds Main Body done.");

            GameObject cloudsTop = new GameObject();
            cloudsTop.SetActive(false);
            cloudsTop.transform.parent = cloudsMain.transform;
            cloudsTop.transform.localScale = new Vector3(400, 400, 400);
            base.ModHelper.Console.WriteLine(": TOP LAYER :");

            MeshFilter MF = cloudsTop.AddComponent<MeshFilter>();
            try
            {
                MF.mesh = GameObject.Find("CloudsTopLayer_GD").GetComponent<MeshFilter>().mesh;
            }
            catch
            {
                base.ModHelper.Console.WriteLine(":     Error in setting mesh! Queuing for later...");
                _doMeshLater = true;
            }

            base.ModHelper.Console.WriteLine(":     MeshFilter done.");

            MeshRenderer MR = cloudsTop.AddComponent<MeshRenderer>();
            try
            {
                MR.materials = GameObject.Find("CloudsTopLayer_GD").GetComponent<MeshRenderer>().materials;
            }
            catch
            {
                base.ModHelper.Console.WriteLine(":     Error in setting materials! Queuing for later...");
                _doMeshLater = true;
            }
            base.ModHelper.Console.WriteLine(":     MeshRenderer done.");

            RotateTransform RT = cloudsTop.AddComponent<RotateTransform>();
            RT.SetValue("_localAxis", Vector3.up);
            RT.SetValue("degreesPerSecond", 10);
            RT.SetValue("randomizeRotationRate", false);
            base.ModHelper.Console.WriteLine(":     RotateTransform done.");

            SectorCullGroup scg = cloudsTop.AddComponent<SectorCullGroup>();
            scg.SetValue("_sector", sector);
            scg.SetValue("_occlusionCulling", true);
            scg.SetValue("_dynamicCullingBounds", false);
            scg.SetValue("_particleSystemSuspendMode", CullGroup.ParticleSystemSuspendMode.Pause);
            scg.SetValue("_waitForStreaming", false);

            GameObject cloudsBottom = new GameObject();
            cloudsBottom.SetActive(false);
            cloudsBottom.transform.parent = cloudsMain.transform;
            cloudsBottom.transform.localScale = new Vector3(380, 380, 380);
            base.ModHelper.Console.WriteLine(": BOTTOM LAYER :");

            TessellatedSphereRenderer TSR = cloudsBottom.AddComponent<TessellatedSphereRenderer>();
            try
            {
                TSR.tessellationMeshGroup = GameObject.Find("CloudsBottomLayer_GD").GetComponent<TessellatedSphereRenderer>().tessellationMeshGroup;
                TSR.sharedMaterials = GameObject.Find("CloudsBottomLayer_GD").GetComponent<TessellatedSphereRenderer>().sharedMaterials;
            }
            catch
            {
                base.ModHelper.Console.WriteLine(":     Error in setting value! Queuing for later...");
                _doMeshLater = true;
            }
            TSR.maxLOD = 6;
            TSR.LODBias = 0;
            TSR.LODRadius = 1f;
            cloudRender = TSR;
            base.ModHelper.Console.WriteLine(":     TessellatedSphereRenderer done.");

            TessSphereSectorToggle TSST = cloudsBottom.AddComponent<TessSphereSectorToggle>();
            TSST.SetValue("_sector", sector);
            base.ModHelper.Console.WriteLine(":     TessSphereSectorToggle done.");

            
            GameObject cloudsFluid = new GameObject();
            cloudsFluid.layer = 17;
            cloudsFluid.SetActive(false);
            cloudsFluid.transform.parent = cloudsMain.transform;
            base.ModHelper.Console.WriteLine(": CLOUD FLUID LAYER :");

            SphereCollider cloudSC = cloudsFluid.AddComponent<SphereCollider>();
            cloudSC.isTrigger = true;
            cloudSC.radius = 400f;
            base.ModHelper.Console.WriteLine(":     SphereCollider done.");

            OWShellCollider cloudShell = cloudsFluid.AddComponent<OWShellCollider>();
            cloudShell.SetValue("_innerRadius", 380f);
            base.ModHelper.Console.WriteLine(":     OWShellCollider done.");

            CloudLayerFluidVolume cloudLayer = cloudsFluid.AddComponent<CloudLayerFluidVolume>();
            cloudLayer.SetValue("_layer", 5);
            cloudLayer.SetValue("_priority", 1);
            cloudLayer.SetValue("_density", 1.2f);
            cloudLayer.SetValue("_fluidType", FluidVolume.Type.CLOUD);
            cloudLayer.SetValue("_allowShipAutoroll", true);
            cloudLayer.SetValue("_disableOnStart", false);
            base.ModHelper.Console.WriteLine(":     CloudLayerFluidVolume done.");

            GameObject air = new GameObject();
            air.layer = 17;
            air.SetActive(false);
            air.transform.parent = cloudsMain.transform;
            base.ModHelper.Console.WriteLine(": AIR :");

            SphereCollider atmoSC = air.AddComponent<SphereCollider>();
            atmoSC.isTrigger = true;
            atmoSC.radius = 400f;
            base.ModHelper.Console.WriteLine(":     SphereCollider done.");

            SimpleFluidVolume sfv = air.AddComponent<SimpleFluidVolume>();
            sfv.SetValue("_layer", 5);
            sfv.SetValue("_priority", 1);
            sfv.SetValue("_density", 1.2f);
            sfv.SetValue("_fluidType", FluidVolume.Type.AIR);
            sfv.SetValue("_allowShipAutoroll", true);
            sfv.SetValue("_disableOnStart", false);
            base.ModHelper.Console.WriteLine(":     SimpleFluidVolume done.");

            VisorRainEffectVolume vref = air.AddComponent<VisorRainEffectVolume>();
            vref.SetValue("_rainDirection", VisorRainEffectVolume.RainDirection.Radial);
            vref.SetValue("_layer", 0);
            vref.SetValue("_priority", 0);
            base.ModHelper.Console.WriteLine(":     VisorRainEffectVolume done.");

            air.SetActive(true);
            cloudsTop.SetActive(true);
            cloudsBottom.SetActive(true);
            cloudsMain.SetActive(true);

            if (_doMeshLater)
            {
                try
                {
                    base.ModHelper.Console.WriteLine(": Trying mesh again...");
                    MF.mesh = GameObject.Find("CloudsTopLayer_QM").GetComponent<MeshFilter>().mesh;
                    base.ModHelper.Console.WriteLine(": Trying materials again...");
                    MR.materials = GameObject.Find("CloudsTopLayer_QM").GetComponent<MeshRenderer>().materials;
                    TSR.tessellationMeshGroup = GameObject.Find("CloudsBottomLayer_QM").GetComponent<TessellatedSphereRenderer>().tessellationMeshGroup;
                    TSR.sharedMaterials = GameObject.Find("CloudsBottomLayer_QM").GetComponent<TessellatedSphereRenderer>().sharedMaterials;
                    base.ModHelper.Console.WriteLine(": Done!");

                }
                catch
                {
                    base.ModHelper.Console.WriteLine(": Error!");
                    base.ModHelper.Console.WriteLine(": Queuing for WakeUp...");
                    _doMeshOnWakeUp = true;
                    _doMeshLater = false;
                    MFlater = MF;
                    MRlater = MR;
                }
            }

            base.ModHelper.Console.WriteLine(": Beginning water generation...");

            GameObject waterBase = new GameObject();
            waterBase.SetActive(false);
            waterBase.layer = 15;
            waterBase.transform.parent = body.transform;
            waterBase.transform.localScale = new Vector3(1.005f, 1.005f, 1.005f);
            waterBase.DestroyAllComponents<SphereCollider>();
            base.ModHelper.Console.WriteLine(": WATER :");

            TessellatedSphereRenderer tsr = waterBase.AddComponent<TessellatedSphereRenderer>();
            tsr.tessellationMeshGroup = GameObject.Find("Ocean_GD").GetComponent<TessellatedSphereRenderer>().tessellationMeshGroup;
            tsr.sharedMaterials = GameObject.Find("Ocean_GD").GetComponent<TessellatedSphereRenderer>().sharedMaterials;
            tsr.maxLOD = 7;
            tsr.LODBias = 2;
            tsr.LODRadius = 2f;
            base.ModHelper.Console.WriteLine(":     TessellatedSphereRenderer done.");

            TessSphereSectorToggle toggle = waterBase.AddComponent<TessSphereSectorToggle>();
            toggle.SetValue("_sector", sector);
            base.ModHelper.Console.WriteLine(":     TessSphereSectorToggle done.");

            OceanEffectController effectC = waterBase.AddComponent<OceanEffectController>();
            effectC.SetValue("_sector", sector);
            effectC.SetValue("_ocean", tsr);
            base.ModHelper.Console.WriteLine(":     OceanEffectController done.");

            waterBase.SetActive(true);

            GameObject sunov = new GameObject();
            sunov.SetActive(false);
            sunov.transform.parent = body.transform;

            GiantsDeepSunOverrideVolume vol = sunov.AddComponent<GiantsDeepSunOverrideVolume>();
            vol.SetValue("_sector", sector);
            vol.SetValue("_cloudsOuterRadius", 400f);
            vol.SetValue("_cloudsInnerRadius", 380f);
            vol.SetValue("_waterOuterRadius", 502.5f);
            vol.SetValue("_waterInnerRadius", 402.5f);

            sunov.SetActive(true);
            base.ModHelper.Console.WriteLine(": All components finalized. Returning object...");

            return body;
        }
    }
}