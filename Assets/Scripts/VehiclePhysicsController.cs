using UnityEngine;
using System.Collections;

public class VehiclePhysicsController : MonoBehaviour {

    cam cameraController;

    void Awake()
    {
        cameraController = Camera.main.GetComponent<cam>();
    }

    void OnTriggerEnter(Collider other)
    {
        cameraController.SetUseVehicleUpVector(true);
        Debug.Log("trigger enter");
    }

    void OnTriggerExit(Collider other)
    {
        cameraController.SetUseVehicleUpVector(false);
        Debug.Log("trigger exit");
    }

    //void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("trigger stay");
    //}
}
