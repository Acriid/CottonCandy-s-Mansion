using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;


public class Player_tests
{
    private GameObject playerObject;
    private Rigidbody playerRigidBody;
    private GameObject groundCheckObject;
    private GameObject cameraObject;
    private Player_New playerComponent;
    private PlayerSettingsSO playerSettingsSO;
    private PlayerSlopeSettingsSO playerSlopeSettingsSO;
    [SetUp]
    public void PlayerSetup()
    {
        playerObject = new();
        playerRigidBody = playerObject.AddComponent<Rigidbody>();
        playerRigidBody.useGravity = false;
        playerRigidBody.isKinematic = false;
        playerObject.AddComponent<CapsuleCollider>();

        groundCheckObject = new();
        groundCheckObject.transform.parent = playerObject.transform;
        groundCheckObject.transform.localPosition = new(0f,-1f,0f);

        cameraObject = new();
        Camera cameraComponent = cameraObject.AddComponent<Camera>();
        cameraObject.transform.parent = playerObject.transform;

        playerComponent = playerObject.AddComponent<Player_New>();
        playerComponent.SetRigidBody(playerRigidBody);

        playerSettingsSO = ScriptableObject.CreateInstance<PlayerSettingsSO>();
        playerComponent.SetPlayerSettings(playerSettingsSO);

        playerSlopeSettingsSO = ScriptableObject.CreateInstance<PlayerSlopeSettingsSO>();
        playerComponent.SetPlayerSlopeSettings(playerSlopeSettingsSO);

        playerComponent.SetMainCamera(cameraComponent);
    }

    [TearDown]
    public void PlayerTearDown()
    {
        GameObject.DestroyImmediate(playerSettingsSO);
        GameObject.DestroyImmediate(playerSlopeSettingsSO);

        GameObject.DestroyImmediate(cameraObject);
        GameObject.DestroyImmediate(groundCheckObject);
        GameObject.DestroyImmediate(playerObject);


        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
    
    [Test]
    public void PlayerSettingsTest()
    {
        PlayerSettingsSO newPlayerSettingsSO = ScriptableObject.CreateInstance<PlayerSettingsSO>();

        playerComponent.SetPlayerSettings(newPlayerSettingsSO);

        Assert.AreEqual(playerComponent.GetPlayerSettings(),newPlayerSettingsSO);

        GameObject.DestroyImmediate(newPlayerSettingsSO);
    }

    [Test]
    public void PlayerSlopeSettingsTest()
    {
        PlayerSlopeSettingsSO newPlayerSlopeSettingsSO = ScriptableObject.CreateInstance<PlayerSlopeSettingsSO>();

        playerComponent.SetPlayerSlopeSettings(newPlayerSlopeSettingsSO);

        Assert.AreEqual(playerComponent.GetPlayerSlopeSettings(),newPlayerSlopeSettingsSO);

        GameObject.DestroyImmediate(newPlayerSlopeSettingsSO);       
    }

    [Test]
    public void PlayerCameraTest()
    {
        GameObject newCamera = new();
        Camera newCameraComponent = newCamera.AddComponent<Camera>();

        playerComponent.SetMainCamera(newCameraComponent);

        Assert.AreEqual(newCameraComponent,playerComponent.GetMainCamera());

        GameObject.DestroyImmediate(newCamera);

    }
    [Test]
    public void PlayerTestLeftAirMovement()
    {
        Physics.simulationMode = SimulationMode.Script;
        Vector3 originalVelocity = playerComponent.GetRigidBody().linearVelocity;  

        MockPhysicsSettings mockPhysicsSettings = new()
        {
            Grounded = false
        };
        playerComponent.SetPhysicsChecks(mockPhysicsSettings);

        MockSlopeSettings mockSlopeSettings = new()
        {
            OnWalkableSlope = false
        };
        playerComponent.SetSlopeDetection(mockSlopeSettings);

        
        for(int i = 0 ; i<60; i++)
        {
            playerComponent.MovePlayer(Vector2.left);
            Physics.Simulate(Time.fixedDeltaTime);
        }

        Vector3 newVelocity = playerComponent.GetRigidBody().linearVelocity;
        bool result = newVelocity.y != originalVelocity.y && newVelocity.x != originalVelocity.x;

        Assert.AreEqual(result,true);  
    }

    [Test]
    public void PlayerTestLeftGroundMovement()
    {
        Physics.simulationMode = SimulationMode.Script;
        Vector3 originalVelocity = playerComponent.GetRigidBody().linearVelocity;  

        MockPhysicsSettings mockPhysicsSettings = new()
        {
            Grounded = true
        };
        playerComponent.SetPhysicsChecks(mockPhysicsSettings);

        MockSlopeSettings mockSlopeSettings = new()
        {
            OnWalkableSlope = false
        };
        playerComponent.SetSlopeDetection(mockSlopeSettings);

        
        for(int i = 0 ; i<60; i++)
        {
            playerComponent.MovePlayer(Vector2.left);
            Physics.Simulate(Time.fixedDeltaTime);
        }

        Vector3 newVelocity = playerComponent.GetRigidBody().linearVelocity;
        bool result = Mathf.Approximately(newVelocity.y,0f) && newVelocity.x != originalVelocity.x;

        Assert.AreEqual(result,true);  
    }

}
