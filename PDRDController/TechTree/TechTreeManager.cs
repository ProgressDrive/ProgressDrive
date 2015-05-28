//
// ProgressDrive
// A KSP Mod by toadicus
//
// TechNodeManager.cs
//
// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using KSP;
using System;
using System.Collections.Generic;
using ToadicusTools;

namespace ProgressDrive.TechTree
{
	public static class TechTreeManager
	{
		private const string TECH_DECAY_KEY = "TECH_DECAY";

		private static List<TechNodeInfo> treeTechNodes;
		private static Dictionary<string, TechNodeInfo> treeTechNodesByTechID;

		private static List<ProtoTechDecay> treeDecayNodes;
		private static Dictionary<string, ProtoTechDecay> treeDecayNodesByTechID;

		public static IList<TechNodeInfo> TreeTechNodes
		{
			get;
			private set;
		}

		public static IList<ProtoTechDecay> TreeDecayNodes
		{
			get;
			private set;
		}

		public static bool IsLoaded
		{
			get;
			private set;
		}

		static TechTreeManager()
		{
			treeTechNodes = new List<TechNodeInfo>();
			TreeTechNodes = treeTechNodes.AsReadOnly();

			treeTechNodesByTechID = new Dictionary<string, TechNodeInfo>();

			treeDecayNodes = new List<ProtoTechDecay>();
			TreeDecayNodes = treeDecayNodes.AsReadOnly();

			treeDecayNodesByTechID = new Dictionary<string, ProtoTechDecay>();

			IsLoaded = false;
		}

		public static TechNodeInfo GetTechInfoByTechID(string techID)
		{
			TechNodeInfo info;

			if (treeTechNodesByTechID.TryGetValue(techID, out info))
			{
				return info;
			}

			return null;
		}

		public static bool TryGetTechInfoByTechID(string techID, out TechNodeInfo info)
		{
			return treeTechNodesByTechID.TryGetValue(techID, out info);
		}

		public static ProtoTechDecay GetTechDecayByTechID(string techID)
		{
			ProtoTechDecay decay;

			if (treeDecayNodesByTechID.TryGetValue(techID, out decay))
			{
				return decay;
			}

			return null;
		}

		public static bool TryGetTechDecayByTechID(string techID, out ProtoTechDecay decay)
		{
			return treeDecayNodesByTechID.TryGetValue(techID, out decay);
		}

		public static ProtoTechNode GetTechStateByTechID(string techID)
		{
			if (ResearchAndDevelopment.Instance != null)
			{
				return ResearchAndDevelopment.Instance.GetTechState(techID);
			}

			return null;
		}

		public static bool TryGetTechStateByTechID(string techID, out ProtoTechNode techState)
		{
			techState = GetTechStateByTechID(techID);

			return (techState != null);
		}

		public static int GetTechCostByTechIDAtUT(string techID, double ut)
		{
			TechNodeInfo techInfo;
			ProtoTechDecay techDecay;

			if (TryGetTechInfoByTechID(techID, out techInfo))
			{
				if (TryGetTechDecayByTechID(techID, out techDecay))
				{
					return (int)((double)techInfo.scienceCost * techDecay.DecayFactorAtUT(ut));
				}
				else
				{
					return techInfo.scienceCost;
				}
			}
			else
			{
				return int.MaxValue;
			}
		}

		public static ProtoTechDecay OnTechnologyResearched(string techID, double halflife, double initialut)
		{
			ProtoTechDecay techDecay;

			if (treeDecayNodesByTechID.TryGetValue(techID, out techDecay))
			{
				Log("Got OnTechnologyResearched for {0} after it was already researched; ignoring.");
			}
			else
			{
				techDecay = new ProtoTechDecay(techID, halflife, initialut);
				treeDecayNodes.Add(techDecay);
				treeDecayNodesByTechID[techID] = techDecay;
				Log("Got first OnTechnologyResearched for {0}, adding new decay node {1}", techID, techDecay);
			}

			return techDecay;
		}

		public static void Load(ConfigNode node)
		{
			Log("Loading tech tree");
			LoadTechTree();
			Log("Loaded {0} techs", treeTechNodes.Count);

			if (node != null)
			{
				Log("Loading tech decay nodes");
				LoadDecayNodes(node);
				Log("Loaded {0} tech decay nodes", treeDecayNodes.Count);
			}
			else
			{
				Log("TECHTREEMANAGER node is null; not loading decay nodes");
			}

			IsLoaded = true;
		}

		public static void Save(ConfigNode node)
		{
			SaveDecayNodes(node);
		}

		private static void LoadTechTree()
		{
			string techTreeFilePath = KSPUtil.ApplicationRootPath + HighLogic.CurrentGame.Parameters.Career.TechTreeUrl;

			ConfigNode techTreeNode = ConfigNode.Load(techTreeFilePath).GetNode("TechTree");
			ConfigNode[] techTreeNodes = techTreeNode.GetNodes("RDNode");
			TechNodeInfo techNodeInfo;

			for (int nIdx = 0; nIdx < techTreeNodes.Length; nIdx++)
			{
				techNodeInfo = new TechNodeInfo();

				techNodeInfo.Load(techTreeNodes[nIdx]);

				if (techNodeInfo.techID != string.Empty)
				{
					treeTechNodes.Add(techNodeInfo);
					treeTechNodesByTechID[techNodeInfo.techID] = techNodeInfo;

					if (OnTechNodeLoaded != null)
					{
						OnTechNodeLoaded(techNodeInfo);
					}
				}
				else
				{
					Tools.PostErrorMessage(
						"[TechNodeManager]: Discarding malformed RDNode: techID missing.\nConfigNode: {0}",
						techTreeNodes[nIdx].ToString()
					);
				}
			}
		}

		private static void LoadDecayNodes(ConfigNode node)
		{
			ConfigNode[] decayNodes = node.GetNodes(TECH_DECAY_KEY);
			ConfigNode decayNode;
			ProtoTechDecay techDecay;

			for (int nIdx = 0; nIdx < decayNodes.Length; nIdx++)
			{
				decayNode = decayNodes[nIdx];

				techDecay = new ProtoTechDecay();

				techDecay.Load(decayNode);

				if (techDecay.IsLoaded)
				{
					treeDecayNodes.Add(techDecay);
					treeDecayNodesByTechID[techDecay.techID] = techDecay;
				}
				else
				{
					LogError("Discarding malformed {0}\nConfigNode: {1}", TECH_DECAY_KEY, decayNode);
				}
			}
		}

		private static void SaveDecayNodes(ConfigNode node)
		{
			node.RemoveNodes(TECH_DECAY_KEY);

			ProtoTechDecay techDecay;
			ConfigNode decayNode;

			for (int nIdx = 0; nIdx < treeDecayNodes.Count; nIdx++)
			{
				techDecay = treeDecayNodes[nIdx];

				decayNode = node.AddNode(TECH_DECAY_KEY);

				techDecay.Save(decayNode);
			}
		}

		private static void Log(string format, params object[] args)
		{
			format = string.Format("[TechNodeManager] {0}", format);

			Tools.PostLogMessage(format, args);
		}

		private static void LogError(string format, params object[] args)
		{
			format = string.Format("[TechNodeManager] {0}", format);

			Tools.PostErrorMessage(format, args);
		}

		public static event TechNodeLoadedHandler OnTechNodeLoaded;
		public delegate void TechNodeLoadedHandler(TechNodeInfo info);
	}
}

