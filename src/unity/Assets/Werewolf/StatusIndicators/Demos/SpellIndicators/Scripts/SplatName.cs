using UnityEngine;
using Werewolf.StatusIndicators.Components;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Werewolf.StatusIndicators.Demo {
  public class SplatName : MonoBehaviour {
    void OnEnable() {
      GetComponent<Text>().text = "";
    }
  }
}