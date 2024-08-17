using GameLogic.Camera;
using Signals;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDetectorManager : MonoBehaviour
{
    List<PlayerDetector> detectorsThePlayerIsIn = new List<PlayerDetector>();

    public void PlayerEntered(PlayerDetector detector)
    {
        detectorsThePlayerIsIn.Add(detector);
        ChangeToCollider(detectorsThePlayerIsIn.Last().PolygonCollider2D);
    }

    public void PlayerExited(PlayerDetector detector)
    {
        detectorsThePlayerIsIn.Remove(detector);
        
        if (detectorsThePlayerIsIn.Count > 0)
            ChangeToCollider(detectorsThePlayerIsIn.Last().PolygonCollider2D);
    }

    public void ChangeToCollider(PolygonCollider2D polygonCollider2D)
    {
        SignalBus<SignalSwitchCameraBoundary>.Fire(new SignalSwitchCameraBoundary
        { ColliderToSwitch = polygonCollider2D });
    }

    [ContextMenu("Inject Reference")]
    void InjectReferences()
    {
        PlayerDetector[] detectors = FindObjectsByType<PlayerDetector>(FindObjectsSortMode.None);
        foreach (PlayerDetector detector in detectors)
            detector.SetManagerTo(this);
    }
}
