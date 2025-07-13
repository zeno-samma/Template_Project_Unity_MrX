using UnityEngine;


namespace MrX.Name_Project
{
    public class ReturnToMyPool : MonoBehaviour
    {
        public MyPool pool;
        // public void Return()
        // {
        //     if (pool != null)
        //     {
        //         pool.AddToPool(this.gameObject);
        //         this.gameObject.SetActive(false);
        //     }
        // }
        public void OnDisable()
        {
            pool.AddToPool(gameObject);
        }
    }
}