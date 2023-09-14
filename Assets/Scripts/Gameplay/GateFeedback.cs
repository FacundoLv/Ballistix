using System.Collections;
using UnityEngine;

public class GateFeedback : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    private LineRenderer _lineRenderer;
    private int _currentSpriteIndex;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    public void OnLocked()
    {
        _lineRenderer.enabled = true;
        StartCoroutine(AnimateLighting());
    }

    private IEnumerator AnimateLighting()
    {
        do
        {
            yield return new WaitForSeconds(1 / 12f);
            _currentSpriteIndex = (_currentSpriteIndex + 1) % sprites.Length;
            _lineRenderer.material.SetTexture(MainTex, sprites[_currentSpriteIndex].texture);
        } while (isActiveAndEnabled || _lineRenderer.enabled);
    }
}