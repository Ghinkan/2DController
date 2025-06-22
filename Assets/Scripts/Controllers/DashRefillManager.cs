using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Controller2DProject.Controllers
{
    public class DashRefillManager
    {
        private readonly PlayerData _playerData;
        public int DashesLeft { get; private set; }
    
        private bool _refilling;
        private int _refillQueue;

        public DashRefillManager(PlayerData playerData)
        {
            _playerData = playerData;
            DashesLeft = _playerData.DashAmount;
        }

        public void ConsumeDash()
        {
            DashesLeft--;
            _refillQueue++;
            _ = RefillDashAsync();
        }

        private async Task RefillDashAsync()
        {
            if (_refilling) return;
            _refilling = true;

            while (_refillQueue > 0 && DashesLeft < _playerData.DashAmount)
            {
                // await Task.Delay((int)(_playerData.DashRefillTime * 1000));
                await Task.Delay(TimeSpan.FromSeconds(_playerData.DashRefillTime));
                
                DashesLeft = Mathf.Min(_playerData.DashAmount, DashesLeft + 1);
                _refillQueue--;
            }

            _refilling = false;
        }
    }
}