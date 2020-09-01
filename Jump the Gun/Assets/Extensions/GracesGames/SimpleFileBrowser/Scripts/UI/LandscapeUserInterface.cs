using UnityEngine.UI;

using UIUtilities = GracesGames.Common.Scripts.Utilities;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

    public class LandscapeUserInterface : UserInterface {

        protected override void SetupParents() {
            // Find directories parent to group directory buttons
            DirectoriesParent = UIUtilities.FindGameObjectOrError("Directories");
            // Find files parent to group file buttons
            FilesParent = UIUtilities.FindGameObjectOrError("Files");
            // Set the button height
            SetButtonParentHeight(DirectoriesParent, ItemButtonHeight);
            SetButtonParentHeight(FilesParent, ItemButtonHeight);
            // Set the panel color
            UIUtilities.FindGameObjectOrError("DirectoryPanel").GetComponent<Image>().color = DirectoryPanelColor;
            UIUtilities.FindGameObjectOrError("FilePanel").GetComponent<Image>().color = FilePanelColor;
        }
    }
}

