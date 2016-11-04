//using System.Collections;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.FinalIK;

public class ReferencePose : Pose
{
	PuppetMaster puppet;

	public ReferencePose(GameObject inReferenceRig, Transform inIKTargetObject, PuppetMaster inPuppet) : base(50, inIKTargetObject)
	{
		rigObject = inReferenceRig.transform;
		puppet = inPuppet;
		ik = rigObject.GetComponent<FullBodyBipedIK>();
	}

	public override void SavePose()
	{
		//for (int i = 0; i < muscles.Length; i++)
		//{
		//	muscles[i].musclePos = puppet.muscles[i].transform.position;
		//	muscles[i].muscleRot = puppet.muscles[i].transform.rotation;
		//	muscles[i].muscleVelocity = puppet.muscles[i].rigidbody.velocity;
		//}

		base.SavePose();
	}

	void CopyHierarchyRecursive(Transform from, Transform to)
	{
		for (int i = 0; i < to.childCount; i++)
		{
			to.GetChild(i).position = from.GetChild(i).position;
			to.GetChild(i).rotation = from.GetChild(i).rotation;

			if (from.GetChild(i).childCount > 0)
			{
				CopyHierarchyRecursive(from.GetChild(i), to.GetChild(i));
			}
		}
	}

	public override void LoadPose(Pose newPose = null)
	{
		Pose poseToUse = this;
		PhysicalPose physPose = null;

		if (newPose != null)
		{
			poseToUse = newPose;
		}

		if (newPose is PhysicalPose)
		{
			physPose = (PhysicalPose) newPose;
			CopyHierarchyRecursive(physPose.rigObject, rigObject);
		}

		base.LoadPose();
	}

	
}