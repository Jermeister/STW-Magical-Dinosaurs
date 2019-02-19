using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMovement : MonoBehaviour
{

    Rigidbody rb;

    float lookSensitivity = 50;
    public float speed = 100f;

    float rotationX = 0.0f;
    float rotationY = 0.0f;

    public float minX, maxX, minZ, maxZ, minY, maxY;
    public float mult;
    public Vector3 lastPosition;

    public bool needsLock;
    public float zoomspeed;

    void Start()
    {
        

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * zoomspeed * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel"));

        needsLock = false;
        if (Input.GetMouseButton(2))
        {
            //needsLock = true;
            transform.Translate(-(Input.mousePosition - lastPosition) * mult);
        }
        lastPosition = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            needsLock = true;
            //Cursor.lockState = CursorLockMode.Locked;
            rotationX += Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

            rb.velocity = Vector3.zero;
            transform.Translate(new Vector3(Vector3.forward.x, 0f, Vector3.forward.z) * Input.GetAxis("Vertical") * Time.deltaTime * speed);
            transform.Translate(new Vector3(Vector3.right.x, 0f, Vector3.right.z) * Input.GetAxis("Horizontal") * Time.deltaTime * speed);

           

            
        }

        if(needsLock)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;



        if (transform.position.x >= maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        if (transform.position.x <= -minX)
        {
            transform.position = new Vector3(-minX, transform.position.y, transform.position.z);
        }

        if (transform.position.y >= maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
        if (transform.position.y <= -minY)
        {
            transform.position = new Vector3(transform.position.x, -minY, transform.position.z);
        }

        if (transform.position.z >= maxZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }
        if (transform.position.z <= -minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -minZ);
        }

    }



}