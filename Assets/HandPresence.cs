using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;

public class HandPresence : MonoBehaviour
{
    public bool showController = false;
    public List<GameObject> ControllerPrefabs;
    public InputDeviceCharacteristics ControllerCharacteristic;
    public GameObject handModelPrefab;

    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;

    private Animator handAnimator;

    void Start()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristic, devices);

        var device = devices.FirstOrDefault();
        if (device == null)
        {
            return;
        }

        targetDevice = device;
        Debug.Log($"Device name: {device.name} and characteristics: {device.characteristics}");

        GameObject prefab = ControllerPrefabs.Find(controller => controller.name.Contains(device.name));

        if (prefab)
        {
            spawnedController = Instantiate(prefab, transform);
        }
        else
        {
            Debug.LogError("Did not find controller model");
            spawnedController = ControllerPrefabs.FirstOrDefault();
        }
        spawnedHandModel = Instantiate(handModelPrefab, transform);
        handAnimator = spawnedHandModel.GetComponent<Animator>();
    }

    void UpdateHandAnimation()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    void Update()
    {
        if(!targetDevice.isValid)
        { 
            TryInitialize();
            return;
        }

        spawnedHandModel.SetActive(!showController);
        spawnedController.SetActive(showController);

        if(!showController)
        {
            UpdateHandAnimation();
        }
    }
}
