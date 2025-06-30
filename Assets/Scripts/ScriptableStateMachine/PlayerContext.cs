using Controller2DProject.Controllers;
using Controller2DProject.ScriptableStateMachine.Controller;
using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "PlayerContext", menuName = "StateMachine/PlayerContext", order = 0)]
    public class PlayerContext : ScriptableObject
    {
        public PlayerController  PlayerController;
        public DashRefillManager DashRefill;
        public bool IsDashing;
        
        public void Initialize(PlayerController playerController)
        {
            PlayerController = playerController;
            DashRefill = new DashRefillManager(PlayerController.PlayerData);
        }
    }
}