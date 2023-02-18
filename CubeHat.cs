using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CubeHat : MonoBehaviour
{
    [SerializeField]
    private ARFaceManager _faceManager;
    [SerializeField]
    private GameObject _cubeHatPrefab;
    private GameObject _cubeHat;

    public ARFaceManager faceManager;
    public GameObject cube;

    public float yOffset = 0.1f; // adjust as needed
    public float scaleFactor = 0.1f; // adjust as needed

    void Update()
    {
        if (faceManager != null && faceManager.facePrefab != null)
        {
            foreach (ARFace face in faceManager.trackables)
            {
                if (face.trackingState == TrackingState.Tracking)
                {
                    // Get the position of the face's center point
                    Vector3 facePosition = face.transform.position;

                    // Add an offset to the position to place the cube on top of the head
                    Vector3 cubePosition = new Vector3(facePosition.x, facePosition.y + yOffset, facePosition.z);

                    // Set the position of the cube
                    cube.transform.position = cubePosition;

                    // Set the rotation of the cube to match the face's rotation
                    cube.transform.rotation = face.transform.rotation;

                    // Set the scale of the cube to match the distance from the camera
                    float distance = Vector3.Distance(Camera.main.transform.position, cube.transform.position);
                    cube.transform.localScale = Vector3.one * scaleFactor * distance;

                    break; // Only update the position, rotation and scale for one tracked face
                }
            }
        }
    }

    private void OnEnable()
    {
        _faceManager.facesChanged += OnFacesChanged;
    }

    private void OnDisable()
    {
        _faceManager.facesChanged -= OnFacesChanged;
    }

    private void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        if (args.updated.Count > 0)
        {
            ARFace face = args.updated[0];
            if (face.trackingState == TrackingState.Tracking)
            {
                if (_cubeHat == null)
                {
                    _cubeHat = Instantiate(_cubeHatPrefab, face.transform.position, face.transform.rotation);
                    cube = _cubeHat.transform.Find("Cube").gameObject;
                }
                else
                {
                    _cubeHat.transform.position = face.transform.position;
                    _cubeHat.transform.rotation = face.transform.rotation;
                }
            }
            else
            {
                if (_cubeHat != null)
                {
                    Destroy(_cubeHat);
                }
            }
        }
        else if (_cubeHat != null)
        {
            Destroy(_cubeHat);
        }
    }
}
