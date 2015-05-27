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

namespace PDRDController
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class PDRDController : MonoBehaviour
	{
		private RDController rdController;
		private RDTechTree rdTechTree;

		// @DEBUG: Get rid of this eventually
		private ulong updateCount;

		private void Awake()
		{
			this.Log("Awake");

			RDController.OnRDTreeSpawn.Add(this.onRDTreeSpawn);
			RDController.OnRDTreeDespawn.Add(this.onRDTreeDespawn);
			RDTechTree.OnTechTreeSpawn.Add(this.onTechTreeSpawn);
			RDTechTree.OnTechTreeDespawn.Add(this.onTechTreeDespawn);

			this.updateCount = 0uL;
			this.Log("Awoke");
		}

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
			this.Log("Caught onRDTreeSpawn with controller {0}", ctrlr == null ? "null" : ctrlr.ToString());
			this.rdController = ctrlr;

			for (int nIdx = 0; nIdx < this.rdController.nodes.Count; nIdx++)
			{
				RDNode node = this.rdController.nodes[nIdx];

				if (node.name.ToLower() == "start")
				{
					this.Log("Got RDNode 'start' at index {0}:\n{1}", nIdx, node.SPrint(0));
					break;
				}
			}
		}

		private void onRDTreeDespawn(RDController ctrlr)
		{
			this.Log("Caught onRDTreeDespawn with controller {0}", ctrlr == null ? "null" : ctrlr.ToString());
			this.rdController = null;
		}

		private void onTechTreeSpawn(RDTechTree tree)
		{
			this.rdTechTree = tree;
			ProtoRDNode[] nodes = this.rdTechTree.GetTreeNodes();

			for (int idx = 0; idx < nodes.Length; idx++)
			{
				ProtoRDNode node = nodes[idx];

				if (node.tech.techID == "start")
				{
					this.Log("Got ProtoRDNode 'start' at index {0}:\n{1}", idx, node.SPrint(0));
					break;
				}
			}
		}

		private void onTechTreeDespawn(RDTechTree tree)
		{
			this.rdTechTree = null;
		}
	}
}

