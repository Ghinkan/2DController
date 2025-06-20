using UnityEditor;
using UnityEngine;
using UnityUtils.EventChannels;
namespace UnityUtils.Editor.EventChannels
{
	[CustomEditor(typeof(Vector2EventChannel))]
	public class Vector3EventChannelEditor : GenericEventChannelEditor<Vector3> { }
}