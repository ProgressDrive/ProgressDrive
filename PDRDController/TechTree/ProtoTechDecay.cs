//
// ProgressDrive
// A KSP Mod by toadicus
//
// ProtoTechDecay.cs
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
using ToadicusTools;

namespace ProgressDrive.TechTree
{
	public class ProtoTechDecay : IConfigNode
	{
		private const string TECHID_KEY = "techID";
		private const string HALFLIFE_KEY = "halflife";
		private const string INITIAL_UT_KEY = "initialUT";

		private DecayFunction decayFunction;

		public string techID
		{
			get;
			private set;
		}

		public double HalfLife
		{
			get
			{
				return this.decayFunction.HalfLife;
			}
		}

		public double InitialUT
		{
			get
			{
				return this.decayFunction.InitialUT;
			}
		}

		public bool IsLoaded
		{
			get
			{
				return (this.decayFunction != null) && (this.techID.Length > 0);
			}
		}

		public ProtoTechDecay() {}

		public ProtoTechDecay(string techID, double halflife, double initialut) : this()
		{
			this.techID = techID;

			this.decayFunction = new DecayFunction(halflife, initialut);
		}

		public void Load(ConfigNode node)
		{
			double halflife;
			double initialUT;

			this.techID = node.GetValue(TECHID_KEY, string.Empty);

			if (
				node.TryGetValue(HALFLIFE_KEY, out halflife) &&
				node.TryGetValue(INITIAL_UT_KEY, out initialUT)
			)
			{
				this.decayFunction = new DecayFunction(halflife, initialUT);
			}
		}

		public void Save(ConfigNode node)
		{
			node.SafeSetValue(TECHID_KEY, this.techID);
			node.SafeSetValue(HALFLIFE_KEY, this.HalfLife.ToString());
			node.SafeSetValue(INITIAL_UT_KEY, this.InitialUT.ToString());
		}

		public double DecayFactorAtUT(double ut)
		{
			return this.decayFunction.DecayFactorAtUT(ut);
		}

		public override string ToString()
		{
			return string.Format("[ProtoTechDecay: techID={0}, HalfLife={1}, InitialUT={2}, IsLoaded={3}]",
				techID, HalfLife, InitialUT, IsLoaded);
		}
	}
}

