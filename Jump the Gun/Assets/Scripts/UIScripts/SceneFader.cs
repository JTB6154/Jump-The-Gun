using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;

    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fade to a certain scene in game
    /// </summary>
    /// <param name="scene">Scene to be transitioned to</param>
    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    IEnumerator FadeIn()
    {
        transform.GetChild(0).gameObject.SetActive(true);

        Debug.Log("Fade In");
        float t = fadeInTime;

        while (t > 0f) // Keep animating until t reaches 0
        {
            //Debug.Log("t = " + t);
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return 0; // Skip to the next frame
        }

        transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator FadeOut(string scene)
    {
        transform.GetChild(0).gameObject.SetActive(true);

        float t = 0;

        while (t < fadeOutTime) // Keep animating until t reaches 0
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return 0; // Skip to the next frame
        }

        // Load the scene
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}