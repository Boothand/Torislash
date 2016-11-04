//using System.Collections;
using UnityEngine;

public class IKTarget
{
	public Vector3 position;
	public Quaternion rotation;

	public float speed = 1;

	public static void InterpolateIK(IKTarget[] oldTargets, IKTarget[] newTargets, bool reverse = false)
	{
		for (int i = 0; i < oldTargets.Length; i++)
		{
			oldTargets[i].position = Vector3.MoveTowards(
				oldTargets[i].position,
				newTargets[i].position,
				Time.deltaTime * newTargets[i].speed);

			oldTargets[i].rotation = Quaternion.Lerp(
				oldTargets[i].rotation,
				newTargets[i].rotation,
				Time.deltaTime * newTargets[i].speed);
		}

		//Debug.Log(oldTargets[4].position.z + " vs " + newTargets[4].position.z);
	}
}