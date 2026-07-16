using System.Collections;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class Gravity_Tests
{
    private Player _playerComponent;
    private GameObject _playerObject;
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

        _playerObject = new();
        _playerComponent = _playerObject.AddComponent<Player>();
        _playerComponent.SetPlayerSettings(_playerSettingsSO);
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

}
