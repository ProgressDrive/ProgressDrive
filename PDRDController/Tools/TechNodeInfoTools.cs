//
// ProgressDrive
// A KSP Mod by toadicus
//
// TechNodeInfoTools.cs
//
// node is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute node software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of node software dedicate any and all copyright interest in the
// software to the public domain. We make node dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend node dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to node
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

using ProgressDrive.TechTree;
using System;
using System.Collections.Generic;
using System.Text;
using ToadicusTools;

namespace ProgressDrive
{
	public static class TechNodeInfoTools
	{
		private static Dictionary<int, Func<TechNodeInfo, string>> bakedSPrints =
			new Dictionary<int, Func<TechNodeInfo, string>>();

		public static StringBuilder SPrint(this TechNodeInfo node, StringBuilder sb, int indent, uint depth)
		{
			if (indent < 0)
			{
				indent = 0;
			}

			int subdent = indent + 1;

			sb.AddIntendedLine("TechNodeInfo {", indent);

			sb.AddIntendedLine(string.Format("nodeName={0}", node.nodeName), subdent);
			sb.AddIntendedLine(string.Format("AnyParentToUnlock={0}", node.AnyParentToUnlock), subdent);
			sb.AddIntendedLine(string.Format("iconRef={0}", node.iconRef), subdent);
			sb.AddIntendedLine(string.Format("scale={0}", node.scale), subdent);

			sb.AddIntendedLine(string.Format("techID={0}", node.techID), subdent);
			sb.AddIntendedLine(string.Format("title={0}", node.title), subdent);
			sb.AddIntendedLine(string.Format("description={0}", node.description), subdent);
			sb.AddIntendedLine(string.Format("scienceCost={0}", node.scienceCost), subdent);
			sb.AddIntendedLine(string.Format("hideIfNoPParts={0}", node.hideIfNoPParts), subdent);

			sb.AddIntendedLine(string.Format("parents=[{0}]", node.Parents.SPrint()), subdent);

			if (depth > 0 && node.Children.Count > 0)
			{
				depth--;
				Func<TechNodeInfo, string> childFunc;

				if (!bakedSPrints.TryGetValue(subdent, out childFunc))
				{
					childFunc = delegate(TechNodeInfo arg)
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
					bakedSPrints[subdent] = childFunc;
				}

				sb.AddIntendedLine(
					string.Format("children=[\n{0}", node.Children.SPrint(childFunc)),
					subdent
				);

				sb.AddIntendedLine("]", subdent);
			}
			else
			{
				sb.AddIntendedLine(string.Format("children=[{0}]", node.Children.SPrint()), subdent);
			}

			sb.AddIntendedLine("}", indent);

			return sb;
		}

		public static string SPrint(this TechNodeInfo node, int indent, uint depth)
		{
			StringBuilder sb = Tools.GetStringBuilder();
			string msg;

			msg = node.SPrint(sb, indent, depth).ToString();

			Tools.PutStringBuilder(sb);

			return msg;
		}

		public static string SPrint(this TechNodeInfo node, int indent)
		{
			return node.SPrint(indent, uint.MaxValue);
		}
	}
}

