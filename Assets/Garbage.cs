using UnityEngine;

public class Garbage : MonoBehaviour
{
    private float _weight;
    private float _value;

    public void Initialize(float weight, float value)
    {
        _weight = weight;
        _value = value;
        
        
        transform.localScale *= weight;
    }
}
