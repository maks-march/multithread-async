using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThreadPool;

internal class MyThreadPool : IThreadPool
{
	object locker = new object();
	Queue<Action> tasks = new Queue<Action>();
	EventWaitHandle wh = new AutoResetEvent(false);
	Thread[] ths;
	
	public MyThreadPool(int concurrency)
	{
		ths = new Thread[concurrency];
		for (int i = 0; i < concurrency; i++)
		{
			ths[i] = new Thread(Work);
			ths[i].Start();
		}
	}
	public void Dispose()
	{
		foreach (var thread in ths)
		{
			EnqueueAction(null);
		}
		foreach (var thread in ths)
		{
			thread.Join();
		}
		wh.Close();
	}
	public void EnqueueAction(Action action)
	{
		lock (locker)
			tasks.Enqueue(action);
		wh.Set();
	}
	
	void Work() 
	{
		while (true) 
		{
			Action task = null;

			lock (locker)
			{
				if (tasks.Count > 0) 
				{
					task = tasks.Dequeue();
					if (task == null)
						return;
				}
			}

			if (task != null) 
			{
				try
				{
					task();
				}
				catch
				{
					//...logging
				}
			}
			else
				wh.WaitOne();
		}
	}
}