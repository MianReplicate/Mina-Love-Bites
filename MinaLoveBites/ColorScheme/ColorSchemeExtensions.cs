using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Lua.Proxy;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MinaLoveBites.ColorScheme;

public class ColorSchemeExtensions
{
        private static ColorSchemeExtensions _instance;
        public static ColorSchemeExtensions Instance
        {
            get
            {
                if (_instance == null)
                    new ColorSchemeExtensions();
                return _instance;
            }
        }

        private Dictionary<Actor, Color> _teamActorColorOverrides = new Dictionary<Actor, Color>();
        private Dictionary<Color, RenderTexture> _colorToRenderTextures = new Dictionary<Color, RenderTexture>();

        private Dictionary<Color, DecalManager.MeshData>
            _colorToMeshData = new Dictionary<Color, DecalManager.MeshData>();
        private Dictionary<Color, int>
            _colorToMeshKey = new Dictionary<Color, int>();

        private Vector3[] _vector3Array1;
        private Vector2[] _vector2Array;
        private Vector3[] _vector3Array2;
        private int[] _numArray;
        private List<GameObject> _decalDrawers;

        private ColorSchemeExtensions()
        {
            _instance = this;
            _decalDrawers = new();
            
            var maxVerts = (int) AccessTools.Field(typeof(DecalManager), "maxVerts").GetValue(DecalManager.instance);
            
            _vector3Array1 = new Vector3[maxVerts];
            _vector2Array = new Vector2[maxVerts];
            _vector3Array2 = new Vector3[maxVerts];
            _numArray = new int[3 * (maxVerts / 2)];
            for (int index = 0; index < maxVerts; ++index)
            {
                _vector3Array1[index] = new Vector3((float) (index / 2 % 2), (float) ((index + 1) / 2 % 2), 0.0f) * 0.0001f;
                _vector2Array[index] = new Vector2((float) (index / 2 % 2), (float) ((index + 1) / 2 % 2));
                _vector3Array2[index] = Vector3.up;
            }
            for (int index = 0; index < _numArray.Length / 6; ++index)
            {
                _numArray[index * 6] = index * 4;
                _numArray[index * 6 + 1] = index * 4 + 1;
                _numArray[index * 6 + 2] = index * 4 + 2;
                _numArray[index * 6 + 3] = index * 4;
                _numArray[index * 6 + 4] = index * 4 + 2;
                _numArray[index * 6 + 5] = index * 4 + 3;
            }
        }

        public static void Destroy()
        {
            var instance = _instance;
            _instance = null;
            
            if (instance != null)
            {
                foreach (var rt in instance._colorToRenderTextures.Values)
                {
                    GameObject.Destroy(rt);
                }
                foreach (var obj in instance._decalDrawers)
                {
                    GameObject.Destroy(obj);
                }
            }
        }
        
        public DecalManager.MeshData GetMeshData(Color color)
        {
            return _colorToMeshData[color];
        }


        public int GetMeshKey(Color color)
        {
            return _colorToMeshKey[color];
        }

        public Color GetActorColor(Actor actor)
        {
            var success = _teamActorColorOverrides.TryGetValue(actor, out var color);

            return success ? color : global::ColorScheme.TeamColor(actor.team);
        }

        public RenderTexture GetOverrideRT(Actor actor)
        {
            if (_teamActorColorOverrides.ContainsKey(actor))
            {
                return _colorToRenderTextures[_teamActorColorOverrides[actor]];
            }

            return null;
        }

        public void RemoveOverrideActorColor(Actor actor)
        {
            if (_teamActorColorOverrides.ContainsKey(actor))
            {
                var colorValue = _teamActorColorOverrides[actor];
                _teamActorColorOverrides.Remove(actor);

                var anyOtherHave = _teamActorColorOverrides.Any(pair => pair.Value.Equals(colorValue));
                if (!anyOtherHave)
                {
                    _colorToRenderTextures.TryGetValue(colorValue, out var mainRT);
                    if(mainRT != null)
                        GameObject.Destroy(mainRT);
                }
                
                actor.skinnedRenderer.material.mainTexture = ActorManager.instance.teamActorRT[actor.team];
                actor.skinnedRendererRagdoll.material.mainTexture = ActorManager.instance.teamActorRT[actor.team];
            
                if (!actor.aiControlled)
                {
                    foreach (var renderer in LocalPlayer.controller.fpParent.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        renderer.material.mainTexture = ActorManager.instance.teamActorRT[actor.team];
                    }
                }
            }
        }

