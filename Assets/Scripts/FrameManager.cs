using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FrameManager : MonoBehaviour
{
	[SerializeField]
	PuppetMaster puppet;

	[SerializeField]
	GameObject referenceRigObj;

	[SerializeField]
	GameObject physicalRigObj;

	[SerializeField]
	Transform referenceIKObject;

	[SerializeField]
	Transform physicalIKObject;

	PhysicalPose physicalPose;
	ReferencePose referencePose;

	int muscleCount;
	int ikTargetCount;

	Server server;

	IKTarget[] roundStartTargets;
	IKTarget[] physicalIKs;
	IKTarget[] referenceIKs;

	public enum State
	{
		Start,
		Posing,
		Ready,
		Simulate
	}

	public State state { get; private set; }

	void Start ()
	{
		ikTargetCount = referenceIKObject.childCount;

		physicalPose = new PhysicalPose(puppet.muscles.Length, physicalRigObj, physicalIKObject, puppet);
		referencePose = new ReferencePose(referenceRigObj, referenceIKObject, puppet);    //Fixme when reference pose

		roundStartTargets = new IKTarget[ikTargetCount];

		server = Server.GetInstance();

		state = State.Start;
	}

	IEnumerator SetPoseRoutine()
	{
		//Reference to physical IK targets:
		physicalIKs = physicalPose.GetIKTargets();

		//Activate the reference mesh so it's visible.
		referenceRigObj.SetActive(true);

		//Always move the IKs to their targets on a new round:
		referencePose.MoveIKsToMesh();
		physicalPose.MoveIKsToMesh();

		//Sample the physical rig.
		physicalPose.SavePose();

		//Store the pos and rot of the physical IK targets for later.
		physicalPose.SaveIKs();

		//Make the reference pose (and IKs) copy the physical pose.
		referencePose.LoadPose(physicalPose);
		referencePose.LoadIKs(physicalPose.GetIKTargets());		

		//Remember the IK pos and rot from beginning of round:
		
		for (int i = 0; i < ikTargetCount; i++)
		{
			roundStartTargets[i] = new IKTarget();
			roundStartTargets[i].position = physicalIKs[i].position;
			roundStartTargets[i].rotation = physicalIKs[i].rotation;
		}


		//3. Set up the reference rig to the physical pose.
		
		//4. Save the reference pose so user can reset to it.

		while (true)
		{
			//1. Remember the position of the reference IKs
			referencePose.SaveIKs();
			referenceIKs = referencePose.GetIKTargets();

			//2. Reset physical pos, rot and velocity to what it was at round-start.
			physicalPose.LoadPose(); // <-- Should reset pose to start values.
			physicalPose.LoadIKs(roundStartTargets);    // <-- Should reset IKs to start values.

			float interpolationTimer = 0f;

			while (interpolationTimer < 1.5f)
			{
				interpolationTimer += Time.deltaTime;

				//Interpolate physical IK pose from initial IK pos/rot to user's IK pos/rot.
				IKTarget.InterpolateIK(physicalIKs, referenceIKs);

				//Update the IKs.
				physicalPose.LoadIKs();
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator SimulateRoutine()
	{
		//1. Reset physical pose.
		physicalPose.LoadPose();
		physicalPose.LoadIKs(roundStartTargets);

		//2. Hide reference pose.
		referenceRigObj.SetActive(false);
		yield return new WaitForEndOfFrame();

		//3. Wait for server to stop counting frames.
		while (server.gameState == Server.GameState.Simulate)
		{
			IKTarget.InterpolateIK(physicalIKs, referenceIKs);
			//Update the IKs.
			physicalPose.LoadIKs();

			//print(physicalIKs[3].position.z + " vs " + referenceIKs[3].position.z);
			yield return new WaitForEndOfFrame();
		}
		//4. Set state to Posing.
	}

	void Update ()
	{
		switch (server.gameState)
		{
			case Server.GameState.Input:

				if (state != State.Posing)
				{
					state = State.Posing;
					StartCoroutine(SetPoseRoutine());
				}

				if (state != State.Ready && Input.GetButtonDown("Ready"))
				{
					state = State.Ready;
					StopAllCoroutines();
				}

				break;
			case Server.GameState.Simulate:

				if (state != State.Simulate)
				{
					state = State.Simulate;
					StartCoroutine(SimulateRoutine());
				}

				break;
		}
	}
}