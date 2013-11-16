using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BulletXNA.LinearMath
{

    public class PooledType<T> where T : new()
    {

        public T Get()
        {
            if (m_poolLocking)
                lock (m_pool)
                {
                    T obj2 =_get();
                    return obj2;
                }
            else
            {
                T obj2 = _get();
                return obj2;
            }
            T obj = _get();
            return obj;
        }

        private T _get()
        {
            if (!m_poolEnabled)
                return new T();
            if (m_pool.Count == 0)
            {
                m_pool.Push(new T());
            }
            return m_pool.Pop();
        }

        public void Free(T obj)
        {
            if (m_poolEnabled)
            {
                if (m_poolLocking)
                {
                    lock (m_pool)
                    {
                        _free(obj);
                    }
                }
                else
                {
                    _free(obj);
                }

            }
        }
        private void _free(T obj)
        {
            Debug.Assert(!m_pool.Contains(obj));
            m_pool.Push(obj);
        }

        public void SetPoolingEnabled(bool tf)
        {
            m_poolEnabled = tf;
        }

        public void SetLockingEnabled(bool tf)
        {
            m_poolLocking = tf;
        }

        private Stack<T> m_pool = new Stack<T>();
        private bool m_poolEnabled = true;
        private bool m_poolLocking = false;
    }
}
