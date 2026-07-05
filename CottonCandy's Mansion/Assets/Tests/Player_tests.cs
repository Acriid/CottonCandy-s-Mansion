using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;


public class Player_Tests
{
    private GameObject _playerObject;
    private Rigidbody _playerRigidBody;
    private GameObject _groundCheckObject;
    private GameObject _cameraObject;
    private Player _playerComponent;
    private PlayerSettingsSO _playerSettingsSO;
    private PlayerSlopeSettingsSO _playerSlopeSettingsSO;
    [SetUp]
    public void PlayerSetup()
    {
        _playerObject = new();
        _playerRigidBody = _playerObject.AddComponent<Rigidbody>();
        _playerRigidBody.useGravity = false;
        _playerRigidBody.isKinematic = false;
        _playerObject.AddComponent<CapsuleCollider>();

        _groundCheckObject = new();
        _groundCheckObject.transform.parent = _playerObject.transform;
        _groundCheckObject.transform.localPosition = new(0f,-1f,0f);

        _cameraObject = new();
        Camera cameraComponent = _cameraObject.AddComponent<Camera>();
        _cameraObject.transform.parent = _playerObject.transform;

        _playerComponent = _playerObject.AddComponent<Player>();
        _playerComponent.SetRigidBody(_playerRigidBody);

        _playerSettingsSO = ScriptableObject.CreateInstance<PlayerSettingsSO>();
        _playerComponent.SetPlayerSettings(_playerSettingsSO);

        _playerSlopeSettingsSO = ScriptableObject.CreateInstance<PlayerSlopeSettingsSO>();
        _playerComponent.SetPlayerSlopeSettings(_playerSlopeSettingsSO);

        _playerComponent.SetMainCamera(cameraComponent);
    }

    [TearDown]
    public void PlayerTearDown()
    {
        GameObject.DestroyImmediate(_playerSettingsSO);
        GameObject.DestroyImmediate(_playerSlopeSettingsSO);

        GameObject.DestroyImmediate(_cameraObject);
        GameObject.DestroyImmediate(_groundCheckObject);
        GameObject.DestroyImmediate(_playerObject);


        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
    
    [Test]
    public void Player_SettingsTest()
    {
        PlayerSettingsSO newPlayerSettingsSO = ScriptableObject.CreateInstance<PlayerSettingsSO>();

        _playerComponent.SetPlayerSettings(newPlayerSettingsSO);

        Assert.AreEqual(_playerComponent.GetPlayerSettings(),newPlayerSettingsSO);

        GameObject.DestroyImmediate(newPlayerSettingsSO);
    }

    [Test]
    public void Player_SlopeSettingsTest()
    {
        PlayerSlopeSettingsSO newPlayerSlopeSettingsSO = ScriptableObject.CreateInstance<PlayerSlopeSettingsSO>();

        _playerComponent.SetPlayerSlopeSettings(newPlayerSlopeSettingsSO);

        Assert.AreEqual(_playerComponent.GetPlayerSlopeSettings(),newPlayerSlopeSettingsSO);

        GameObject.DestroyImmediate(newPlayerSlopeSettingsSO);       
    }

    [Test]
    public void Player_CameraTest()
    {
        GameObject newCamera = new();
        Camera newCameraComponent = newCamera.AddComponent<Camera>();

        _playerComponent.SetMainCamera(newCameraComponent);

        Assert.AreEqual(newCameraComponent,_playerComponent.GetMainCamera());

        GameObject.DestroyImmediate(newCamera);

    }
    [Test]
    public void Player_LeftAirMovementTest()
    {
        Physics.simulationMode = SimulationMode.Script;
        Vector3 originalVelocity = _playerComponent.GetRigidBody().linearVelocity;  

        MockPhysicsSettings mockPhysicsSettings = new()
        {
            Grounded = false
        };
        _playerComponent.SetPhysicsChecks(mockPhysicsSettings);

        MockSlopeSettings mockSlopeSettings = new()
        {
            OnWalkableSlope = false
        };
        _playerComponent.SetSlopeDetection(mockSlopeSettings);

        
        for(int i = 0 ; i<60; i++)
        {
            _playerComponent.MovePlayer(Vector2.left);
            Physics.Simulate(Time.fixedDeltaTime);
        }

        Vector3 newVelocity = _playerComponent.GetRigidBody().linearVelocity;
        bool result = newVelocity.y != originalVelocity.y && newVelocity.x != originalVelocity.x;

        Assert.AreEqual(result,true);  
    }

    [Test]
    public void Player_LeftGroundMovementTest()
    {
        Physics.simulationMode = SimulationMode.Script;
        Vector3 originalVelocity = _playerComponent.GetRigidBody().linearVelocity;  

        MockPhysicsSettings mockPhysicsSettings = new()
        {
            Grounded = true
        };
        _playerComponent.SetPhysicsChecks(mockPhysicsSettings);

        MockSlopeSettings mockSlopeSettings = new()
        {
            OnWalkableSlope = false
        };
        _playerComponent.SetSlopeDetection(mockSlopeSettings);

        
        for(int i = 0 ; i<60; i++)
        {
            _playerComponent.MovePlayer(Vector2.left);
            Physics.Simulate(Time.fixedDeltaTime);
        }

        Vector3 newVelocity = _playerComponent.GetRigidBody().linearVelocity;
        bool result = Mathf.Approximately(newVelocity.y,0f) && newVelocity.x != originalVelocity.x;

        Assert.AreEqual(result,true);  
    }

}
