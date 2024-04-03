using System;
using System.Collections.Generic;
using UnityEngine;

internal class UnityMainThread : MonoBehaviour
{
  internal static UnityMainThread instance;
  Queue<Action> jobs = new Queue<Action>();

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(this);
    }
    DontDestroyOnLoad(gameObject);
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
