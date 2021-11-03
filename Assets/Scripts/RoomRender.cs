using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using Zenject;

public class RoomRender : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] SpriteRenderer roomBackGround;
    [SerializeField] SpriteRenderer computer;
    [SerializeField] SpriteRenderer camera;
    [SerializeField] SpriteRenderer microphone;
    [SerializeField] SpriteRenderer desk;
    [SerializeField] SpriteRenderer[] windows;
    [SerializeField] SpriteRenderer[] paints;
    [SerializeField] SpriteRenderer table;
    [SerializeField]  SpriteRenderer shelf;
    
    void Start()
    {
        signalBus.Subscribe<OnPlayerRoomThemeItemEquippedSignal>(OnEquipRoomThemeItemFired);
    }

    public void OnEquipRoomThemeItemFired(OnPlayerRoomThemeItemEquippedSignal roomThemeItemEquippedSignal)
    {
        switch (roomThemeItemEquippedSignal.ThemeCustomizationItem)
        {
            case WallOrnament wallOrnament : AddWallOrnament(wallOrnament); break;
            case FloorOrnament floorOrnament: break;
            case RoomObject roomObject : break;
            
        }
    }

    void AddWallOrnament(WallOrnament wallOrnament)
    {
        switch (wallOrnament.WallOrnamentType)
        {
            case WallOrnamentType.Paintings:
                break;
            case WallOrnamentType.TV:
                break;
            case WallOrnamentType.Windows:
                break;
            case WallOrnamentType.Blackboard:
                break;
            case WallOrnamentType.Bookshelf:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void AddFloorOrnament(FloorOrnament floorOrnament)
    {
        switch (floorOrnament.floorOrnamentType)
        {
            case FloorOrnamentType.Table:
                break;
            case FloorOrnamentType.Statues:
                break;
            case FloorOrnamentType.CatTree:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void AddRoomObject(RoomObject roomObject)
    {
        switch (roomObject.roomObjectType)
        {
            case RoomObjectType.Ball:
                break;
            case RoomObjectType.VideoGameConsole:
                break;
            case RoomObjectType.FlowerVase:
                break;
            case RoomObjectType.ClothingRack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    void Update()
    {
        
    }
}
