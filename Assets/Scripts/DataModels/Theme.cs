using UnityEngine;
public enum ThemeType 
{   FilmsAndAnimation,
    AutosAndVehicles, 
    Music, 
    PetsAndAnimals, 
    Sports, 
    TravelAndEvents, 
    Gaming,
    PeopleAndBlogs,
    Comedy,
    Entertainment,
    NewsAndPolitics,
    HowToAndStyle,
    Education,
    ScienceAndTechnology
}
[System.Serializable]
public class Theme
{
    public ThemeType themeType;
    [Header ("Each Element is the hour and the value its popularity")]
    public float[] popularityEachHour;
}