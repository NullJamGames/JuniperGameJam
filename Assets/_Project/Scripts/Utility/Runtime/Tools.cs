using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace NJG.Utilities
{
    public static class Tools
    {
        /// <summary>
        ///     Checks if a GameObject is in a given LayerMask.
        /// </summary>
        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) =>
            (layerMask.value & (1 << obj.layer)) != 0;

        /// <summary>
        ///     Will get all the layers that are currently set in the editor that are not blank.
        /// </summary>
        public static IEnumerable<string> GetLayerNames()
        {
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                    yield return layerName;
            }
        }

        /// <summary>
        ///     Generates a collection of layer indices for all non-empty layers defined in the Unity Editor
        /// </summary>
        public static IEnumerable<int> GetLayerIndices() => GetLayerNames().Select(LayerMask.NameToLayer);

        /// <summary>
        ///     Will Debug.Log all layers in a given mask. Optionally provide a collider name for easier readability.
        /// </summary>
        public static void LogLayersInMask(LayerMask mask, string colliderName = "NOT PROVIDED")
        {
            for (int i = 0; i < 32; i++)
            {
                int shifted = 1 << i; // Shift 1 by i places to get the mask for layer i
                if ((mask.value & shifted) == shifted) // Check if the mask includes this layer
                {
                    string layerName = LayerMask.LayerToName(i);
                    Debug.Log("Name: " + colliderName + " Layer: " + i + ": " + layerName);
                }
            }
        }

#if ODIN_INSPECTOR
        /// <summary>
        /// This is used to help generate layer names for editor dropdown purposes.
        /// </summary>
        public static IEnumerable<ValueDropdownItem<int>> GetLayerDropdownItems()
        {
            for (int i = 0; i < 32; i++)
            {
                string name = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(name))
                    continue;

                yield return new ValueDropdownItem<int>(name, i);
            }
        }
#endif

        /// <summary>
        ///     Toggles the visibility of a CanvasGroup.
        /// </summary>
        public static void ToggleVisibility(CanvasGroup canvasGroup, bool isVisible)
        {
            canvasGroup.alpha = isVisible ? 1 : 0;
            canvasGroup.blocksRaycasts = isVisible;
            canvasGroup.interactable = isVisible;
        }
        
        /// <summary>
        ///     Toggles the visibility of a CanvasGroup.
        /// </summary>
        public static void ToggleVisibility(CanvasGroup canvasGroup, bool isVisible, bool blockRaycasts, bool interactable)
        {
            canvasGroup.alpha = isVisible ? 1 : 0;
            canvasGroup.blocksRaycasts = blockRaycasts;
            canvasGroup.interactable = interactable;
        }

        public static bool TryLoadResource<T>(string resourcePath, out T asset) where T : Object
        {
            asset = Resources.Load<T>(resourcePath);
            if (asset != null)
                return true;

            Debug.LogError($"{typeof(T).Name} asset file/path '{resourcePath}' not found in the resources folder.");
            return false;
        }

        /// <summary>
        ///     Helper to normalize angles into [-180, 180] range.
        /// </summary>
        public static float NormalizeAngle(float angle)
        {
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }
        
        public static float Normalize180(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
    }
}