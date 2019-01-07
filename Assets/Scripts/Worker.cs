using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour, IPerson
{
  public NodeManager.ResourceTypes heldResourceType;
  public ResourceManager RM;
  public int heldResource;
  public int maxHeldResource;
  public bool isGathering = false;

  private ActionList actionList;
  private GameObject[] drops;

  //IPerson
  public NavMeshAgent Agent { get; set; }
  public TaskList Task { get; set; }
  public GameObject TargetNode { get; set; }

  void Start()
  {
    StartCoroutine(GatherTick());
    Agent = GetComponent<NavMeshAgent>();
    actionList = FindObjectOfType<ActionList>();
  }

  void Update()
  {
    if (TargetNode == null)
    {
      if (heldResource > 0)
      {
        drops = GameObject.FindGameObjectsWithTag("Drops");
        Agent.destination = GetClosestDropoff(drops).transform.position;
        drops = null;

        Task = TaskList.Delivering;
      }
      else
      {
        Task = TaskList.Idle;
      }
    }

    if (heldResource >= maxHeldResource)
    {
      drops = GameObject.FindGameObjectsWithTag("Drops");
      Agent.destination = GetClosestDropoff(drops).transform.position;
      drops = null;

      Task = TaskList.Delivering;
    }

    if (Input.GetMouseButtonDown(1) && GetComponent<ObjectInfo>().isSelected)
    {
      RightClick();
    }
  }

  public void RightClick()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, 100))
    {
      if (hit.collider.tag == "Ground")
      {
        actionList.Move(this, hit);
      }
      else if (hit.collider.tag == "Resource")
      {
        actionList.Harvest(this, hit);
        Debug.Log("my task is " + this.Task);
      }
    }
  }


  public void OnTriggerEnter(Collider other)
  {
    Debug.Log("task -> " + Task);

    GameObject hitObject = other.gameObject;
    if (hitObject.tag == "Resource" && Task == TaskList.Gathering)
    {
      isGathering = true;
      hitObject.GetComponent<NodeManager>().gatherers++;
      heldResourceType = hitObject.GetComponent<NodeManager>().resourceType;
    }
    else if (hitObject.tag == "Drops" && Task == TaskList.Delivering)
    {
      if (RM.wood >= RM.maxWood)
      {
        Task = TaskList.Idle;
      }
      else
      {
        RM.wood += heldResource;
        heldResource = 0;
        Task = TaskList.Gathering;
        Agent.destination = TargetNode.transform.position;
      }
    }
  }

  public void OnTriggerExit(Collider other)
  {
    GameObject hitObject = other.gameObject;
    if (hitObject.tag == "Resource")
    {
      isGathering = false;
      hitObject.GetComponent<NodeManager>().gatherers--;
    }
  }

  GameObject GetClosestDropoff(GameObject[] dropOffs)
  {
    GameObject closestDrop = null;
    float closestDistance = Mathf.Infinity;
    Vector3 postion = transform.position;

    foreach (var targetDropOff in dropOffs)
    {
      Vector3 direction = targetDropOff.transform.position - postion;
      float distance = direction.sqrMagnitude;

      if (distance < closestDistance)
      {
        closestDistance = distance;
        closestDrop = targetDropOff;
      }
    }

    return closestDrop;
  }

  IEnumerator GatherTick()
  {
    while (true)
    {
      yield return new WaitForSeconds(1);
      if (isGathering)
      {
        heldResource++;
      }
    }
  }
}
