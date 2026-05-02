using HarmonyLib;
using MinaLoveBites.ColorScheme;
using UnityEngine;

namespace MinaLoveBites.DecalManagerExtras;

public static class DecalManagerExtras
{
    public static void CreateBloodDrop(Vector3 point, Vector3 baseVelocity, Actor actor)
    {

        if (ColorSchemeExtensions.Instance.GetOverrideRT(actor) == null)
        {
            DecalManager.CreateBloodDrop(point, baseVelocity, actor.team);
            return;
        }
        if (!DecalManager.instance.emitBloodDrops)
            return;

        Color color = ColorSchemeExtensions.Instance.GetActorColor(actor);
        float bloodParticleSize = ActorManager.instance.bloodParticleSize;
        BloodParticle component1 = UnityEngine.Object.Instantiate<GameObject>(DecalManager.instance.bloodDropPrefab, point, Quaternion.identity).AddComponent<BloodParticle>();
        // GameObject.Destroy(component1.transform.gameObject.GetComponent<BloodParticle>());
        component1.transform.localScale.Scale(new Vector3(bloodParticleSize, bloodParticleSize, bloodParticleSize) * UnityEngine.Random.Range(2f, 3f));
        float num = ActorManager.instance.bloodParticleLifetime;
        if (BloodParticle.BLOOD_PARTICLE_SETTING == BloodParticle.BloodParticleType.DecalOnly)
        {
            component1.velocity = baseVelocity * 2f + (UnityEngine.Random.insideUnitSphere + Vector3.down) * 2f;
            num = 0.5f;
        }
        else
            component1.velocity = baseVelocity + (UnityEngine.Random.insideUnitSphere + new Vector3(0.0f, 1.2f, 0.0f)) * ActorManager.instance.bloodParticleVerticalLaunchSpeed;
        component1.expires = Time.time + num;
        component1.team = actor.team;
        Renderer component2 = component1.GetComponent<Renderer>();
        if (BloodParticle.BLOOD_PARTICLE_SETTING == BloodParticle.BloodParticleType.DecalOnly)
            component2.enabled = false;
        else
            component2.material.color = color;
    }
        
        public static void EmitBloodEffect(Vector3 point, Vector3 baseVelocity, Actor actor)
        {
            if (ColorSchemeExtensions.Instance.GetOverrideRT(actor) == null)
            {
                DecalManager.EmitBloodEffect(point, baseVelocity, actor.team);
                return;
            }
            if (!DecalManager.instance.emitSplatParticles)
                return;
            
            var bloodParticleParams = (ParticleSystem.EmitParams) AccessTools.DeclaredField(typeof(DecalManager), "bloodParticleParams").GetValue(DecalManager.instance);
            var splatParticleSystem = (ParticleSystem) AccessTools.DeclaredField(typeof(DecalManager), "splatParticleSystem").GetValue(DecalManager.instance);
            bloodParticleParams.position = point - baseVelocity.normalized * 0.4f;
            bloodParticleParams.velocity = -baseVelocity * 0.3f + Vector3.up + UnityEngine.Random.insideUnitSphere;
            bloodParticleParams.startColor = (Color32) ColorSchemeExtensions.Instance.GetActorColor(actor);
            bloodParticleParams.startSize = UnityEngine.Random.Range(0.7f, 3f);
            splatParticleSystem.Emit(bloodParticleParams, 1);
        }
        //
        // private static void PushQuad(
        //     DecalManager.MeshData mesh,
        //     Vector3 c1,
        //     Vector3 c2,
        //     Vector3 c3,
        //     Vector3 c4,
        //     Vector3 normal)
        // {
        //     int vertexIndex = mesh.vertexIndex;
        //     mesh.verts[vertexIndex] = c1;
        //     mesh.verts[vertexIndex + 1] = c2;
        //     mesh.verts[vertexIndex + 2] = c3;
        //     mesh.verts[vertexIndex + 3] = c4;
        //     mesh.normals[vertexIndex] = normal;
        //     mesh.normals[vertexIndex + 1] = normal;
        //     mesh.normals[vertexIndex + 2] = normal;
        //     mesh.normals[vertexIndex + 3] = normal;
        //     mesh.vertexIndex = (vertexIndex + 4) % mesh.maxVerts;
        //     mesh.dirty = true;
        // }
        //
        // internal static bool TryAddGroundDecal(Vector3 point, Vector3 normal, float size, Color color)
        // {
        //     var mesh = ColorSchemeExtensions.Instance.GetMeshData(color);
        //     if (mesh == null || mesh.maxVerts <= 0)
        //         return false;
        //     Vector3 vector3 = Vector3.Cross(normal, UnityEngine.Random.insideUnitSphere).normalized * (size / 2f);
        //     Vector3 right = Vector3.Cross(normal, vector3);
        //     point += normal * 0.03f;
        //     int num = DecalManager.CanSpawnDecal(point, vector3, right, normal) ? 1 : 0;
        //     if (num == 0)
        //     {
        //         vector3 *= 0.3f;
        //         right *= 0.3f;
        //     }
        //     if (num == 0 && !DecalManager.CanSpawnDecal(point, vector3, right, normal))
        //         return false;
        //     PushQuad(teamMesh, point - vector3 - right, point + vector3 - right, point + vector3 + right, point - vector3 + right, normal);
        //     return true;
        // }
}