using System.Numerics;
using UnityEngine;


namespace MrX.Name_Project
{
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] private Texture2D cursorNormal;
        [SerializeField] private Texture2D cursorShoot;
        [SerializeField] private Texture2D cursorReload;
        private UnityEngine.Vector2 hotspot = new UnityEngine.Vector2(16, 48);

        void Start()
        {
            Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(cursorShoot, hotspot, CursorMode.Auto);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.SetCursor(cursorReload, hotspot, CursorMode.Auto);
            }
            else if (Input.GetMouseButtonUp(1))
            {
               Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto); 
            }
        }
    }

}
