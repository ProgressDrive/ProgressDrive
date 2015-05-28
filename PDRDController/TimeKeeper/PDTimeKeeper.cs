//
// ProgressDrive
// A KSP Mod by toadicus
//
// TimeKeeper.cs
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
	public class PDTimeKeeper : MonoBehaviour
	{
		public static PDTimeKeeper Instance
		{
			get;
			private set;
		}

		private double[] lastTenLaunches;
		private int launchIdx;

		public double LastTurnUT
		{
			get;
			private set;
		}

		public ulong TurnNumber
		{
			get;
			private set;
		}

		public double CurrentTurnDelay
		{
			get;
			private set;
		}

		public delegate void TurnHandler(ulong turnNumber);

		public event TurnHandler OnNextTurn;

		private void Awake()
		{
			Instance = this;

			GameEvents.onLaunch.Add(this.onLaunch);

			this.lastTenLaunches = new double[10];
			this.launchIdx = 0;

			this.LastTurnUT = 0d;
			this.TurnNumber = 0;

			this.CurrentTurnDelay = double.NaN;

			this.Log("Awake");
		}

		private void FixedUpdate()
		{
			if (double.IsNaN(this.CurrentTurnDelay))
			{
				return;
			}

			if (this.OnNextTurn != null && Planetarium.GetUniversalTime() > (this.LastTurnUT + this.CurrentTurnDelay))
			{
				this.OnNextTurn(this.TurnNumber++);
			}
		}

		private void onLaunch(EventReport report)
		{
			double ut = Planetarium.GetUniversalTime();

			this.lastTenLaunches[this.launchIdx % this.lastTenLaunches.Length] = ut;
			this.launchIdx++;

			if (double.IsNaN(this.CurrentTurnDelay))
			{
				this.LastTurnUT = ut;
			}

			double sum = 0d;
			int limit = Math.Min(this.launchIdx, this.lastTenLaunches.Length);

			for (int i = 0; i < limit; i++)
			{
				sum += this.lastTenLaunches[i];
			}

			this.CurrentTurnDelay = sum / (double)limit;

			this.Log("Caught onLaunch: {0}", report.ToString());
		}
	}
}

