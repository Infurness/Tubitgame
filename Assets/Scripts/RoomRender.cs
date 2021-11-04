using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoomRender : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private Image computer;
    [SerializeField] private Image camera;
    [SerializeField] private Image microphone;
    [SerializeField] private Image painting;
    [SerializeField] private Image tv;
    [SerializeField] private Image window;
    [SerializeField] private Image blackBoard;
    [SerializeField] private Image bookShelf;
    [SerializeField] private Image table;
    [SerializeField] private Image statues;
    [SerializeField] private Image carTree;
    [SerializeField] private Image videoGameConsole;
    [SerializeField] private Image ball;
    [SerializeField] private Image flowerVase;
    [SerializeField] private Image clothingRack;
    [SerializeField] private Canvas roomCanvas;
    
    
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
                painting.sprite = wallOrnament.wallOrnamentSprite;
                break;
            case WallOrnamentType.TV:
                tv.sprite = wallOrnament.wallOrnamentSprite;
                break;
            case WallOrnamentType.Windows:
                window.sprite = wallOrnament.wallOrnamentSprite;
                break;
            case WallOrnamentType.Blackboard:
                blackBoard.sprite = wallOrnament.wallOrnamentSprite;
                break;
            case WallOrnamentType.Bookshelf:
                bookShelf.sprite = wallOrnament.wallOrnamentSprite;
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
                table.sprite = floorOrnament.floorOrnamentSprite;
                break;
            case FloorOrnamentType.Statues:
                statues.sprite = floorOrnament.floorOrnamentSprite;
                break;
            case FloorOrnamentType.CatTree:
                carTree.sprite = floorOrnament.floorOrnamentSprite;
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
                ball.sprite = roomObject.roomObjectSprite;
                break;
            case RoomObjectType.VideoGameConsole:
                videoGameConsole.sprite = roomObject.roomObjectSprite;
                break;
            case RoomObjectType.FlowerVase:
                flowerVase.sprite = roomObject.roomObjectSprite;
                break;
            case RoomObjectType.ClothingRack:
                clothingRack.sprite = roomObject.roomObjectSprite;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
 
}
