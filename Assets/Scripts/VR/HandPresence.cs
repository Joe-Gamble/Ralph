using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public GameObject handModelPrefab;

    private GameObject spawnedHandModel;
    private InputDevice targetDevice;
    private Animator handAnimator;
    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharactistics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharactistics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }

        spawnedHandModel = Instantiate(handModelPrefab, this.transform);
        handAnimator = spawnedHandModel.GetComponent<Animator>();

    }

    void UpdateAnimation()
    {
        /*
        if (targetDevice.TryGetFeature(CommonUsuages.trigger, out float triggerValue))
        {
            
        }
        */
    }

    // Update is called once per frame
    void Update()
    {

    }
}
