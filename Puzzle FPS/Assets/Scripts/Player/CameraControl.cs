using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] int sensHor;
    [SerializeField] int sensVer;

    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    [SerializeField] bool invertX;

    float xRotation;
    #region Public Access Methods
    public int HorizontalSensitivity
    {
        get { return sensHor; }
        set { sensHor = value; }
    }
    public int VeritcalSensitivity
    {
        get { return sensVer; }
        set { sensVer = value; }
    }
    public int VeticalLockMin
    {
        get { return lockVerMin; }
        set { lockVerMin = value; }
    }
    public int VeticalLockMax
    {
        get { return lockVerMax; }
        set { lockVerMax = value; }
    }
    public bool InvertX
    {
        get { return invertX; }
        set { invertX = value; }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVer;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;

        if (invertX)
        {
            xRotation += mouseY;
        }
        else
        {
            xRotation -= mouseY;
        }

        //clamp the camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);


        //rotate the camera on the X-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //rotate the player on its Y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    public void ToggleCursorVisibility()
    {
        Cursor.visible = !Cursor.visible;
    }
}