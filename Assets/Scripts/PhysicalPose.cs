//using System.Collections;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.FinalIK;

public class PhysicalPose : Pose
{
	PuppetMaster puppet;


	public PhysicalPose(int muscleCount, GameObject inPhysicalPoseObj, Transform inIKTargetObject, PuppetMaster inPuppet) : base(muscleCount, inIKTargetObject)
	{
		rigObject = inPhysicalPoseObj.transform;
		puppet = inPuppet;
		ik = rigObject.GetComponent<FullBodyBipedIK>();
	}

	public override void SavePose()
	{
		for (int i = 0; i < muscles.Length; i++)
		{
			muscles[i].musclePos = puppet.muscles[i].transform.position;
			muscles[i].muscleRot = puppet.muscles[i].transform.rotation;
			muscles[i].muscleVelocity = puppet.muscles[i].rigidbody.velocity;
		}

		base.SavePose();
	}

	public override void LoadPose(Pose newPose = null)
	{
		Pose poseToUse = this;

		if (newPose != null)
		{
			poseToUse = newPose;
		}

		for (int i = 0; i < muscles.Length; i++)
		{
			puppet.muscles[i].transform.position = poseToUse.muscles[i].musclePos;
			puppet.muscles[i].transform.rotation = poseToUse.muscles[i].muscleRot;
			puppet.muscles[i].rigidbody.velocity = poseToUse.muscles[i].muscleVelocity;
		}

		base.LoadPose();
	}
}