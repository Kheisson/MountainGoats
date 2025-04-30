using Core;
using UnityEngine;
using UnityEngine.UI;

public class FuelController : BaseMonoBehaviour
{
    [SerializeField] private Image fuelBar;
    [SerializeField] private float fuelAmount;

    private float currentFuelAmount;

    protected override void Awake()
    {
        base.Awake();
        currentFuelAmount = fuelAmount;
    }

    public void UseFuel(float fuelToUse)
    {
        currentFuelAmount = Mathf.Max(currentFuelAmount - fuelToUse, 0);
    }
}
