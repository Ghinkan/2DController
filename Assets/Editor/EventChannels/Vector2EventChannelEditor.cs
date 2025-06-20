using UnityEditor;
using UnityEngine;
using UnityUtils.EventChannels;
namespace UnityUtils.Editor.EventChannels
{
    [CustomEditor(typeof(Vector2EventChannel))]
    public class Vector2EventChannelEditor : GenericEventChannelEditor<Vector2> { }
}
