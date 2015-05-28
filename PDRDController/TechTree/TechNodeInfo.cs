//
// ProgressDrive
// A KSP Mod by toadicus
//
// TechNodeInfo.cs
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

using System;
using System.Collections.Generic;
using System.Text;
using ToadicusTools;

namespace ProgressDrive.TechTree
{
	public class TechNodeInfo : IConfigNode
	{
		private List<string> missingParentIDs;
		private bool subscribedForParents;

		private List<TechNodeInfo> children;
		private List<TechNodeInfo> parents;

		public IList<TechNodeInfo> Children
		{
			get;
			private set;
		}

		public IList<TechNodeInfo> Parents
		{
			get;
			private set;
		}

		public string nodeName
		{
			get;
			private set;
		}

		public bool AnyParentToUnlock
		{
			get;
			private set;
		}

		public string iconRef
		{
			get;
			private set;
		}

		public float scale
		{
			get;
			private set;
		}

		public string techID
		{
			get;
			private set;
		}

		public string title
		{
			get;
			private set;
		}

		public string description
		{
			get;
			private set;
		}

		public int scienceCost
		{
			get;
			private set;
		}

		public bool hideIfNoPParts
		{
			get;
			private set;
		}

		public TechNodeInfo()
		{
			this.children = new List<TechNodeInfo>();
			this.Children = this.children.AsReadOnly();

			this.parents = new List<TechNodeInfo>();
			this.Parents = this.parents.AsReadOnly();

			this.missingParentIDs = new List<string>();
			this.subscribedForParents = false;
		}

		public void Load(ConfigNode node)
		{
			this.nodeName = node.GetValue("nodeName", string.Empty);

			this.AnyParentToUnlock = node.GetValue("anyToUnlock", false);

			this.iconRef = node.GetValue("icon", "RDicon_generic");

			this.scale = node.GetValue("scale", 0.6f);

			this.techID = node.GetValue("id", string.Empty);

			this.title = node.GetValue("title", string.Empty);

			this.description = node.GetValue("description", string.Empty);

			int cost;
			if (node.TryGetValue("cost", out cost))
			{
				this.scienceCost = cost;
			}
			else
			{
				this.scienceCost = int.MaxValue;
			}

			this.hideIfNoPParts = node.GetValue("hideEmpty", true);

			ConfigNode[] parentNodes = node.GetNodes("Parent");
			ConfigNode parentNode; 
			string parentTechID;

			for (int idx = 0; idx < parentNodes.Length; idx++)
			{
				parentNode = parentNodes[idx];

				parentTechID = parentNode.GetValue("parentID", string.Empty);

				if (parentTechID != string.Empty)
				{
					TechNodeInfo parent;

					if (TechTreeManager.TryGetTechInfoByTechID(parentTechID, out parent))
					{
						this.RegisterParent(parent);
						parent.RegisterChild(this);
					}
					else if (!this.subscribedForParents)
					{
						TechTreeManager.OnTechNodeLoaded += this.onTechNodeLoadedHandler;
						this.subscribedForParents = true;
					}
				}
				else
				{
					Tools.PostErrorMessage("TechNodeInfo ({0}): Skipping malformed parent node missing parentID value.",
						this.techID);
				}
			}
		}

		public void Save(ConfigNode node)
		{
			throw new NotImplementedException("TechNodeInfo: Save action not currently supported");
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", title, techID);
		}

		private void onTechNodeLoadedHandler(TechNodeInfo info)
		{
			for (int idx = 0; idx < this.missingParentIDs.Count; idx++)
			{
				if (this.missingParentIDs[idx] == info.techID)
				{
					this.RegisterParent(info);
					info.RegisterChild(this);
					this.missingParentIDs.RemoveAt(idx);
					break;
				}
			}

			if (this.missingParentIDs.Count < 1)
			{
				TechTreeManager.OnTechNodeLoaded -= this.onTechNodeLoadedHandler;
				this.subscribedForParents = false;
			}
		}

		private void RegisterParent(TechNodeInfo info)
		{
			for (int idx = 0; idx < this.parents.Count; idx++)
			{
				if (this.parents[idx] == info)
				{
					return;
				}
			}

			this.parents.Add(info);
		}

		private void RegisterChild(TechNodeInfo info)
		{
			for (int idx = 0; idx < this.children.Count; idx++)
			{
				if (this.children[idx] == info)
				{
					return;
				}
			}

			this.children.Add(info);
		}
	}
}

