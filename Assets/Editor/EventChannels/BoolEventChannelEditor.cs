using UnityEditor;
using UnityUtils.EventChannels;
namespace UnityUtils.Editor.EventChannels
{
    [CustomEditor(typeof(BoolEventChannel))]
    public class BoolEventChannelEditor : GenericEventChannelEditor<bool> { }
}
