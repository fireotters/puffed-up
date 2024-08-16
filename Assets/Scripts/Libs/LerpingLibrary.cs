using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public static class LerpingLibrary
{
    public static async UniTaskVoid ExecuteOverDuration(this MonoBehaviour go, float duration, CancellationToken cancellationToken, Action<float> action)
    {
        float time = 0;

        while (time < duration)
        {
            action?.Invoke(time / duration);
            time += Time.deltaTime;

            await UniTask.Yield(cancellationToken);
        }

        action?.Invoke(1);
    }

    public static async UniTaskVoid AsyncDelayedCall(this MonoBehaviour go, float delay, CancellationToken cancellationToken, Action onComplete)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
        onComplete?.Invoke();
    }

    public static async UniTask DelayedCall(this MonoBehaviour go, float delay, CancellationToken cancellationToken, Action onComplete)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
        onComplete?.Invoke();
    }

    public static UniTask LerpPosition(this MonoBehaviour go, Vector3 endValue, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete = null)
    {
        return LerpPosition(go.transform, endValue, duration, curve, cancellationToken, onComplete);
    }

    public static UniTask LerpScale(this MonoBehaviour go, Vector3 endValue, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete = null)
    {
        return LerpScale(go.transform, go.transform.localScale, endValue, duration, curve, cancellationToken, onComplete);
    }

    public static UniTask LerpRotation(this MonoBehaviour go, Quaternion destinationValue, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete = null)
    {
        return LerpQuaternion(go.transform, destinationValue, duration, curve, cancellationToken, onComplete);
    }
    
    public static UniTask LerpTransform(this MonoBehaviour go, Vector3 translation, Quaternion rotation, Vector3 scale, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete = null)
    {
        return LerpTransform(go.transform, translation, rotation, scale, duration, curve, cancellationToken, onComplete);
    }

    private static async UniTask LerpPosition(Transform transform, Vector3 destinationValue, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete)
    {
        float time = 0;

        Vector3 startingPos = transform.position;
        
        while (time < duration)
        {
           transform.position = Vector3.Lerp(startingPos, destinationValue, curve.Evaluate(time/duration));
            
           time += Time.deltaTime;
           
            await UniTask.Yield(cancellationToken);
        }

        transform.position = destinationValue;
        onComplete?.Invoke();
    }
    
    private static async UniTask LerpScale(Transform t, Vector3 startingPos, Vector3 destinationValue, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete)
    {

        float time = 0;

        while (time < duration)
        {
            t.localScale = Vector3.Lerp(startingPos, destinationValue, curve.Evaluate(time/duration));
            
            time += Time.deltaTime;

            await UniTask.Yield(cancellationToken);
        }

        t.localScale = destinationValue;
        onComplete?.Invoke();
    }
    
   private static async UniTask LerpQuaternion(Transform transform, Quaternion destinationValue, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete)
    {

        float time = 0;

        var startingRotation = transform.localRotation;
        
        while (time < duration)
        {
            transform.localRotation = Quaternion.Lerp(startingRotation, destinationValue, curve.Evaluate(time)/duration);
            
            time += Time.deltaTime;

            await UniTask.Yield(cancellationToken);
        }

        transform.localRotation = destinationValue;
        onComplete?.Invoke();
    }

    private static async UniTask LerpTransform(Transform transform, Vector3 translation, Quaternion rotation, Vector3 scale, float duration, AnimationCurve curve, CancellationToken cancellationToken, Action onComplete)
    {
        float time = 0;

        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;
        Vector3 startingScale = transform.localScale;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startingPos, translation, curve.Evaluate(time / duration));
            transform.rotation = Quaternion.Lerp(startingRot, rotation, curve.Evaluate(time / duration));
            transform.localScale = Vector3.Lerp(startingScale, scale, curve.Evaluate(time / duration));

            time += Time.deltaTime;

            await UniTask.Yield(cancellationToken);
        }

        transform.position = translation;
        transform.rotation = rotation;
        transform.localScale = scale;
        onComplete?.Invoke();
    }
}
