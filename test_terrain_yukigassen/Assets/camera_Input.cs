using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_Input : MonoBehaviour {

    public enum RotationAxis
    {
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxis axis = RotationAxis.MouseX;

    public float minimumvert = -45.0f;
    public float maximumvert = 45.0f;

    public float sensHorizontal = 10.0f;
    public float sensVertical = 10.0f;

    public float _rotationX = 0;

    // Update is called once per frame
    void Update()
    {

        if (axis == RotationAxis.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensHorizontal, 0);
        }
        else if (axis == RotationAxis.MouseY)
        {
            _rotationX -= Input.GetAxis("Mouse Y") * sensVertical;
            _rotationX = Mathf.Clamp(_rotationX, minimumvert, maximumvert);

            float rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
    }
}
