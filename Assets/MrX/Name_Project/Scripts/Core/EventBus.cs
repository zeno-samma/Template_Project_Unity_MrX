using System;
using System.Collections.Generic;
using UnityEngine;

namespace MrX.Name_Project
{
    public static class EventBus
    {
        // Dùng Dictionary để lưu tất cả các listener cho từng loại Event
        private static Dictionary<Type, Delegate> s_events = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Đăng ký lắng nghe một loại sự kiện cụ thể.
        /// </summary>
        /// <typeparam name="T">Loại sự kiện để lắng nghe.</typeparam>
        /// <param name="listener">Hàm sẽ được gọi khi sự kiện xảy ra.</param>
        public static void Subscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (s_events.TryGetValue(eventType, out Delegate d))
            {
                s_events[eventType] = Delegate.Combine(d, listener);
            }
            else
            {
                s_events[eventType] = listener;
            }
        }

        /// <summary>
        /// Hủy đăng ký lắng nghe một loại sự kiện.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (s_events.TryGetValue(eventType, out Delegate d))
            {
                Delegate newDelegate = Delegate.Remove(d, listener);
                if (newDelegate == null)
                {
                    s_events.Remove(eventType);
                }
                else
                {
                    s_events[eventType] = newDelegate;
                }
            }
        }

        /// <summary>
        /// Phát đi một sự kiện đến tất cả các listener đã đăng ký.
        /// </summary>
        /// <typeparam name="T">Loại sự kiện để phát.</typeparam>
        /// <param name="eventToPublish">Đối tượng sự kiện chứa dữ liệu.</param>
        public static void Publish<T>(T eventToPublish)
        {
            if (s_events.TryGetValue(typeof(T), out Delegate d))
            {
                // ?.Invoke() sẽ an toàn gọi delegate, ngay cả khi nó không có listener nào
                (d as Action<T>)?.Invoke(eventToPublish);
            }
        }
    }
}
