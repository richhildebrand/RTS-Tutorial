﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class InputManager : MonoBehaviour
{
  public float panSpeed;
  public float rotateSpeed;
  public float rotateAmount;
  public GameObject selectedObject;

  private Quaternion rotation;
  private ObjectInfo selectedInfo;
  private float panDetect = -15f;
  private float minHeight = 10f;
  private float maxHeight = 100f;

  // Start is called before the first frame update
  void Start()
  {
    rotation = Camera.main.transform.rotation;
  }

  // Update is called once per frame
  void Update()
  {
    MoveCamera();
    RotateCamera();

    if (Input.GetMouseButtonDown(0))
    {
      LeftClick();
    }

    if (Input.GetKeyDown(KeyCode.Space))
    {
      Camera.main.transform.rotation = rotation;  
    }
  }

  public void LeftClick()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, 100))
    {
      if (hit.collider.tag == "Ground")
      {
        if (selectedInfo != null)
        {
          selectedInfo.isSelected = false;
        }

        Debug.Log("Deselected");
      }
      else if(hit.collider.tag == "Selectable")
      {
        selectedObject = hit.collider.gameObject;
        selectedInfo = selectedObject.GetComponent<ObjectInfo>();

        selectedInfo.isSelected = true;
        Debug.Log("Selected " + selectedInfo.objectName);
      }
    }
  }

  void MoveCamera()
  {
    float moveX = Camera.main.transform.position.x; 
    float moveY = Camera.main.transform.position.y; 
    float moveZ = Camera.main.transform.position.z;

    float xPos = Input.mousePosition.x;
    float yPos = Input.mousePosition.y;

    if (Input.GetKey(KeyCode.A) || xPos > 0 && xPos < panDetect)
    {
      moveX = moveX - panSpeed;
    }
    else if (Input.GetKey(KeyCode.D) || xPos < Screen.width && xPos > Screen.width - panDetect)
    {
      moveX = moveX + panSpeed;
    }

    if (Input.GetKey(KeyCode.W) || yPos < Screen.height && yPos > Screen.height - panDetect)
    {
      moveZ = moveZ + panSpeed;
    }
    else if (Input.GetKey(KeyCode.S) || yPos > 0 && yPos < panDetect)
    {
      moveZ = moveZ - panSpeed;
    }

    moveY = moveY - Input.GetAxis("Mouse ScrollWheel") * (panSpeed * 20);
    moveY = Mathf.Clamp(moveY, minHeight, maxHeight);

    Vector3 newPosition = new Vector3(moveX, moveY, moveZ);
    Camera.main.transform.position = newPosition; 
  }

  void RotateCamera()
  {
    Vector3 origin = Camera.main.transform.eulerAngles;
    Vector3 destination = origin;

    if (Input.GetMouseButton(2))
    {
      destination.x = destination.x - Input.GetAxis("Mouse Y") * rotateAmount;
      destination.y = destination.y + Input.GetAxis("Mouse X") * rotateAmount;
    }

    if (destination != origin)
    {
      Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
    }
  }
}