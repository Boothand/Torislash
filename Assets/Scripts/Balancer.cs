//using System.Collections;
using UnityEngine;

public class Balancer : MonoBehaviour
{
	Rigidbody rb;

	[SerializeField]
	Transform[] referencePoints = new Transform[2];

	[SerializeField]
	float strength = 10f;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}

	Vector3 AveragePos()
	{
		Vector3 pos = Vector3.zero;

		foreach (Transform obj in referencePoints)
		{
			pos += obj.position;
		}

		return pos /= referencePoints.Length;
	}
	
	void Update ()
	{
		Vector3 centerPoint = Vector3.zero;

		if (referencePoints.Length > 0)
		{
			centerPoint = AveragePos();
			centerPoint.y = referencePoints[0].position.y + 1;

			Vector3 delta = centerPoint - transform.position;
		
			rb.AddForce((delta.normalized * strength) + Vector3.up * 1f, ForceMode.Impulse);

			Debug.DrawLine(transform.position, centerPoint, Color.red);
		}
	}
}