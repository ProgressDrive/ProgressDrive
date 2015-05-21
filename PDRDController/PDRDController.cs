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

namespace PDRDController {
	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class PDRDController : MonoBehaviour {
		private bool firstRun;
		private RDController rdController;

		// @DEBUG: Get rid of this eventually
		private ulong updateCount;

		private void Awake() {
			this.Log("Awake");

			RDController.OnRDTreeSpawn.Add(this.onRDTreeSpawn);
			RDController.OnRDTreeDespawn.Add(this.onRDTreeDespawn);

			this.firstRun = true;

			this.updateCount = 0uL;
		}

		private void Update() {
			this.updateCount++;

			if (this.rdController == null) {
				if (RDController.Instance == null) {
					this.Log(
						"RDController is {0}null at update {1}",
						this.updateCount > 1 ? "still " : string.Empty,
						this.updateCount
					);
				}
				else {
					this.Log("Got RDController: {0} on update {1}", RDController.Instance, this.updateCount);
					this.rdController = RDController.Instance;
				}
			}
			else if (this.firstRun && this.rdController.nodes.Count > 0) {
				this.firstRun = false;

				this.Log("RDController node count was non-zero at update {0}", this.updateCount);

				for (int nIdx = 0; nIdx < this.rdController.nodes.Count; nIdx++) {
					RDNode node = this.rdController.nodes[nIdx];

					if (node.name.ToLower() == "start") {
						this.Log("Got node at index {0}:\n{1}", nIdx, node.SPrint(0));
					}
				}
			}
		}

		private void OnDestroy() {
			this.Log("OnDestroy");

			RDController.OnRDTreeSpawn.Remove(this.onRDTreeSpawn);
			RDController.OnRDTreeDespawn.Remove(this.onRDTreeDespawn);
		}

		private void onRDTreeSpawn(RDController ctrlr) {
			this.Log("Caught onRDTreeSpawn with controller {0}", ctrlr == null ? "null" : ctrlr.ToString());
			this.rdController = ctrlr;
		}

		private void onRDTreeDespawn(RDController ctrlr) {
			this.Log("Caught onRDTreeDespawn with controller {0}", ctrlr == null ? "null" : ctrlr.ToString());
			this.rdController = null;
		}
	}
}

