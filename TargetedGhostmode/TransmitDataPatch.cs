using Harmony;

namespace TargetedGhostmode
{
	[HarmonyPatch(typeof(PlayerPositionManager), "TransmitData")]
	class TransmitDataPatch
	{
		private static bool Prefix(PlayerPositionManager __instance)
		{
			Ghostmode.HandleHook(__instance);
			return false;
		}
	}
}
