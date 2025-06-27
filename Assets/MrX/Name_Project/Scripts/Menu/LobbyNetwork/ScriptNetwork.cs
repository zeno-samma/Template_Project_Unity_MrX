using UnityEngine;
using UnityEngine.UI;

namespace MrX.Name_Project
{
    public class ScriptNetwork : Panel
    {

        [SerializeField] private Button closeButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private GameObject Network_Off;

        public override void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            closeButton.onClick.AddListener(ClosePanel);
            joinButton.onClick.AddListener(ClosePanel);
            base.Initialize();
        }

        private void ClosePanel()
        {
            Close();
        }
    }
}



