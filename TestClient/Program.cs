using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageContract;
using Orleans;
using Orleans.Streams;

namespace TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			bool orleansInitialized = false;
			while(!orleansInitialized)
			{
				try
				{
					Console.WriteLine("...Initializing Orleans Client...");
					var config = Orleans.Runtime.Configuration.ClientConfiguration.StandardLoad();
					Orleans.GrainClient.Initialize(config); //this can take a few seconds or more
					orleansInitialized = true;
					Console.WriteLine("...Orleans connected...");
					break;
				}
				catch(Exception exc)
				{
				
				}
			}

			WireUpSubscriptionAsync().Wait();

			while(true)
			{
				Console.WriteLine("Press enter to send a message, use CTRL-C to exit");
				Console.ReadLine();
				SendMessage();
			}

		}

		public static async Task WireUpSubscriptionAsync()
		{
			Console.WriteLine("...Setting up subscription...");

			IStreamProvider streamProvider = Orleans.GrainClient.GetStreamProvider("StreamProviding");
			IAsyncStream<SimpleMessage> messageStream = streamProvider.GetStream<SimpleMessage>(Guid.Empty, "test");
			if(messageStream == null)
				Console.WriteLine("OrleansBrcMessageSpy.WireUpSubscription got a null messageStream!?!?!?");
			IList<StreamSubscriptionHandle<SimpleMessage>> allMyHandles = await messageStream.GetAllSubscriptionHandles();
			bool atLeastOneStreamResumed = false;
			foreach(StreamSubscriptionHandle<SimpleMessage> subscription in allMyHandles)
			{
				atLeastOneStreamResumed = true;
				await subscription.ResumeAsync<SimpleMessage>(ReceivePubSubMessageFromTcpServerStream);
				Console.WriteLine("ResumeAsync: WireUpSubscriptionAsync guy = " + subscription.ToString());
			}
			if(!atLeastOneStreamResumed)
			{
				StreamSubscriptionHandle<SimpleMessage> subscriptionHandle = await messageStream.SubscribeAsync<SimpleMessage>
					(
					(m, s) => ReceivePubSubMessageFromTcpServerStream(m, s),
					(e) =>
					{
						Console.WriteLine(e.Message);
						return TaskDone.Done;
					});
				Console.WriteLine("SubscribeAsync: WireUpSubscriptionAsync guy");
			}
		}

		private static Task ReceivePubSubMessageFromTcpServerStream(SimpleMessage message, StreamSequenceToken arg2)
		{
			Console.WriteLine("Got a message");
			return TaskDone.Done;
		}

		private static async void SendMessage()
		{
			await Task.Run(async () =>
			{
				try
				{
					//HEY! this will throw an exception if the Orleans.GrainClient hasn't initialized yet...
					IStreamProvider streamProvider = Orleans.GrainClient.GetStreamProvider("StreamProviding");
					IAsyncStream<SimpleMessage> messageStream = streamProvider.GetStream<SimpleMessage>(Guid.Empty, "test");
					if(messageStream == null)
						Console.WriteLine("MessagesToTcpClientSender.SendMessageFromBrcAsync got a null messageStream trying to send!?!?!?");

					SimpleMessage message = new SimpleMessage() { SimpleText = "Hello World" };

					await messageStream.OnNextAsync(message);
				}
				catch(Exception exc)
				{
					Console.WriteLine("Orleans call MessagesToTcpClientSender.SendMessageAsync has failed, I am going to throw exception: " + exc.Message);
					Console.WriteLine(exc.Message);
					throw;
				}
			});

		}

	}
}
