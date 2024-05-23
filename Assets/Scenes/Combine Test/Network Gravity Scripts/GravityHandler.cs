using UnityEngine;

public class GravityHandler : MonoBehaviour
{
    private bool isGravityEnabled = true;
    private int clickCount = 0;
    private const int maxClickCount = 1;
    private GameObject _gravityObject;
    
    public void ToggleGravity(GameObject gravityObject)
    {
        if (clickCount >= maxClickCount)
            return;

        isGravityEnabled = !isGravityEnabled;

        foreach (Transform child in gravityObject.transform)
        {
            if (child.gameObject.name == "GravityArea")
            {
                _gravityObject = child.gameObject;
                StartCoroutine(ToggleGravityWithAnimation());
                clickCount++;
            }
        }
    }

    private System.Collections.IEnumerator ToggleGravityWithAnimation()
    {
        float duration = 1.2f;
        Vector3 originalScale = _gravityObject.transform.localScale;
        Vector3 targetScale = isGravityEnabled ? originalScale : Vector3.zero;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            _gravityObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _gravityObject.transform.localScale = targetScale;

        yield return new WaitForSeconds(1.0f);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            _gravityObject.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _gravityObject.transform.localScale = originalScale;

        EnableGravity();
    }

    private void EnableGravity()
    {
        isGravityEnabled = true;
        clickCount = 0;
    }
}
