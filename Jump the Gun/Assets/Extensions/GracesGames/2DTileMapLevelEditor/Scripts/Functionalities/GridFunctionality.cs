using UnityEngine;
using UnityEngine.UI;

using UIUtilities = GracesGames.Common.Scripts.Utilities;

namespace GracesGames._2DTileMapLevelEditor.Scripts.Functionalities {

	public class GridFunctionality : MonoBehaviour {
		
		// ----- PRIVATE VARIABLES -----

		// UI objects to toggle the grid
		private GameObject _gridEyeImage;

		private GameObject _gridClosedEyeImage;
		private Toggle _gridEyeToggleComponent;
		
		// ----- SETUP -----

		public void Setup(int width, int height) {
			SetupClickListeners();
			// Setup grid overlay
			SetupGridOverlay(width, height);
			// Initialy enable grid
			ToggleGrid(true);
		}

		// Hook up Grid methods to Grid button
		private void SetupClickListeners() {
			// Hook up ToggleGrid method to GridToggle
			GameObject gridEyeToggle = UIUtilities.FindGameObjectOrError("GridEyeToggle");
			_gridEyeImage = UIUtilities.FindGameObjectOrError("GridEyeImage");
			_gridClosedEyeImage = UIUtilities.FindGameObjectOrError("GridClosedEyeImage");
			_gridEyeToggleComponent = gridEyeToggle.GetComponent<Toggle>();
			_gridEyeToggleComponent.onValueChanged.AddListener(ToggleGrid);

			// Hook up Grid Size methods to Grid Size buttons
			UIUtilities.FindButtonAndAddOnClickListener("GridSizeUpButton", GridOverlay.Instance.GridSizeUp);
			UIUtilities.FindButtonAndAddOnClickListener("GridSizeDownButton", GridOverlay.Instance.GridSizeDown);

			// Hook up Grid Navigation methods to Grid Navigation buttons
			UIUtilities.FindButtonAndAddOnClickListener("GridUpButton", GridOverlay.Instance.GridUp);
			UIUtilities.FindButtonAndAddOnClickListener("GridDownButton", GridOverlay.Instance.GridDown);
			UIUtilities.FindButtonAndAddOnClickListener("GridLeftButton", GridOverlay.Instance.GridLeft);
			UIUtilities.FindButtonAndAddOnClickListener("GridRightButton", GridOverlay.Instance.GridRight);
		}

		// Define the level sizes as the sizes for the grid
		private void SetupGridOverlay(int width, int height) {
			GridOverlay.Instance.SetGridSizeX(width);
			GridOverlay.Instance.SetGridSizeY(height);
		}
		
		// ----- PRIVATE METHODS -----

		// Method that toggles the grid
		private void ToggleGrid(bool enable) {
			GridOverlay.Instance.enabled = enable;
			// Update UI 
			_gridEyeImage.SetActive(!enable);
			_gridClosedEyeImage.SetActive(enable);
			_gridEyeToggleComponent.targetGraphic =
				enable ? _gridClosedEyeImage.GetComponent<Image>() : _gridEyeImage.GetComponent<Image>();
		}
	}
}