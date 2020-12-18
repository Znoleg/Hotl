using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TransformDetector
{
    private readonly IDetectorUser detectorUser;
    //private bool lastDetectionSuccessful = false;

    //public bool LastDetectionSuccessful => lastDetectionSuccessful;
    public TransformDetector(IDetectorUser detectorUser)
    {
        this.detectorUser = detectorUser;
    }
    public IEnumerable<Transform> Detect()
    {
        if (detectorUser.DetectingTargets is null) yield break;
        foreach (var target in detectorUser.DetectingTargets)
        {
            if (TryDetect(target)) yield return target;
        }
    }

    private bool TryDetect(Transform target)
    {
        if (target == null)
            return false;

        var source = detectorUser.Transform;
        var distance = detectorUser.DetectingDistance;
        var angle = detectorUser.DetectingAmplitude;

        if (Vector2.Distance(source.position, target.position) > distance)
            return false;

        if (Vector2.Angle(source.right, target.position - source.position) > (angle / 2))
            return false;

        var hitObjects = Physics2D.RaycastAll(source.position, target.position - source.position, distance + 1f);
        foreach (var hit in hitObjects)
        {
            Debug.DrawLine(source.position, hit.transform.position, Color.blue, 2f);
            if (hit.transform == source) continue;
            if (hit.collider.isTrigger) continue;

            var body = hit.transform.GetComponent<PhysicalBody>();
            if (body && !(body is Character)) 
                if (!body.Transparent) return false;

            if (hit.transform == target) return true;
            Debug.DrawRay(source.position, hit.transform.position - source.position, Color.red, 3f);
        }
        return true;
    }

}
