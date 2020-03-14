using OWML.ModHelper.Events;
using UnityEngine;

namespace MysteryPlanet
{
    static class MakeWater
    {
        public static void Make(GameObject body, float waterScale)
        {
            GameObject waterBase = new GameObject();
            waterBase.SetActive(false);
            waterBase.layer = 15;
            waterBase.transform.parent = body.transform;
            waterBase.transform.localScale = new Vector3(waterScale, waterScale, waterScale);
            waterBase.DestroyAllComponents<SphereCollider>();

            TessellatedSphereRenderer tsr = waterBase.AddComponent<TessellatedSphereRenderer>();
            tsr.tessellationMeshGroup = GameObject.Find("Ocean_GD").GetComponent<TessellatedSphereRenderer>().tessellationMeshGroup;
            tsr.sharedMaterials = GameObject.Find("Ocean_GD").GetComponent<TessellatedSphereRenderer>().sharedMaterials;
            tsr.maxLOD = 7;
            tsr.LODBias = 2;
            tsr.LODRadius = 2f;

            TessSphereSectorToggle toggle = waterBase.AddComponent<TessSphereSectorToggle>();
            toggle.SetValue("_sector", MainClass.SECTOR);

            OceanEffectController effectC = waterBase.AddComponent<OceanEffectController>();
            effectC.SetValue("_sector", MainClass.SECTOR);
            effectC.SetValue("_ocean", tsr);

            waterBase.SetActive(true);
        }
    }
}
