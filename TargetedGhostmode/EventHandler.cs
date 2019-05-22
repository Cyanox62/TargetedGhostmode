using Smod2.EventHandlers;
using Smod2.Events;

namespace TargetedGhostmode
{
	internal class EventHandler : IEventHandlerWaitingForPlayers
	{
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev) => Ghostmode.pHideDict.Clear();
	}
}
