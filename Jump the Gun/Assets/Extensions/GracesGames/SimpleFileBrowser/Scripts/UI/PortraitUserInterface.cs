using UnityEngine.UI;

using UIUtilities = GracesGames.Common.Scripts.Utilities;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

	public class PortraitUserInterface : UserInterface {

		protected override void SetupParents() {
			// Find directories parent to group directory buttons
			DirectoriesParent = UIUtilities.FindGameObjectOrError("Items");
			// Find files parent to group file buttons
			FilesParent = UIUtilities.FindGameObjectOrError("Items");
			// Set the button height
			SetButtonParentHeight(DirectoriesParent, ItemButtonHeight);
			SetButtonParentHeight(FilesParent, ItemButtonHeight);
			// Set the panel color
			UIUtilities.FindGameObjectOrError("ItemPanel").GetComponent<Image>().color = DirectoryPanelColor;
		}
	}
}
