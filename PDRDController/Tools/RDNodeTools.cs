//
// ProgressDrive
// A KSP Mod by toadicus
//
// RDNodeTools.cs
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

namespace ProgressDrive
{
	public static class RDNodeTools
	{
		private static Dictionary<int, Func<RDNode, string>> bakedRDNodeSPrints =
			new Dictionary<int, Func<RDNode, string>>();

		public static StringBuilder SPrint(this RDNode node, StringBuilder sb, int indent, uint depth)
		{
			if (indent < 0)
			{
				indent = 0;
			}

			int subdent = indent + 1;

			sb.AddIntendedLine("RDNode {", indent);

			sb.AddIntendedLine(string.Format("name={0}", node.name), subdent);
			sb.AddIntendedLine(string.Format("description={0}", node.description), subdent);
			sb.AddIntendedLine(string.Format("treeNode={0}", node.treeNode), subdent);
			sb.AddIntendedLine(string.Format("AnyParentToUnlock={0}", node.AnyParentToUnlock), subdent);
			sb.AddIntendedLine(string.Format("selected={0}", node.selected), subdent);
			sb.AddIntendedLine(string.Format("scale={0}", node.scale), subdent);
			sb.AddIntendedLine(string.Format("state={0}", Enum.GetName(typeof(RDNode.State), node.state)), subdent);

			if (node.icon != null)
			{
				sb.AddIntendedLine(string.Format("icon={{name={0}, texture={1}}}", node.icon.name, node.icon.texture), subdent);
			}
			else
			{
				sb.AddIntendedLine("icon=null", subdent);
			}

			sb.AddIntendedLine("tech=(\n", subdent);
			node.tech.SPrint(sb, subdent + 1);
			sb.AddIntendedLine(")", subdent);

			sb.AddIntendedLine(string.Format("parents=[{0}]", node.parents.SPrint(SPrint)), subdent);

			if (depth > 0 && node.children.Count > 0)
			{
				depth--;
				Func<RDNode, string> childFunc;

				if (!bakedRDNodeSPrints.TryGetValue(subdent, out childFunc))
				{
					childFunc = delegate(RDNode arg)
					{
						if (object.ReferenceEquals(arg, node))
						{
							return '\t' * subdent + "self reference";
						}
						else if (arg == null)
						{
							return '\t' * subdent + "null";
						}
						else
						{
							return arg.SPrint(subdent + 1, depth);
						}
					};
					bakedRDNodeSPrints[subdent] = childFunc;
				}

				sb.AddIntendedLine(
					string.Format("children=[\n{0}", node.children.SPrint(childFunc)),
					subdent
				);

				sb.AddIntendedLine("]", subdent);
			}
			else
			{
				sb.AddIntendedLine(string.Format("children=[{0}]", node.children.SPrint(SPrintSimple)), subdent);
			}

			sb.AddIntendedLine("}", indent);

			return sb;
		}

		public static string SPrint(this RDNode node, int indent, uint depth)
		{
			StringBuilder sb = Tools.GetStringBuilder();
			string msg;

			node.SPrint(sb, indent, depth);

			msg = sb.ToString();

			Tools.PutStringBuilder(sb);

			return msg;
		}

		public static string SPrint(this RDNode node, int indent)
		{
			return node.SPrint(indent, uint.MaxValue);
		}

		public static string SPrintSimple(this RDNode node)
		{
			return string.Format("{0} (RDNode)", node.name);
		}

		public static string SPrint(this RDNode.Parent parent)
		{
			if (parent.parent != null)
			{
				if (parent.parent.node != null)
				{
					if (parent.parent.node.name != null)
					{
						return parent.parent.node.name;
					}
				}
			}

			return "null";
		}
	}
}

