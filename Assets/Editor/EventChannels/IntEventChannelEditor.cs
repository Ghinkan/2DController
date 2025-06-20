using UnityEditor;
using UnityUtils.EventChannels;
namespace UnityUtils.Editor.EventChannels
{
	[CustomEditor(typeof(IntEventChannel))]
	public class IntEventChannelEditor : GenericEventChannelEditor<int> { }
}