using UnityEngine;
using System.Collections;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components {
	public class RangeIndicator : Splat {
		public override ScalingType Scaling { get { return ScalingType.LengthAndHeight; } }

		public float DefaultScale;

		public override void OnShow() {
			UpdateRangeIndicatorSize();
		}

		/// <summary>
		/// Scale Range Indicator back to default
		/// </summary>
		private void UpdateRangeIndicatorSize() {
			Scale = DefaultScale;
		}
	}
}