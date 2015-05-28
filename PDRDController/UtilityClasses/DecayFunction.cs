//
// ProgressDrive
// A KSP Mod by toadicus
//
// DecayFunction.cs
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

namespace ProgressDrive
{
	public class DecayFunction
	{
		private const double EULER = 2.7182818284590452d;
		private const double EULERSQR = 7.3890560989306502d;

		public double HalfLife;

		public double InitialUT;

		public DecayFunction(double halflife, double initialut)
		{
			this.HalfLife = halflife;
			this.InitialUT = initialut;
		}

		public double DecayFactorAtUT(double ut)
		{
			if (ut <= this.InitialUT)
			{
				return 1d;
			}
			else
			{
				return 1d / Math.Log((ut - InitialUT) * (EULERSQR - EULER) / HalfLife + EULER);
			}
		}
	}
}

