using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image loadingScreen;

    private void Awake()
    {
        loadingScreen = GetComponent<Image>();
        Debug.Log("Loading Screen");
        StartCoroutine(FadeInCoroutine(1));
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        Color startColor = new Color(loadingScreen.color.r, loadingScreen.color.g, loadingScreen.color.b, 1);
        Color targetColor = new Color(loadingScreen.color.r, loadingScreen.color.g, loadingScreen.color.b, 0);

        yield return FadeCoroutine(startColor, targetColor, duration);

        gameObject.SetActive(false);
    }
    private IEnumerator FadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        yield return new WaitForSeconds(6);
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            loadingScreen.color = Color.Lerp(startColor, targetColor, elapsedPercentage);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        yield return null;
    }

}
