using System.Collections;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class Gravity_Tests
{
    private PlayerSettingsSO _playerSettingsSO;
    private GameObject _gravityObject;
    private GravityChanger _gravityChanger;
    [SetUp]
    public void GravitySetup()
    {
        _playerSettingsSO = ScriptableObject.CreateInstance<PlayerSettingsSO>();
        _gravityObject = new();
        _gravityChanger = _gravityObject.AddComponent<GravityChanger>();
        _gravityChanger.SetGravityDirection(Vector3.down);
    }
    [TearDown]
    public void GravityTearDown()
    {
        GameObject.DestroyImmediate(_playerSettingsSO);
        GameObject.DestroyImmediate(_gravityObject);
    }
    [Test]
    public void Gravity_GravityDirectionTest()
    {
        Vector3 newDirection = Vector3.forward;

        _gravityChanger.SetGravityDirection(newDirection);

        Assert.AreEqual(newDirection,_gravityChanger.GetGravityDirection());
    }
    [Test]
    public void Gravity_GravitySetTest()
    {
        Vector3 originalDirection = _gravityChanger.GetGravityDirection();

        Vector3 directionToChange = Vector3.zero;

        _gravityChanger.ChangeGravity(ref directionToChange);

        Assert.AreEqual(originalDirection,directionToChange);
    }

}
