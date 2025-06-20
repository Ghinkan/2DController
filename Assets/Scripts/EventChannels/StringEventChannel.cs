using UnityEngine;
namespace UnityUtils.EventChannels
{
    [CreateAssetMenu(menuName = "Events/StringEventChannel", fileName = "String Event Channel")]
    public class StringEventChannel : GenericEventChannel<string> { }
}