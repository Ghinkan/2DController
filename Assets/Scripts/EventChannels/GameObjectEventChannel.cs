using UnityEngine;
namespace UnityUtils.EventChannels
{
    [CreateAssetMenu(fileName = "GameObjectChannel", menuName = "Events/GameObject Event Channel")]
    public class GameObjectEventChannel : GenericEventChannel<GameObject> { }

}