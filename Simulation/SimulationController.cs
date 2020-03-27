using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MysteryPlanet.Simulation
{
	public class SimulationController : MonoBehaviour
	{
		private void Start()
		{
			this._simBody = base.gameObject.GetAttachedOWRigidbody(false);
			this._planetBody = Locator.GetAstroObject(AstroObject.Name.InvisiblePlanet).GetOWRigidbody();
			this._playerCamEffectController = Locator.GetPlayerCamera().GetComponent<PlayerCameraEffectController>();
			this._entrySocket = MakeSimulationBody.simSpawn.transform;
		}

		public void EnterSimulation()
		{
			if (this._enteringSim || this._exitingSim)
			{
				return;
			}

			OWInput.ChangeInputMode(InputMode.None);
			Locator.GetToolModeSwapper().UnequipTool();
			this._playerCamEffectController.CloseEyes(2f);
			this._enteringSim = true;
			this._enterSimTime = Time.time + 2f + Time.deltaTime;
		}

		public void ExitSimulation()
		{
			if (this._enteringSim || this._exitingSim)
			{
				return;
			}

			OWInput.ChangeInputMode(InputMode.None);
			Locator.GetFlashlight().TurnOff(false);
			this._playerCamEffectController.CloseEyes(0.5f);
			this._exitingSim = true;
			this._exitSimTime = Time.time + 0.5f + Time.deltaTime;
		}

		private void FixedUpdate()
		{
			if (this._enteringSim && Time.time > this._enterSimTime)
			{
				Locator.GetPlayerSuit().RemoveSuit(true);
				this._enteringSim = false;
				this._playerCamEffectController.OpenEyes(2f, false);
				this._insideSim = true;
				this._localSleepPos = this._planetBody.transform.InverseTransformPoint(Locator.GetPlayerTransform().position);
				this._localSleepRot = Quaternion.Inverse(this._planetBody.transform.rotation) * Locator.GetPlayerTransform().rotation;
				Locator.GetFlashlight().TurnOff(false);
				Locator.GetPlayerBody().WarpToPositionRotation(this._entrySocket.position, this._entrySocket.rotation);
				Locator.GetPlayerBody().SetVelocity(this._simBody.GetPointVelocity(this._entrySocket.position));
				GlobalMessenger.FireEvent("EnterDreamWorld");
				OWInput.ChangeInputMode(InputMode.Character);
			}
			else if (this._exitingSim && Time.time > this._exitSimTime)
			{
				Locator.GetPlayerSuit().SuitUp(false, true);
				this._exitingSim = false;
				this._playerCamEffectController.OpenEyes(0.5f, false);
				this._insideSim = false;
				Locator.GetPlayerBody().WarpToPositionRotation(this._planetBody.transform.TransformPoint(this._localSleepPos), this._planetBody.transform.rotation * this._localSleepRot);
				Locator.GetPlayerBody().SetVelocity(this._planetBody.GetPointVelocity(this._planetBody.transform.TransformPoint(this._localSleepPos)));
				GlobalMessenger.FireEvent("ExitDreamWorld");
				OWInput.ChangeInputMode(InputMode.Character);
			}
		}

		private Transform _entrySocket;

		private OWRigidbody _simBody;

		private OWRigidbody _planetBody;

		private PlayerCameraEffectController _playerCamEffectController;

		private bool _enteringSim;

		private bool _exitingSim;

		private bool _insideSim;

		private float _enterSimTime;

		private float _exitSimTime;

		private Vector3 _localSleepPos;

		private Quaternion _localSleepRot;
	}

}
