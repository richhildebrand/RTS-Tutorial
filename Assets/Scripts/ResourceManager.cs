using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
  public float wood;
  public float maxWood;
  public Text woodDisplay;

  public float rock;
  public float maxRock;
  public Text rockDisplay;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    woodDisplay.text = "" + wood + "/" + maxWood;
    rockDisplay.text = "" + rock + "/" + maxRock;
  }
}
