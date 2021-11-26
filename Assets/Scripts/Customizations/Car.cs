using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "Car", menuName = "Customizations/Car")]
    public class Car : ThemeCustomizationItem
    {
        public CarType carType;
    }

    public enum CarType
    {
        SportsCar,
        PickupTruck,
        Jeep,
        Winnebago,
        WienerMobile
    }
}