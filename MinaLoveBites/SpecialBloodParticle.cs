using Lua;
using Lua.Wrapper;
using MinaLoveBites.ColorScheme;
using UnityEngine;

namespace MinaLoveBites;

public class SpecialBloodParticle: MonoBehaviour
{
    private const int LAYER_MASK = 1;
    public const float LIFETIME = 5f;
    public const float LAUNCH_SPEED = 2f;
    public const float LIFETIME_DECALS_ONLY = 0.5f;
    public const float LAUNCH_SPEED_DECALS_ONLY = 2f;
    public const float BASE_SPEED_MULTIPLIER = 0.1f;
    public float expires;
    public Vector3 velocity;
    public Actor actor;

    private void Update()
    {
        if ((double) Time.time > (double) this.expires)
        {
            Object.Destroy((Object) this.gameObject);
        }
        else
        {
            this.velocity += Physics.gravity * Time.deltaTime;
            Vector3 vector3 = this.velocity * Time.deltaTime;
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(this.transform.position, vector3.normalized), out hitInfo, vector3.magnitude,
                    1))
            {
                bool flag = (Object)hitInfo.rigidbody == (Object)null;
                ScriptEvent<Vector3, WTeam, bool> bloodParticleLand = RavenscriptManager.events.onBloodParticleLand;
                bloodParticleLand.Invoke(hitInfo.point, (WTeam)actor.team, flag);
                if (flag && !bloodParticleLand.isConsumed)
                {
                    // LoveBites.Logger.LogInfo("Splat!");
                    // DecalManager.DecalType type = ColorSchemeExtensions.Instance.GetOverrideRT(actor) == null ? 
                        // (actor.team == 0 ? DecalManager.DecalType.BloodBlue : DecalManager.DecalType.BloodRed) : (DecalManager.DecalType) ColorSchemeExtensions.Instance.GetMeshKey(ColorSchemeExtensions.Instance.GetActorColor(actor));
                        DecalManager.DecalType type =
                            (actor.team == 0 ? DecalManager.DecalType.BloodBlue : DecalManager.DecalType.BloodRed);
                    DecalManager.AddDecal(hitInfo.point, hitInfo.normal, Random.Range(0.7f, 2.5f), type);
                }

            Object.Destroy((Object) this.gameObject);
            }
            else
                this.transform.position += vector3;
        }
    }
}