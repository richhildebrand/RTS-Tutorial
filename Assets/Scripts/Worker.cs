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
  public bool isGather = false;

  private ActionList actionList;
  private GameObject[] drops;
  public float distanceToTarget;
  private Vector3 offset;

  //IPerson
  public NavMeshAgent Agent { get; set; }
  public TaskList Task { get; set; }
  public GameObject TargetNode { get; set; }

  void Start()
  {
    StartCoroutine(GatherTick());
    Agent = GetComponent<NavMeshAgent>();
    actionList = FindObjectOfType<ActionList>();

    GetComponent<NavMeshObstacle>().enabled = false;
    GetComponent<NavMeshAgent>().enabled = true;
  }

  void Update()
  {
    if (TargetNode != null)
    {
      
      if (Task == TaskList.Gathering)
      {
        //distanceToTarget = Vector3.Distance(transform.position, TargetNode.transform.position);
        offset = TargetNode.transform.position - transform.position;
        distanceToTarget = offset.sqrMagnitude;
        if (distanceToTarget <= 2.5f * 2.5f)
        {
          Gather();
        }
      }
      else if (Task == TaskList.Delivering)
      {
        drops = GameObject.FindGameObjectsWithTag("Drops");
        var closestDropOff = GetClosestDropoff(drops).transform.position;
        //distanceToTarget = Vector3.Distance(closestDropOff, transform.position);
        offset = closestDropOff - transform.position;
        distanceToTarget = offset.sqrMagnitude;
        if (distanceToTarget <= 2.5f * 2.5f)
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
            isGather = false;
          }
        }
      }
    }
    else
    {
      if (heldResource > 0)
      {
        drops = GameObject.FindGameObjectsWithTag("Drops");
        var closestDropOff = GetClosestDropoff(drops).transform.position;
        Agent.destination = closestDropOff;
        //distanceToTarget = Vector3.Distance(closestDropOff, transform.position);
        offset = closestDropOff - transform.position;
        distanceToTarget = offset.sqrMagnitude;
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
      TargetNode.GetComponent<NodeManager>().gatherers--;
      isGathering = false;
      GetComponent<NavMeshObstacle>().enabled = false;
      GetComponent<NavMeshAgent>().enabled = true;

      drops = GameObject.FindGameObjectsWithTag("Drops");
      var closestDropOff = GetClosestDropoff(drops).transform.position;
      Agent.destination = closestDropOff;
      //distanceToTarget = Vector3.Distance(closestDropOff, transform.position);
      offset = closestDropOff - transform.position;
      distanceToTarget = offset.sqrMagnitude;
      drops = null;

      Task = TaskList.Delivering;
      Debug.Log("now " + TaskList.Delivering);
    }

    if (Input.GetMouseButtonDown(1) && GetComponent<ObjectInfo>().isSelected)
    {
      RightClick();
    }
  }

  public void Gather()
  {
    isGathering = true;
    if (!isGather)
    {
      TargetNode.GetComponent<NodeManager>().gatherers++;
      isGather = true; 
    }
    
    heldResourceType = TargetNode.GetComponent<NodeManager>().resourceType;
    GetComponent<NavMeshObstacle>().enabled = true;
    GetComponent<NavMeshAgent>().enabled = false;
  }

  public void RightClick()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, 100))
    {
      if (hit.collider.tag == "Ground")
      {
        if (TargetNode != null && isGathering)
        {
          TargetNode.GetComponent<NodeManager>().gatherers--;
          isGathering = false;
        }

        GetComponent<NavMeshObstacle>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = true;
        actionList.Move(this, hit);
      }
      else if (hit.collider.tag == "Resource")
      {
        actionList.Harvest(this, hit);
        Debug.Log("my task is " + this.Task);
      }
      else if (hit.collider.tag == "Drops")
      {
        TargetNode.GetComponent<NodeManager>().gatherers--;
        isGathering = false;

        drops = GameObject.FindGameObjectsWithTag("Drops");
        var closestDropOff = GetClosestDropoff(drops).transform.position;
        Agent.destination = closestDropOff;
        //distanceToTarget = Vector3.Distance(closestDropOff, transform.position);
        offset = closestDropOff - transform.position;
        distanceToTarget = offset.sqrMagnitude;
        drops = null;

        Task = TaskList.Delivering;
        GetComponent<NavMeshObstacle>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = false;
        Debug.Log("clicked on a drop... " + TaskList.Delivering);
      }
    }
  }


  //public void OnTriggerEnter(Collider other)
  //{
  //  Debug.Log("task -> " + Task);

  //  GameObject hitObject = other.gameObject;
  //  if (hitObject.tag == "Resource" && Task == TaskList.Gathering)
  //  {
  //    isGathering = true;
  //    hitObject.GetComponent<NodeManager>().gatherers++;
  //    heldResourceType = hitObject.GetComponent<NodeManager>().resourceType;
  //  }
  //  else if (hitObject.tag == "Drops" && Task == TaskList.Delivering)
  //  {
  //    if (RM.wood >= RM.maxWood)
  //    {
  //      Task = TaskList.Idle;
  //    }
  //    else
  //    {
  //      RM.wood += heldResource;
  //      heldResource = 0;
  //      Task = TaskList.Gathering;
  //      Agent.destination = TargetNode.transform.position;
  //    }
  //  }
  //}

  public void OnTriggerExit(Collider other) //No longer used
  {
    //GameObject hitObject = other.gameObject;
    //if (hitObject.tag == "Resource")
    //{
    //  isGathering = false;
    //  hitObject.GetComponent<NodeManager>().gatherers--;
    //}
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
