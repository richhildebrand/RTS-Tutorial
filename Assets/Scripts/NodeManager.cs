using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
  public enum ResourceTypes { Wood }

  public ResourceTypes resourceType;
  public float harvestTime;
  public float avilableResource;
  public int gatherers;

  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine(ResourceTick());
  }

  // Update is called once per frame
  void Update()
  {
    if (avilableResource <= 0)
    {
      Destroy(gameObject);
    }
  }

  void ResourceGather()
  {
    avilableResource = avilableResource - gatherers;
  }

  IEnumerator ResourceTick()
  {
    while (true)
    {
      yield return new WaitForSeconds(1);
      ResourceGather();
    }
  }
}
