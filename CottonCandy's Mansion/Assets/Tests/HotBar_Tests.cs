using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HotBar_Tests
{
    private HotBar _playerHotbar;
    private GameObject _playerTestObject;
    [SetUp]
    public void HotBar_Setup()
    {
        _playerHotbar = new();
    }
    [TearDown]
    public void HotBar_Teardown()
    {
        _playerHotbar = null;
    }
    // A Test behaves as an ordinary method
    [Test]
    public void HotBar_PickUpOneItemTest()
    {
        GameObject object1 = new();

        _playerHotbar.PickUpItem(object1);

        Assert.AreEqual(_playerHotbar.GetCurrentlyHeldItem(), object1);

        GameObject.DestroyImmediate(object1);

    }
    [Test]
    public void HotBar_PickUpTwoItemsTest()
    {
        GameObject object1 = new();
        GameObject object2 = new();

        _playerHotbar.PickUpItem(object1);
        _playerHotbar.PickUpItem(object2);

        Assert.AreEqual(_playerHotbar.GetOffHandItem(), object2);

        GameObject.DestroyImmediate(object1);     
        GameObject.DestroyImmediate(object2);   
    }

    [Test]
    public void HotBar_PickUpThreeItemsTest()
    {
        GameObject object1 = new();
        GameObject object2 = new();
        GameObject object3 = new();

        _playerHotbar.PickUpItem(object1);
        _playerHotbar.PickUpItem(object2);
        _playerHotbar.PickUpItem(object3);

        Assert.AreEqual(_playerHotbar.GetCurrentlyHeldItem(), object3);

        GameObject.DestroyImmediate(object1);     
        GameObject.DestroyImmediate(object2);   
        GameObject.DestroyImmediate(object3); 
    }
    [Test]
    public void HotBar_SwitchCurrentlyHeldAndOffHandTest()
    {
        GameObject object1 = new();
        GameObject object2 = new();

        _playerHotbar.PickUpItem(object1);
        _playerHotbar.PickUpItem(object2);

        _playerHotbar.SwitchMainHandAndOffHand();


        Assert.AreEqual(_playerHotbar.GetCurrentlyHeldItem(), object2);

        GameObject.DestroyImmediate(object1);     
        GameObject.DestroyImmediate(object2);         
    }
    [Test]
    public void HotBar_PutDownCurrentlyHeldItemTest()
    {
        GameObject object1 = new();

        _playerHotbar.PickUpItem(object1);

        _playerHotbar.PutDownItem(object1);
        Assert.AreEqual(_playerHotbar.GetCurrentlyHeldItem(),null);

        GameObject.DestroyImmediate(object1);
    }

    [Test]
    public void HotBar_PutDownOffHandItemTest()
    {
        GameObject object1 = new();
        GameObject object2 = new();

        _playerHotbar.PickUpItem(object1);
        _playerHotbar.PickUpItem(object2);

        _playerHotbar.PutDownItem(object2);
        Assert.AreEqual(_playerHotbar.GetOffHandItem(),null);

        GameObject.DestroyImmediate(object1);
        GameObject.DestroyImmediate(object2);
    }


}
