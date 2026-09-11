using UnityEngine;

public class SimpleRenderColorChangeService : MonoBehaviour, IObjectColorChangeService
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void ChangeColor(Color newColor)
    {
        if (meshRenderer)
            meshRenderer.material.color = newColor;
    }
}