        public void OverrideActorColor(Actor actor, Color color)
        {
            _teamActorColorOverrides.TryGetValue(actor, out var oldColor);
            if (oldColor == color)
                return;
            RemoveOverrideActorColor(actor);

            var team = actor.team;
            _colorToRenderTextures.TryGetValue(color, out var mainRT);
            if (mainRT == null){
                mainRT = new RenderTexture(ActorManager.instance.teamActorRT[team]);
                mainRT.Create();
                
                Color white = Color.white;
                Color.RGBToHSV(color, out white.r, out white.g, out white.b);
                ActorManager.instance.blitTeamColorMaterial.SetColor("_Color", white);
                Graphics.Blit((Texture) ActorManager.instance.teamActorTextureOriginal, mainRT, ActorManager.instance.blitTeamColorMaterial);
                
                _colorToRenderTextures.Add(color, mainRT);

                // var decalMeshData = (Dictionary<DecalManager.DecalType, DecalManager.MeshData>) AccessTools.Field(typeof(DecalManager), "decalMeshData").GetValue(DecalManager.instance);
                // var vertexIndex = (Dictionary<DecalManager.DecalType, int>) AccessTools.Field(typeof(DecalManager), "vertexIndex").GetValue(DecalManager.instance);
                //
                // Mesh mesh = new Mesh();
                // DecalMesh meshData = new DecalMesh();
                // mesh.name = $"{color.r} {color.g} {color.b}" + " mesh";
                // meshData.mesh = mesh;
                // meshData.verts = (Vector3[]) _vector3Array1.Clone();
                // meshData.uvs = (Vector2[]) _vector2Array.Clone();
                // meshData.normals = (Vector3[]) _vector3Array2.Clone();
                // meshData.vertexIndex = 0;
                // meshData.dirty = false;
                // meshData.maxVerts = meshData.verts.Length;
                // mesh.triangles = _numArray;
                // mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 9999f);
                // mesh.MarkDynamic();
                //
                // GameObject gameObject1 = (GameObject) null;
                // if (DecalManager.instance.decalDrawers != null && DecalManager.instance.decalDrawers.Length != 0)
                // {
                //     int index = DecalManager.instance.decalDrawers.Length > 2 ? 2 : DecalManager.instance.decalDrawers.Length - 1;
                //     gameObject1 = index >= 0 ? DecalManager.instance.decalDrawers[index] : (GameObject) null;
                // }
                // GameObject decalDrawer = new GameObject("Custom Splat Decal Drawer");
                // decalDrawer.transform.SetParent((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null ? gameObject1.transform.parent : DecalManager.instance.transform, false);
                // if ((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null)
                //     decalDrawer.layer = gameObject1.layer;
                // decalDrawer.SetActive(true);
                // MeshFilter meshFilter = decalDrawer.AddComponent<MeshFilter>();
                // MeshRenderer meshRenderer = decalDrawer.AddComponent<MeshRenderer>();
                // Material material1 = (UnityEngine.Object) gameObject1 != (UnityEngine.Object) null ? gameObject1.GetComponent<Renderer>()?.material : (Material) null;
                // Material material2 = (Material) null;
                // if ((UnityEngine.Object) material1 != (UnityEngine.Object) null)
                // {
                //     material2 = new Material(material1);
                // }
                // else
                // {
                //     Shader shader = Shader.Find("Diffuse") ?? Shader.Find("Standard") ?? Shader.Find("Legacy Shaders/Diffuse");
                //     if ((UnityEngine.Object) shader != (UnityEngine.Object) null)
                //         material2 = new Material(shader);
                // }
                // if ((UnityEngine.Object) material2 == (UnityEngine.Object) null)
                // {
                //     UnityEngine.Object.Destroy((UnityEngine.Object) decalDrawer);
                //     return;
                // }
                // material2.SetColor("_Color", color);
                // meshRenderer.material = material2;
                //
                // _decalDrawers.Add(decalDrawer);
                // var key = Mathf.Max(_decalDrawers.Count - 1, 3);
                //
                // decalDrawer.GetComponent<MeshFilter>().mesh = mesh;
                // decalMeshData.Add((DecalManager.DecalType)key, meshData);
                // vertexIndex.Add((DecalManager.DecalType)key, 0);
                //
                // decalDrawer.GetComponent<Renderer>().material.SetColor("_Color", color);
                //
                // // too see if this works
                // // var testPurposes = decalDrawers[1];
                // // testPurposes.GetComponent<MeshFilter>().mesh = mesh;
                // // testPurposes.GetComponent<Renderer>().material.SetColor("_Color", color);
                // //
                //
                // _colorToMeshData.Add(color, meshData);
                // _colorToMeshKey.Add(color, key);
                //
            }

            actor.skinnedRenderer.material.mainTexture = mainRT;
            actor.skinnedRendererRagdoll.material.mainTexture = mainRT;

            _teamActorColorOverrides[actor] = color;
            
            if (!actor.aiControlled)
            {
                foreach (var renderer in LocalPlayer.controller.fpParent.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.material.mainTexture = mainRT;
                }
            }
        }
        
        // internal static void FlushDirty()
        // {
        //     foreach (MultiTeamBloodDecalRuntime.TeamDecalMesh teamDecalMesh in MultiTeamBloodDecalRuntime.TeamMeshes.Values)
        //     {
        //         if ((UnityEngine.Object) teamDecalMesh?.drawerObject != (UnityEngine.Object) null && !teamDecalMesh.drawerObject.activeSelf)
        //             teamDecalMesh.drawerObject.SetActive(true);
        //         if (!((UnityEngine.Object) teamDecalMesh?.mesh == (UnityEngine.Object) null) && teamDecalMesh.dirty)
        //         {
        //             teamDecalMesh.mesh.vertices = teamDecalMesh.verts;
        //             teamDecalMesh.mesh.normals = teamDecalMesh.normals;
        //             teamDecalMesh.mesh.uv = teamDecalMesh.uvs;
        //             teamDecalMesh.dirty = false;
        //         }
        //     }
        // }
        //
        // private sealed class DecalMesh
        // {
        //     internal GameObject drawerObject;
        //     internal Mesh mesh;
        //     internal Material material;
        //     internal Vector3[] verts;
        //     internal Vector3[] normals;
        //     internal Vector2[] uvs;
        //     internal int vertexIndex;
        //     internal bool dirty;
        //     internal int maxVerts;
        // }
}