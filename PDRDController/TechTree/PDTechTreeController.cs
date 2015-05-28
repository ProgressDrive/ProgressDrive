//
// ProgressDrive
// A KSP Mod by toadicus
//
// PDRDController.cs
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
using ToadicusTools;
using UnityEngine;

namespace ProgressDrive.TechTree
{
	[KSPScenario(
		ScenarioCreationOptions.AddToExistingCareerGames | ScenarioCreationOptions.AddToNewCareerGames,
		GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION
	)]
	public class PDTechTreeController : ScenarioModule
	{
		private const string TECHTREEMANAGER_KEY = "TECHTREEMANAGER";

		private RDController rdController;
		private RDTechTree rdTechTree;

		// @DEBUG: Get rid of this eventually
		private ulong updateCount;

		#region ScenarioModule overrides
		public override void OnAwake()
		{
			this.Log("Awake");

			base.OnAwake();

			RDController.OnRDTreeSpawn.Add(this.onRDTreeSpawn);
			RDController.OnRDTreeDespawn.Add(this.onRDTreeDespawn);
			RDTechTree.OnTechTreeSpawn.Add(this.onTechTreeSpawn);
			RDTechTree.OnTechTreeDespawn.Add(this.onTechTreeDespawn);

			this.updateCount = 0uL;
			this.Log("Awoke");
		}

		public override void OnLoad(ConfigNode node)
		{
			this.Log("OnLoad");

			if (!TechTreeManager.IsLoaded)
			{
				ConfigNode ttmNode = node.GetNode(TECHTREEMANAGER_KEY);

				TechTreeManager.Load(ttmNode);

				TechTreeManager.OnTechnologyResearched("basicRocketry", 3600d, 0d);
			}
		}

		public override void OnSave(ConfigNode node)
		{
			if (TechTreeManager.IsLoaded)
			{
				node.RemoveNode(TECHTREEMANAGER_KEY);

				ConfigNode ttmNode = node.AddNode(TECHTREEMANAGER_KEY);

				TechTreeManager.Save(ttmNode);
			}
		}
		#endregion

		#region MonoBehaviour LifeCycle
		private void Update()
		{
			this.updateCount++;
		}

		private void OnDestroy()
		{
			this.Log("OnDestroy");
			RDController.OnRDTreeSpawn.Remove(this.onRDTreeSpawn);
			RDController.OnRDTreeDespawn.Remove(this.onRDTreeDespawn);
			RDTechTree.OnTechTreeSpawn.Remove(this.onTechTreeSpawn);
			RDTechTree.OnTechTreeDespawn.Remove(this.onTechTreeDespawn);
			this.Log("OnDestroyed");
		}

		private void onRDTreeSpawn(RDController ctrlr)
		{
			this.Log("Caught onRDTreeSpawn with RDController {0}", ctrlr == null ? "null" : ctrlr.ToString());
			this.rdController = ctrlr;

			for (int nIdx = 0; nIdx < this.rdController.nodes.Count; nIdx++)
			{
				RDNode node = this.rdController.nodes[nIdx];

				node.tech.scienceCost = TechTreeManager.GetTechCostByTechIDAtUT(
					node.tech.techID, Planetarium.GetUniversalTime()
				);
			}
		}
		#endregion

		private void onRDTreeDespawn(RDController ctrlr)
		{
			this.Log("Caught onRDTreeDespawn, nulling RDController");
			this.rdController = null;
		}

		private void onTechTreeSpawn(RDTechTree tree)
		{
			this.Log("Caught onTechTreeSpawn with RDTechTree {0}", tree == null ? "null" : tree.ToString());
			this.rdTechTree = tree;
		}

		private void onTechTreeDespawn(RDTechTree tree)
		{
			this.Log("Caught onTechTreeDespawn, nulling RDTechTree");
			this.rdTechTree = null;
		}
	}
}

