using System.Collections;
using UnityEngine;

public class Server : MonoBehaviour
{
	FrameManager[] players;

	[SerializeField]
	int frameCount = 300;

	[SerializeField]
	int frameInterval = 10;

	int frame;

	bool simulating;

	public enum GameState
	{
		Input,
		Simulate
	}

	static Server instance;

	public GameState gameState { get; private set; }


	void Start()
	{
		players = Transform.FindObjectsOfType<FrameManager>();

		gameState = GameState.Input;
	}

	public static Server GetInstance()
	{
		if (instance == null)
		{
			instance = Transform.FindObjectOfType<Server>();
		}

		return instance;
	}

	IEnumerator SimulateRoutine()
	{
		int currentFrame = frame;

		while (frame < currentFrame + frameInterval)
		{
			frame += Mathf.CeilToInt(Time.deltaTime * 60f);

			yield return new WaitForEndOfFrame();
		}

		gameState = GameState.Input;
		simulating = false;
	}

	void Update()
	{
		switch (gameState)
		{
			case GameState.Input:

				foreach (FrameManager player in players)
				{
					if (player.state == FrameManager.State.Ready)
					{
						gameState = GameState.Simulate;
					}
				}

				break;

			case GameState.Simulate:

				if (!simulating)
				{
					simulating = true;

					StartCoroutine(SimulateRoutine());
				}

				break;
		}
	}
}