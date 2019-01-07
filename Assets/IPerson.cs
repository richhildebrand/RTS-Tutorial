using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets
{
  public interface IPerson
  {
    NavMeshAgent Agent { get; set; }
    TaskList Task { get; set; }
    GameObject TargetNode { get; set; }
  }
}
