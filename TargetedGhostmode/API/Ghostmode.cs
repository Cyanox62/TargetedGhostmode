using RemoteAdmin;
using Smod2;
using System;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace TargetedGhostmode
{
	public static class Ghostmode
	{
		internal static Dictionary<int, List<int>> pHideDict = new Dictionary<int, List<int>>();

		/// <summary>
		/// Determines if a player can be seen by another player.
		/// </summary>
		/// <param name="source">The player to check visibility on.</param>
		/// <param name="target">The player being checked for visibility.</param>
		public static bool IsHiddenFrom(this Player source, Player target)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (pHideDict.ContainsKey(source.PlayerId))
			{
				return pHideDict[source.PlayerId].Contains(target.PlayerId);
			}
			return false;
		}

		/// <summary>
		/// Gets a list of player IDs that are invisible to a player.
		/// </summary>
		/// <param name="source">The player to get the hidden players from.</param>
		public static IEnumerable<int> HiddenFrom(this Player source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			if (pHideDict.ContainsKey(source.PlayerId))
			{
				foreach (int pId in pHideDict[source.PlayerId])
				{
					yield return pId;
				}
			}
		}

		/// <summary>
		/// Hides player target from player source.
		/// </summary>
		/// <param name="source">Player to be hidden from.</param>
		/// <param name="target">Player to hide.</param>
		public static void HidePlayer(this Player source, Player target)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (target == null) throw new ArgumentNullException(nameof(target));

			VerifyPlayerInDict(source);
			if (pHideDict[source.PlayerId].Contains(target.PlayerId)) throw new InvalidOperationException("Player is already hidden.");
			pHideDict[source.PlayerId].Add(target.PlayerId);
		}

		/// <summary>
		/// Shows player target to player source.
		/// </summary>
		/// <param name="source">Player to be shown to.</param>
		/// <param name="target">Player to show.</param>
		public static void ShowPlayer(this Player source, Player target)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (target == null) throw new ArgumentNullException(nameof(target));

			if (pHideDict.ContainsKey(source.PlayerId))
			{
				if (!pHideDict[source.PlayerId].Contains(target.PlayerId)) throw new InvalidOperationException("Player is not hidden.");
				pHideDict[source.PlayerId].Remove(target.PlayerId);
			}
		}

		/// <summary>
		/// Hides a player from everyone.
		/// </summary>
		/// <param name="target">Player to hide globally.</param>
		public static void HideGlobal(this Player target)
		{
			if (target == null) throw new ArgumentNullException(nameof(target));

			foreach (Player player in PluginManager.Manager.Server.GetPlayers())
			{
				VerifyPlayerInDict(player);
				if (!pHideDict[player.PlayerId].Contains(target.PlayerId))
				{
					pHideDict[player.PlayerId].Add(target.PlayerId);
				}
			}
		}

		/// <summary>
		/// Shows a player to everyone.
		/// </summary>
		/// <param name="target">Player to show globally.</param>
		public static void ShowGlobal(this Player target)
		{
			if (target == null) throw new ArgumentNullException(nameof(target));

			foreach (KeyValuePair<int, List<int>> entry in pHideDict)
			{
				if (entry.Value.Contains(target.PlayerId))
				{
					entry.Value.Remove(target.PlayerId);
				}
			}
		}

		private static void VerifyPlayerInDict(Player player)
		{
			if (!pHideDict.ContainsKey(player.PlayerId))
			{
				pHideDict.Add(player.PlayerId, new List<int>());
			}
		}

		internal static void HandleHook(PlayerPositionManager __instance)
		{
			List<PlayerPositionData> normalData = new List<PlayerPositionData>();
			List<GameObject> players = ((IEnumerable<GameObject>)PlayerManager.singleton.players).ToList<GameObject>();
			foreach (GameObject _player in players) normalData.Add(new PlayerPositionData(_player));
			__instance.ReceiveData(normalData.ToArray());
			foreach (GameObject gameObject in players)
			{
				CharacterClassManager component1 = gameObject.GetComponent<CharacterClassManager>();
				if (component1.curClass >= 0 && (component1.curClass == (int)Role.SCP_939_53 || component1.curClass == (int)Role.SCP_939_89))
				{
					List<PlayerPositionData> modifiedData2 = new List<PlayerPositionData>((IEnumerable<PlayerPositionData>)normalData);
					for (int index = 0; index < modifiedData2.Count; ++index)
					{
						CharacterClassManager component2 = players[index].GetComponent<CharacterClassManager>();
						if ((double)modifiedData2[index].position.y < 800.0 && component2.curClass >= 0 && component2.klasy[component2.curClass].team != Team.SCP && (component2.klasy[component2.curClass].team != Team.RIP && !players[index].GetComponent<Scp939_VisionController>().CanSee(component1.GetComponent<Scp939PlayerScript>())))
						{
							modifiedData2[index] = new PlayerPositionData()
							{
								position = Vector3.up * 6000f,
								rotation = 0.0f,
								playerID = modifiedData2[index].playerID
							};
						}
					}
					__instance.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, modifiedData2.ToArray());
				}
				else
					__instance.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, normalData.ToArray());

				KeyValuePair<int, List<int>> entry = pHideDict.FirstOrDefault(x => x.Key == gameObject.GetComponent<QueryProcessor>().PlayerId);
				if (PlayerManager.singleton.players.FirstOrDefault(x => x.GetComponent<QueryProcessor>().PlayerId == entry.Key && entry.Key != 0) != null)
				{
					List<PlayerPositionData> modifiedData = new List<PlayerPositionData>((IEnumerable<PlayerPositionData>)normalData);

					for (int index = 0; index < modifiedData.Count; ++index)
					{
						if (component1.curClass >= 0 && players[index] != gameObject && entry.Value.Contains(players[index].GetComponent<QueryProcessor>().PlayerId))
						{
							modifiedData[index] = new PlayerPositionData()
							{
								position = Vector3.up * 6000f,
								rotation = 0.0f,
								playerID = modifiedData[index].playerID
							};
						}
					}
					__instance.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, modifiedData.ToArray());
				}
			}
		}
	}
}
