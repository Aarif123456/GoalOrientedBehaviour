#region Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

// Microsoft Reciprocal License (Ms-RL)
//
// This license governs use of the accompanying software. If you use the software, you accept this
// license. If you do not accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
// meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// copyright license to reproduce its contribution, prepare derivative works of its contribution,
// and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works of the contribution in
// the software.
//
// 3. Conditions and Limitations
// (A) Reciprocal Grants- For any file you distribute that contains code from the software (in
// source code or binary format), you must provide recipients the source code to that file along
// with a copy of this license, which license will govern that file. You may license other files
// that are entirely your own work and do not contain code from the software under any terms you
// choose.
// (B) No Trademark License- This license does not grant you rights to use any contributors' name,
// logo, or trademarks.
// (C) If you bring a patent claim against any contributor over patents that you claim are
// infringed by the software, your patent license from such contributor to the software ends
// automatically.
// (D) If you distribute any portion of the software, you must retain all copyright, patent,
// trademark, and attribution notices that are present in the software.
// (E) If you distribute any portion of the software in source code form, you may do so only under
// this license by including a complete copy of this license with your distribution. If you
// distribute any portion of the software in compiled or object code form, you may only do so under
// a license that complies with this license.
// (F) The software is licensed "as-is." You bear the risk of using it. The contributors give no
// express warranties, guarantees or conditions. You may have additional consumer rights under your
// local laws which this license cannot change. To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for a particular purpose
// and non-infringement.

#endregion Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

using System.Collections.Generic;
using UnityEngine;

// Add to the component menu.
namespace GameWorld.Cameras {
/* Fades out any objects between the player and this transform.
   The renderers shader is first changed to be an Alpha/Diffuse, then alpha is faded out to fadedOutAlpha.
   Only objects

   In order to catch all occluders, 5 rays are casted. occlusionRadius is the distance between them.
*/
    public class FadeOutLineOfSight : TargetedCamera {
        public LayerMask layerMask = 2;
        public float fadeSpeed = 1.0f;
        public float occlusionRadius = 0.3f;
        public float fadedOutAlpha = 0.3f;

        private readonly List<FadeOutLOSInfo> fadedOutObjects = new List<FadeOutLOSInfo>();

        // After all objects are initialized, Awake is called when the script
        // is being loaded. This occurs before any Start calls.
        // Use Awake instead of the constructor for initialization.
        public override void Awake(){
            base.Awake();

            if (!target){
                Debug.Log("Please assign a target to the camera.");
            }

            CameraName = "Fade Out Line Of Sight";
        }

        // If this behaviour is enabled, LateUpdate is called once per frame
        // after all Update functions have been called.
        public void LateUpdate(){
            if (!target){
                return;
            }

            var from = transform.position;
            var to = target.position;
            var castDistance = Vector3.Distance(to, from);

            // Mark all objects as not needing fade out
            foreach (var fade in fadedOutObjects){
                fade.needFadeOut = false;
            }

            Vector3[] offsets ={
                new Vector3(0, 0, 0), new Vector3(0, occlusionRadius, 0), new Vector3(0, -occlusionRadius, 0),
                new Vector3(occlusionRadius, 0, 0), new Vector3(-occlusionRadius, 0, 0)
            };

            // We cast 5 rays to really make sure even occluders that are partly occluding the player are faded out
            foreach (var offset in offsets){
                var relativeOffset = transform.TransformDirection(offset);

                // Find all blocking objects which we want to hide
                var hits = Physics.RaycastAll(from + relativeOffset, to - from, castDistance, layerMask.value);
                foreach (var hit in hits){
                    if (hit.transform.gameObject == target.gameObject){
                        continue;
                    }

                    // Make sure we have a renderer
                    var hitRenderer = hit.collider.GetComponent<Renderer>();
                    if (hitRenderer == null || !hitRenderer.enabled){
                        continue;
                    }

                    var info = FindLosInfo(hitRenderer);

                    // We are not fading this renderer already, so insert into faded objects map
                    if (info == null){
                        info = new FadeOutLOSInfo();
                        info.originalMaterials = hitRenderer.sharedMaterials;
                        info.alphaMaterials = new Material[info.originalMaterials.Length];
                        info.renderer = hitRenderer;
                        for (var i = 0; i < info.originalMaterials.Length; i++){
                            var newMaterial = new Material(Shader.Find("Alpha/Diffuse"));
                            newMaterial.mainTexture = info.originalMaterials[i].mainTexture;
                            var color = info.originalMaterials[i].color;
                            color.a = 1.0f;
                            newMaterial.color = color;
                            info.alphaMaterials[i] = newMaterial;
                        }

                        hitRenderer.sharedMaterials = info.alphaMaterials;
                        fadedOutObjects.Add(info);
                    }
                    // Just mark the renderer as needing fade out
                    else{
                        info.needFadeOut = true;
                    }
                }
            }

            // Now go over all renderers and do the actual fading!
            var fadeDelta = fadeSpeed * Time.deltaTime;
            for (var i = 0; i < fadedOutObjects.Count; i++){
                var fade = fadedOutObjects[i];
                // Fade out up to minimum alpha value
                if (fade.needFadeOut){
                    foreach (var alphaMaterial in fade.alphaMaterials){
                        var alpha = alphaMaterial.color.a;
                        alpha -= fadeDelta;
                        alpha = Mathf.Max(alpha, fadedOutAlpha);
                        var color = alphaMaterial.color;
                        color.a = alpha;
                        alphaMaterial.color = color;
                    }
                }
                // Fade back in
                else{
                    var totallyFadedIn = 0;
                    foreach (var alphaMaterial in fade.alphaMaterials){
                        var alpha = alphaMaterial.color.a;
                        alpha += fadeDelta;
                        alpha = Mathf.Min(alpha, 1.0f);
                        var color = alphaMaterial.color;
                        color.a = alpha;
                        alphaMaterial.color = color;
                        if (alpha >= 0.99){
                            totallyFadedIn++;
                        }
                    }

                    // All alpha materials are faded back to 100%
                    // Thus we can switch back to the original materials
                    if (totallyFadedIn == fade.alphaMaterials.Length){
                        if (fade.renderer){
                            fade.renderer.sharedMaterials = fade.originalMaterials;
                        }

                        foreach (var newMaterial in fade.alphaMaterials){
                            Destroy(newMaterial);
                        }

                        fadedOutObjects.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private FadeOutLOSInfo FindLosInfo(Renderer r){
            foreach (var fade in fadedOutObjects){
                if (r == fade.renderer){
                    return fade;
                }
            }

            return null;
        }

        public class FadeOutLOSInfo {
            public Material[] alphaMaterials;
            public bool needFadeOut = true;
            public Material[] originalMaterials;
            public Renderer renderer;
        }
    }
}