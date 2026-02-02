using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MatEditor : MonoBehaviour
{
    public ShaderLink shaderLink;
    public AnimationCurve invulnerabilityCurve;
    public Gradient gradient;

    [ContextMenu("Debug Invulnerability")]
    public void Debug_Invulnerability()
    {
        StartCoroutine(InvulnerabilityColorChange(2f));
    }

    public IEnumerator InvulnerabilityColorChange(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float normalizedTime = elapsed / duration;
            float curveValue = invulnerabilityCurve.Evaluate(normalizedTime);
            shaderLink.UpdateProperty(("_Invulnerability", curveValue));
            Color color = gradient.Evaluate(normalizedTime);
            shaderLink.UpdateProperty(("_InvulColor", color));
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        shaderLink.UpdateProperty(("_Invulnerability", 0f)); // Reset to no invulnerability at the end
    }
}
