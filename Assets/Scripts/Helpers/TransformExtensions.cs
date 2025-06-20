using System.Collections.Generic;
using UnityEngine.Events;
namespace UnityEngine
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Flips the transform's local scale towards a specified movement direction.
        /// </summary>
        /// <param name="transform">The transform to flip.</param>
        /// <param name="movementDirection">The movement direction. Positive values flip to the right, negative values flip to the left.</param>
        public static void FlipScaleTowards(this Transform transform, float movementDirection)
        {
            if (movementDirection != 0f)
            {
                Vector3 scale = transform.localScale;

                if (movementDirection > 0f)
                    scale.x = Mathf.Abs(scale.x);
                else
                    scale.x = -Mathf.Abs(scale.x);

                transform.localScale = scale;
            }
        }
        
        /// <summary>
        /// Flips the transform's local scale towards a target transform.
        /// </summary>
        /// <param name="transform">The transform to flip.</param>
        /// <param name="target">The target transform to flip towards.</param>
        public static void FlipScaleTowards(this Transform transform, Transform target)
        {
            Vector3 scale = transform.localScale;

            if (target.position.x > transform.position.x)
                scale.x = Mathf.Abs(scale.x);
            else
                scale.x = -Mathf.Abs(scale.x);

            transform.localScale = scale;
        }
        
        /// <summary>
        /// Check if the transform is within a certain distance and optionally within a certain angle (FOV) from the target transform.
        /// </summary>
        /// <param name="source">The transform to check.</param>
        /// <param name="target">The target transform to compare the distance and optional angle with.</param>
        /// <param name="maxDistance">The maximum distance allowed between the two transforms.</param>
        /// <param name="maxAngle">The maximum allowed angle between the transform's forward vector and the direction to the target (default is 360).</param>
        /// <returns>True if the transform is within range and angle (if provided) of the target, false otherwise.</returns>
        public static bool InRangeOf(this Transform source, Transform target, float maxDistance, float maxAngle = 360f)
        {
            Vector3 directionToTarget = (target.position - source.position).Set(y: 0);
            return directionToTarget.magnitude <= maxDistance && Vector3.Angle(source.forward, directionToTarget) <= maxAngle / 2;
        }
        
        /// <summary>
        /// Retrieves all the children of a given Transform.
        /// </summary>
        /// <param name="parent">The Transform to retrieve children from.</param>
        /// <returns>An IEnumerable&lt;Transform&gt; containing all the child Transforms of the parent.</returns>    
        public static IEnumerable<Transform> Children(this Transform parent)
        {
            foreach (Transform child in parent)
                yield return child;
        }
        
        /// <summary>
        /// Resets transform's position, scale and rotation
        /// </summary>
        /// <param name="transform">Transform to use</param>
        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// Destroys all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be destroyed.</param>
        public static void DestroyChildren(this Transform parent)
        {
            parent.ForEveryChild(child => Object.Destroy(child.gameObject));
        }
        
        /// <summary>
        /// Immediately destroys all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be immediately destroyed.</param>
        public static void DestroyChildrenImmediate(this Transform parent)
        {
            parent.ForEveryChild(child => Object.DestroyImmediate(child.gameObject));
        }
        
        /// <summary>
        /// Enables all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be enabled.</param>
        public static void EnableChildren(this Transform parent)
        {
            parent.ForEveryChild(child => child.gameObject.SetActive(true));
        }

        /// <summary>
        /// Disables all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be disabled.</param>
        public static void DisableChildren(this Transform parent)
        {
            parent.ForEveryChild(child => child.gameObject.SetActive(false));
        }
        
        /// <summary>
        /// Executes a specified action for each child of a given transform in reverse order.
        /// </summary>
        /// <param name="parent">The parent transform.</param>
        /// <param name="action">The action to be performed on each child.</param>
        public static void ForEveryChild(this Transform parent, UnityAction<Transform> action)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                action(parent.GetChild(i));
            }
        }
    }
}