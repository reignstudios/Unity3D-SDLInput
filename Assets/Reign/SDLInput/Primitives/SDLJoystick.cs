using UnityEngine;

namespace Reign
{
	public struct SDLJoystick
	{
		public readonly string name;
		public static float tolerance = 0.1f;
		public Vector2 value;
		private float valueX, valueY;

		public readonly SDL3.SDL_GamepadAxis axisX, axisY;
		public readonly bool invertX, invertY;

		public SDLJoystick(SDL3.SDL_GamepadAxis axisX, SDL3.SDL_GamepadAxis axisY, string name)
		: this(axisX, axisY, name, false, true)
		{}

		public SDLJoystick(SDL3.SDL_GamepadAxis axisX, SDL3.SDL_GamepadAxis axisY, string name, bool invertX, bool invertY)
		{
			this.axisX = axisX;
			this.axisY = axisY;
			this.name = name;
			this.invertX = invertX;
			this.invertY = invertY;
			value = Vector2.zero;
			valueX = 0;
			valueY = 0;
		}

		internal void UpdateX(float valueX)
		{
			if (axisX != SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_INVALID)
			{
				if (invertX) valueX = -valueX;
				this.valueX = valueX;
			}
		}

		internal void UpdateY(float valueY)
		{
			if (axisY != SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_INVALID)
			{
				if (invertX) valueY = -valueY;
				this.valueY = valueY;
			}
		}

		internal void UpdateFinish()
		{
			var newValue = new Vector2(valueX, valueY);
			if (newValue.magnitude <= tolerance) newValue = Vector2.zero;
			value = newValue;
		}
	}
}