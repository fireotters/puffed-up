using Cinemachine;
using Signals;
using UnityEngine;

namespace GameLogic.Camera
{
    public class BoundarySwitcher : MonoBehaviour
    {
        private CinemachineConfiner2D _confiner2D;
        private readonly CompositeDisposable _disposables = new();
        
        private void Start()
        {
            _confiner2D = GetComponent<CinemachineConfiner2D>();
            
            SignalBus<SignalSwitchCameraBoundary>.Subscribe(SwitchToBoundary).AddTo(_disposables);
        }

        private void SwitchToBoundary(SignalSwitchCameraBoundary signal)
        {
            _confiner2D.m_BoundingShape2D = signal.ColliderToSwitch;
        }
    }    
}
