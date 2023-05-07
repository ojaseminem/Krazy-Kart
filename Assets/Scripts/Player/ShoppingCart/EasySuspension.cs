using UnityEngine;

namespace Player.ShoppingCart
{
	public class EasySuspension : MonoBehaviour {
		[Range(0, 20)]
		public float naturalFrequency = 10;
		[Range(0, 3)]
		public float dampingRatio = 0.8f;
		[Range(-1, 1)]
		public float forceShift = 0.03f;
		public bool setSuspensionDistance = true;

		private void Update () {
			foreach (WheelCollider wc in GetComponentsInChildren<WheelCollider>()) {
				JointSpring spring = wc.suspensionSpring;

				spring.spring = Mathf.Pow(Mathf.Sqrt(wc.sprungMass) * naturalFrequency, 2);
				spring.damper = 2 * dampingRatio * Mathf.Sqrt(spring.spring * wc.sprungMass);

				wc.suspensionSpring = spring;

				Vector3 wheelRelativeBody = transform.InverseTransformPoint(wc.transform.position);
				float distance = GetComponent<Rigidbody>().centerOfMass.y - wheelRelativeBody.y + wc.radius;

				wc.forceAppPointDistance = distance - forceShift;

				if (spring.targetPosition > 0 && setSuspensionDistance)
					wc.suspensionDistance = wc.sprungMass * Physics.gravity.magnitude / (spring.targetPosition * spring.spring);
			}
		}
	}
}
