using System;
using GrainInterface;
using MessageContract;
using Orleans;
using System.Threading.Tasks;

namespace GrainActor
{
	public class MyActor : Grain, IMyInterface
	{
		public Task<int> ThisMethodBreaksTheClient(SimpleMessage message)
		{
			return Task.FromResult(2);
		}

		public Task<int> ThisMethodDoesNotBreakTheClient(string message)
		{
			return Task.FromResult(2);
		}
	}
}
