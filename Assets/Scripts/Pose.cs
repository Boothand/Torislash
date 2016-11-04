//using System.Collections;
using UnityEngine;
using RootMotion.FinalIK;

public class Pose
{
	protected FullBodyBipedIK ik;

	public PoseMuscle[] muscles { get; protected set; }
	protected IKTarget[] ikTargets;
	public Transform rigObject { get; protected set; }

	protected Transform ikTargetObject;

	protected int ikCount;
	

	public Pose(int muscleCount, Transform inIKTargetObject)
	{
		muscles = new PoseMuscle[muscleCount];

		for (int i = 0; i < muscleCount; i++)
		{
			muscles[i] = new PoseMuscle();
		}

		ikTargetObject = inIKTargetObject;
		ikCount = inIKTargetObject.childCount;

		ikTargets = new IKTarget[ikCount];

		for (int i = 0; i < ikCount; i++)
		{
			ikTargets[i] = new IKTarget();
		}

	}

	public virtual void SavePose()
	{
		
	}

	public void SaveIKs()
	{
		for (int i = 0; i < ikTargets.Length; i++)
		{
			ikTargets[i].position = ikTargetObject.GetChild(i).position;
			ikTargets[i].rotation = ikTargetObject.GetChild(i).rotation;
		}
	}

	public virtual void LoadPose(Pose newPose = null)
	{

	}

	public void LoadIKs(IKTarget[] newTargets = null)
	{

		if (newTargets != null)
		{

			for (int i = 0; i < ikTargets.Length; i++)
			{
				ikTargets[i].position = newTargets[i].position;
				ikTargets[i].rotation = newTargets[i].rotation;
			}
		}

		for (int i = 0; i < ikTargets.Length; i++)
		{
			ikTargetObject.GetChild(i).position = ikTargets[i].position;
			ikTargetObject.GetChild(i).rotation = ikTargets[i].rotation;
		}
	}

	public IKTarget[] GetIKTargets()
	{
		return ikTargets;
	}

	public void MoveIKsToMesh()
	{
		if (ik)
		{
			for (int i = 0; i < ik.solver.effectors.Length; i++)
			{
				if (ik.solver.effectors[i].target)
				{
					ik.solver.effectors[i].target.position = ik.solver.effectors[i].bone.position;
					ik.solver.effectors[i].target.rotation = ik.solver.effectors[i].bone.rotation;
				}
			}
		}
	}
}