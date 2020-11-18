using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class ExplosionLighting : MonoBehaviour
{
    private Light2D light;

    [SerializeField]
    private float maxIntensity;
    [SerializeField]
    private float minIntensity;
    [SerializeField]
    private float fadeInTime;
    [SerializeField]
    private float fadeOutTime;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        StartCoroutine(LightFadeInOut());
    }

    private IEnumerator LightFadeInOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInTime)
        {
            light.intensity = Mathf.Lerp(minIntensity, maxIntensity, elapsedTime / fadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            light.intensity = Mathf.Lerp(maxIntensity, minIntensity, elapsedTime / fadeOutTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}
