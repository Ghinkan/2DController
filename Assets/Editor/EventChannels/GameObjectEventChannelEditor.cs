using UnityEditor;
using UnityEngine;
using UnityUtils.EventChannels;
namespace UnityUtils.Editor.EventChannels
{
    [CustomEditor(typeof(GameObjectEventChannel))]
    public class GameObjectEventChannelEditor : GenericEventChannelEditor<GameObject> { }

}


