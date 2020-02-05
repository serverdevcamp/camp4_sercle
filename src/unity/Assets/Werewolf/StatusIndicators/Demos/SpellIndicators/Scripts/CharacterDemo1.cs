using UnityEngine;
using Werewolf.StatusIndicators.Components;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace Werewolf.StatusIndicators.Demo {
	public class CharacterDemo1 : MonoBehaviour {
		public SplatManager Splats { get; set; }

		private int index;

		void Start() {
			Splats = GetComponentInChildren<SplatManager>();
		}

		void Update() {
			if(Input.GetMouseButtonDown(0)) {
				Splats.CancelSpellIndicator();
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				index = (int)Mathf.Repeat(index - 1, Splats.SpellIndicators.Length);
				UpdateSelection();
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)) {
				index = (int)Mathf.Repeat(index + 1, Splats.SpellIndicators.Length);
				UpdateSelection();
			}
		}

		private void UpdateSelection() {
			Splats.SelectSpellIndicator(Splats.SpellIndicators[index].name);
			GameObject.FindObjectOfType<SplatName>().GetComponent<Text>().text = index + ": " + Splats.CurrentSpellIndicator.name;
		}
	}
}