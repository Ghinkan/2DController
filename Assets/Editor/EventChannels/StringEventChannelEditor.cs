using UnityEditor;
using UnityUtils.EventChannels;
namespace UnityUtils.Editor.EventChannels
{
    [CustomEditor(typeof(StringEventChannel))]
    public class StringEventChannelEditor : GenericEventChannelEditor<string> { }
}
