using System.Collections;
using UnityEngine;

namespace LoR.NeuroIntegration.Controllers;

public class CursorPositionController : MonoBehaviour
{
    public static CursorPositionController Instance;

    public Vector3 CursorPosition = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    public void MoveTo(Vector3 position, float time)
    {
        StartCoroutine(MoveToCoroutine(position, time));
    }

    public void SetPosition(Vector3 position)
    {
        CursorPosition = position;
    }

    private IEnumerator MoveToCoroutine(Vector3 position, float time)
    {
        var elapsedTime = 0f;
        while (elapsedTime < time)
        {
            CursorPosition = Vector3.Lerp(CursorPosition, position, Mathf.Min(elapsedTime / time, 1));
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
    }
}
