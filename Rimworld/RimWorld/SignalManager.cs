using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class SignalManager
	{
		public List<ISignalReceiver> receivers = new List<ISignalReceiver>();

		public void RegisterReceiver(ISignalReceiver receiver)
		{
			if (receiver == null)
			{
				Log.Error("Tried to register a null reciever.");
			}
			else if (receivers.Contains(receiver))
			{
				Log.Error("Tried to register the same receiver twice: " + receiver.ToStringSafe());
			}
			else
			{
				receivers.Add(receiver);
			}
		}

		public void DeregisterReceiver(ISignalReceiver receiver)
		{
			receivers.Remove(receiver);
		}

		public void SendSignal(Signal signal)
		{
			if (DebugViewSettings.logSignals)
			{
				Log.Message("Signal: tag=" + signal.tag.ToStringSafe() + " args=" + signal.args.ToStringSafeEnumerable());
			}
			for (int i = 0; i < receivers.Count; i++)
			{
				try
				{
					receivers[i].Notify_SignalReceived(signal);
				}
				catch (Exception ex)
				{
					Log.Error("Error while sending signal to " + receivers[i].ToStringSafe() + ": " + ex);
				}
			}
		}
	}
}
