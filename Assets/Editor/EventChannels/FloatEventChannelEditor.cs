using UnityEditor;
using UnityUtils.EventChannels;
namespace UnityUtils.Editor.EventChannels
{
	[CustomEditor(typeof(FloatEventChannel))]
	public class FloatEventChannelEditor : GenericEventChannelEditor<float> { }
}