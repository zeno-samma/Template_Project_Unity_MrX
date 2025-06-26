using UnityEngine;
using UnityEngine.UI;

namespace MrX.Name_Project
{
    public class ScriptNetwork : MonoBehaviour
    {

        [SerializeField] private Button Button;
        [SerializeField] private GameObject Network_Off;
        

        public void BtnClose()
        {
            Network_Off.SetActive(false);
        }
    }
}



