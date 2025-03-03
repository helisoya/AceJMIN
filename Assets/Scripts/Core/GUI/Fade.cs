using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a fadable image
/// </summary>
public class Fade : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private CanvasGroup canvaGroup;
    [SerializeField] private Image fadeImg;
    public float currentAlpha
    {
        get { return canvaGroup.alpha; }
    }
    public bool fading { get { return routineFading != null; } }
    public Color currenColor { get { return fadeImg.color; } }
    private Coroutine routineFading;

    /// <summary>
    /// Fades the image
    /// <param name="alpha"/>The alpha target</param>
    /// <param name="speed">The transition's speed</param>
    /// </summary>
    public void FadeTo(float alpha, float speed = 2)
    {
        if (routineFading != null) StopCoroutine(routineFading);
        routineFading = StartCoroutine(RoutineFading(alpha, speed));
    }

    /// <summary>
    /// Changes the color of the fade
    /// </summary>
    /// <param name="color">The new color</param>
    public void SetColor(Color color)
    {
        fadeImg.color = color;
    }

    /// <summary>
    /// Forces the alpha value
    /// </summary>
    /// <param name="alpha">The new alpha value</param>
    public void ForceAlphaTo(float alpha)
    {
        canvaGroup.alpha = alpha;
        if (routineFading != null)
        {
            StopCoroutine(routineFading);
            routineFading = null;
        }
    }


    /// <summary>
    /// IEnumerator for the image's fading
    /// </summary>
    /// <param name="target">The target alpha</param>
    /// <param name="speed">The transition's speed</param>
    /// <returns>IEnumerator</returns>
    IEnumerator RoutineFading(float target, float speed = 2)
    {
        float currentAlpha = canvaGroup.alpha;
        int side = target > currentAlpha ? 1 : -1;
        float max = side == 1 ? target : 1f;
        float min = side == 1 ? currentAlpha : target;

        while (currentAlpha != target)
        {
            currentAlpha = Mathf.Clamp(currentAlpha + speed * side * Time.deltaTime, min, max);
            canvaGroup.alpha = currentAlpha;
            yield return new WaitForEndOfFrame();
        }

        routineFading = null;
    }
}
