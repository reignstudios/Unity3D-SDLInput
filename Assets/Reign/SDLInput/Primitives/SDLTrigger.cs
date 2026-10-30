using UnityEngine;

namespace Reign
{
	public struct SDLTrigger
	{
		public readonly string name;
		public static float tolerance = 0.1f;
		public float value;

		public readonly SDL3.SDL_GamepadAxis axis;
		public readonly float axisOffset, axisDiv, delayUntilNotEqual;
		private bool hasStarted;

		public SDLTrigger(SDL3.SDL_GamepadAxis axis, string name)
		: this(axis, name, 0, 1, 0)
		{}

		public SDLTrigger(SDL3.SDL_GamepadAxis axis, string name, float axisOffset, float axisDiv, float delayUntilNotEqual)
		{
			this.axis = axis;
			this.name = name;
			this.axisOffset = axisOffset;
			this.axisDiv = axisDiv;
			this.delayUntilNotEqual = delayUntilNotEqual;
			hasStarted = false;
			value = 0;
		}

		internal void Update(float value)
		{
			if (axis != SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_INVALID)
			{
				if (value != delayUntilNotEqual) hasStarted = true;
				if (hasStarted) UpdateInternal((value + axisOffset) / axisDiv);
			}
		}

		private void UpdateInternal(float newValue)
		{
			if (newValue <= tolerance) newValue = 0;
			value = newValue;
		}
	}
}
