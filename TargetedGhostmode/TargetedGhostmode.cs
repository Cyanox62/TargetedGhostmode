using Smod2.Attributes;
using Harmony;
using Smod2;

namespace TargetedGhostmode
{
	[PluginDetails(
	author = "Cyanox",
	name = "TargetedGhostmode",
	description = "Allows for player specific ghostmode.",
	id = "cyan.ghostmode",
	version = "1.0.1",
	SmodMajor = 3,
	SmodMinor = 0,
	SmodRevision = 0
	)]
	public class TargetedGhostmode : Plugin
	{
		public override void OnDisable() { }

		public override void OnEnable() { }

		public override void Register()
		{
			HarmonyInstance.Create(Details.id).PatchAll();
			AddEventHandlers(new EventHandler());
		}
	}
}
