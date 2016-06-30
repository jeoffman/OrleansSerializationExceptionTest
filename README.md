# OrleansSerializationExceptionTest
test program to show a SerializationException problem using Orleans Streams with/without grains


1) Run the Silo
2) Run the client - after connected to the silo, press Enter a few times to send a message

Look in the MyActor grain and the IMyInterface interface for the method "ThisMethodBreaksTheClient". When this is commented out the client works fine.
