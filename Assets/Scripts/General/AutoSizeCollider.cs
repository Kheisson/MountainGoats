using Core;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AutoSizeCollider : MonoBehaviour
{
    public bool includeInactiveChildren = false;
    public float ColliderWidth => GetComponent<BoxCollider2D>().size.x;
    public (float,float) ColliderMinMax() => (transform.position.x - ColliderWidth / 2, transform.position.x + ColliderWidth / 2);

    private void Start()
    {
        SetColliderSizeToChildren();
    }
    
    public void SetColliderSizeToChildren()
    {
        var childRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactiveChildren);
        
        if (childRenderers.Length == 0)
        {
            MgLogger.LogWarning("No sprite renderers found in children!");
            return;
        }
        
        var bounds = childRenderers[0].bounds;
        
        for (int i = 1; i < childRenderers.Length; i++)
        {
            bounds.Encapsulate(childRenderers[i].bounds);
        }
        
        var localCenter = transform.InverseTransformPoint(bounds.center);
        var localSize = transform.InverseTransformVector(bounds.size);
        
        var boxCollider = GetComponent<BoxCollider2D>();
        
        boxCollider.size = new Vector2(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y));
        boxCollider.offset = new Vector2(localCenter.x, localCenter.y);
    }
}