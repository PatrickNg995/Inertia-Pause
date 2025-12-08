using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public static class PostProcessEffectManager
{
    public static IEnumerator FadeInFadOutEffect(Volume volume, float duration)
    {
        // Fade in.
        yield return FadeEffectWeight(volume, duration, 0f, 1f);

        // Fade out.
        yield return FadeEffectWeight(volume, duration, 1f, 0f);

        // Ensure effect is off.
        volume.weight = 0f;
    }

    private static IEnumerator FadeEffectWeight(Volume volume, float duration, float start, float end)
    {
        //volume.weight = start;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            volume.weight = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
