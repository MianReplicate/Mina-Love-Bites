using HarmonyLib;
using UnityEngine;

namespace MinaLoveBites.DecalManagerExtras;

public class DecalManagerExtras
{
            public static void CreateBloodDrop(Vector3 point, Vector3 baseVelocity, Actor actor)
        {
            if (!DecalManager.instance.emitBloodDrops)
                return;
            Color color = ColorSchemeExtensions.ColorSchemeExtensions.instance.GetActorColor(actor);
            float bloodParticleSize = ActorManager.instance.bloodParticleSize;
            BloodParticle component1 = UnityEngine.Object.Instantiate<GameObject>(DecalManager.instance.bloodDropPrefab, point, Quaternion.identity).GetComponent<BloodParticle>();
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
            if (!DecalManager.instance.emitSplatParticles)
                return;

            var bloodParticleParams = (ParticleSystem.EmitParams) AccessTools.DeclaredField(typeof(DecalManager), "bloodParticleParams").GetValue(DecalManager.instance);
            var splatParticleSystem = (ParticleSystem) AccessTools.DeclaredField(typeof(DecalManager), "splatParticleSystem").GetValue(DecalManager.instance);
            bloodParticleParams.position = point - baseVelocity.normalized * 0.4f;
            bloodParticleParams.velocity = -baseVelocity * 0.3f + Vector3.up + UnityEngine.Random.insideUnitSphere;
            bloodParticleParams.startColor = (Color32) ColorSchemeExtensions.ColorSchemeExtensions.instance.GetActorColor(actor);
            bloodParticleParams.startSize = UnityEngine.Random.Range(0.7f, 3f);
            splatParticleSystem.Emit(bloodParticleParams, 1);
        }
}