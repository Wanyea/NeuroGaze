using System.Collections;
using UnityEngine;

public class CubeTrainingStimulus : MonoBehaviour
{
    [SerializeField] private Transform cube;
    [SerializeField] private float shrinkDurationSeconds = 8f;

    private Vector3 _originalScale = new Vector3(0.23f, 0.23f, 0.23f);
    private Coroutine _loopRoutine;

    private void Awake()
    {
        if (cube == null) cube = transform;
        _originalScale = cube.localScale;
    }

    public void StartLoop()
    {
        StopLoop();
        _loopRoutine = StartCoroutine(ShrinkLoop());
    }

    public void StopLoop()
    {
        if (_loopRoutine != null)
        {
            StopCoroutine(_loopRoutine);
            _loopRoutine = null;
        }
        ResetCube();
    }

    public void ResetCube()
    {
        if (cube != null) cube.localScale = _originalScale;
    }

    private IEnumerator ShrinkLoop()
    {
        while (true)
        {
            yield return ShrinkOnce();
            // small pause between reps if you want:
            // yield return new WaitForSeconds(0.25f);
            ResetCube();
        }
    }

    private IEnumerator ShrinkOnce()
    {
        float t = 0f;
        while (t < shrinkDurationSeconds)
        {
            float a = t / shrinkDurationSeconds;
            cube.localScale = Vector3.Lerp(_originalScale, Vector3.zero, a);
            t += Time.deltaTime;
            yield return null;
        }
        cube.localScale = Vector3.zero;
    }
}