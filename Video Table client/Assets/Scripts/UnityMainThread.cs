using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class UnityMainThread : MonoBehaviour
    {
        internal static UnityMainThread wkr;
        Queue<Action> jobs = new Queue<Action>();

        void Awake()
        {
            wkr = this;
        }

        void Update()
        {
            while (jobs.Count > 0)
                jobs.Dequeue().Invoke();
        }

        internal void AddJob(Action newJob)
        {
            jobs.Enqueue(newJob);
        }
    }
}
