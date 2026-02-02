using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenShake : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }

    private void Update()
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            DEBUG_Shake();
        }
    }

    [ContextMenu("DEBUG Shake")]
    public void DEBUG_Shake()
    {
        Shake(5f, 2f, .66f);
    }

    public void StopShake()
    {
        StopAllCoroutines();
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }
    public void Shake(float amplitude, float frequency, float duration)
    {
        StartCoroutine(ShakeCoroutine(amplitude, frequency, duration));
    }
    private IEnumerator ShakeCoroutine(float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain += amplitude;
        noise.FrequencyGain += frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain -= amplitude;
        noise.FrequencyGain -= frequency;
    }
}
