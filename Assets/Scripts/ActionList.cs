using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.AI;

public class ActionList : MonoBehaviour
{
  public void Move(IPerson person, RaycastHit hit)
  {
    person.Agent.destination = hit.point;
    person.Task = TaskList.Moving;
    Debug.Log("Moving");
  }

  public void Harvest(IPerson person, RaycastHit hit)
  {
    person.Agent.destination = hit.collider.gameObject.transform.position;
    person.Task = TaskList.Gathering;
    person.TargetNode = hit.collider.gameObject;
    Debug.Log("Getting");
  }
}
