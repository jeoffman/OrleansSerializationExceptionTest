using System.Threading.Tasks;
using MessageContract;
using Orleans;

namespace GrainInterface
{
	public interface IMyInterface : IGrainWithGuidKey
	{
		Task<int> ThisMethodBreaksTheClient(SimpleMessage message);
		Task<int> ThisMethodDoesNotBreakTheClient(string message);
	}
}
