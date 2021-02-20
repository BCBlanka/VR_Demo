using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    private Vector3 initialAttachLocalPosition;
    private Quaternion initialAttachLocalRotation;

    private void Start()
    {
        // create attach point
        if(!attachTransform)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }
        initialAttachLocalPosition = attachTransform.localPosition;
        initialAttachLocalRotation = attachTransform.localRotation;
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if(args.interactor is XRDirectInteractor)
        {
            attachTransform.position = args.interactor.transform.position;
            attachTransform.rotation = args.interactor.transform.rotation;
        }
        else
        {
            attachTransform.position = initialAttachLocalPosition;
            attachTransform.rotation = initialAttachLocalRotation;
        }
        base.OnSelectEntered(args);
    }
}
