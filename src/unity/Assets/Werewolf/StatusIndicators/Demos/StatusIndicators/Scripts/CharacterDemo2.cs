using UnityEngine;
using Werewolf.StatusIndicators.Components;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Werewolf.StatusIndicators.Demo {
	public class CharacterDemo2 : MonoBehaviour {
		public SplatManager Splats { get; set; }

		private int index;

		void Start() {
			Splats = GetComponentInChildren<SplatManager>();
			Splats.SelectStatusIndicator(Splats.StatusIndicators[0].name);
			UpdateSelection();
		}

		void Update() {
			if(Input.GetMouseButtonDown(0)) {
				Splats.CancelStatusIndicator();
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				index = (int)Mathf.Repeat(index - 1, Splats.StatusIndicators.Length);
				UpdateSelection();
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)) {
				index = (int)Mathf.Repeat(index + 1, Splats.StatusIndicators.Length);
				UpdateSelection();
			}
		}

		private void UpdateSelection() {
			Splats.SelectStatusIndicator(Splats.StatusIndicators[index].name);
			GameObject.FindObjectOfType<SplatName>().GetComponent<Text>().text = index + ": " + Splats.CurrentStatusIndicator.name;
		}
	}
}