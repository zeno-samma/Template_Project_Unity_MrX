using UnityEngine;


namespace MrX.Name_Project
{
    public class ReturnToMyPool : MonoBehaviour
    {
        public MyPool pool;
        public void OnDisable()
        {
            pool.AddToPool(gameObject);
        }
    }
}