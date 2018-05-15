using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    /// <summary>
    /// Simple method extension to save 2 or 3 lines of very redundant and ugly code. It will check if component exists before adding new one.
    /// </summary>
    static public class MethodExtensionForMonoBehaviourTransform
    {
        /// <summary>
        /// Gets or add a component. Usage example:
        /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        static public T vGetOrAddComponent<T>(this Component child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }
    }


    /// <summary>
    /// SomeEnum parsedState;
    ///if (parsedState.TryParse<SomeEnum>(aString, out parsedState))
    ///{
    ///	/* The parsed string matches an enum state, proceed successfully here. */
    ///}
    ///else
    ///{
    ///	/* The parsed  string does not match any enum state, handle failure here. */
    ///}
    /// </summary>
   
    public static class EnumExtensions
    {

        public static bool vTryParse<T>(this System.Enum theEnum, string valueToParse, out T returnValue)
        {
            returnValue = default(T);
            if (System.Enum.IsDefined(typeof(T), valueToParse))
            {
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                returnValue = (T)converter.ConvertFromString(valueToParse);
                return true;
            }
            return false;
        }

    }
    

    //This C# class gives simple extension access to checking if an Renderer is rendered by a specific Camera.
    /*
     * public class TestRendered : MonoBehaviour
       {	
            void Update()
            {
                if (renderer.IsVisibleFrom(Camera.main)) Debug.Log("Visible");
                else Debug.Log("Not visible");
            }
       }
    */

    public static class RendererExtensions
    {
        public static bool vIsVisibleFrom(this Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }

    /*
    LayerMaskExtensions.Create(params string[] names) - Creates a new LayerMask from a variable number of layer names
    LayerMaskExtensions.Create(params int[] layerNumbers) - Creates a new LayerMask from a variable number of layer numbers
    LayerMaskExtensions.NamesToMask(params string[] names) - Same as Create
    LayerMaskExtensions.LayerNumbersToMask(params int[] layerNumbers) - Same as Create
    layerMask.Inverse() - Returns the inverse of the mask
    layerMask.AddToMask(params string[] names) - Returns a new LayerMask with the specified layers added
    layerMask.RemoveFromMask(params string[] names) - Returns a new LayerMask with the specified layers removed
    layerMask.MaskToNames() - Returns a string array with the layer names from the mask
    layerMask.MaskToString() - Returns a string with the layer names from the mask, delimited by comma
    layerMask.MaskToString(string delimiter) - Returns a string with the layer names from the mask, delimited by the specified delimiter
    */

    public static class LayerMaskExtensions
    {
        public static LayerMask Create(params string[] layerNames)
        {
            return NamesToMask(layerNames);
        }

        public static LayerMask Create(params int[] layerNumbers)
        {
            return LayerNumbersToMask(layerNumbers);
        }

        public static LayerMask NamesToMask(params string[] layerNames)
        {
            LayerMask ret = (LayerMask)0;
            foreach (var name in layerNames)
            {
                ret |= (1 << LayerMask.NameToLayer(name));
            }
            return ret;
        }

        public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
        {
            LayerMask ret = (LayerMask)0;
            foreach (var layer in layerNumbers)
            {
                ret |= (1 << layer);
            }
            return ret;
        }

        public static LayerMask Inverse(this LayerMask original)
        {
            return ~original;
        }

        public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
        {
            return original | NamesToMask(layerNames);
        }

        public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
        {
            LayerMask invertedOriginal = ~original;
            return ~(invertedOriginal | NamesToMask(layerNames));
        }

        public static string[] MaskToNames(this LayerMask original)
        {
            var output = new List<string>();

            for (int i = 0; i < 32; ++i)
            {
                int shifted = 1 << i;
                if ((original & shifted) == shifted)
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(layerName))
                    {
                        output.Add(layerName);
                    }
                }
            }
            return output.ToArray();
        }

        public static string MaskToString(this LayerMask original)
        {
            return MaskToString(original, ", ");
        }

        public static string MaskToString(this LayerMask original, string delimiter)
        {
            return string.Join(delimiter, MaskToNames(original));
        }
    }
}