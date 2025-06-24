using System.Collections.Generic;
using UnityEngine;

namespace MrX.Name_Project
{
    public class MyPool
    {
        private Stack<GameObject> stack = new Stack<GameObject>();
        private GameObject baseObj;
        private GameObject tmp;
        private ReturnToMyPool returnPool;

        public MyPool(GameObject baseObj)
        {
            this.baseObj = baseObj;
        }

        public GameObject Get(bool activeValue)
        {
            if (baseObj == null)
            {
                // Debug.LogError("Base object is null, cannot get from pool.");
                return null;
            }
            while (stack.Count > 0)
            {
                // Debug.Log("Kiểm tra và lấy ra");
                tmp = stack.Pop();//
                if (tmp != null)
                {
                    tmp.SetActive(activeValue);
                    // Debug.Log("Enemy: " + tmp.name);
                    return tmp;
                }
                else
                {
                    // Debug.LogWarning($"game object with key {baseObj.name} has been destroyed!");
                }
            }
            tmp = GameObject.Instantiate(baseObj, new Vector3(6f, -1f, 0f), Quaternion.identity);
            returnPool = tmp.AddComponent<ReturnToMyPool>();
            returnPool.pool = this;
            return tmp;
        }
        public void AddToPool(GameObject obj)
        {
            stack.Push(obj);// add the object to the pool
        }
    }

}
