using UnityEngine;

namespace Reign
{
	public struct SDLButton
	{
		public readonly string name;
		public bool on, down, up;

		public readonly SDL3.SDL_GamepadButton button;
		public readonly SDL3.SDL_GamepadAxis axis;
		public readonly bool useAxis, negAxis;

		public SDLButton(SDL3.SDL_GamepadButton button, string name)
		: this(button, name, false, false, SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_INVALID)
		{}

		public SDLButton(SDL3.SDL_GamepadButton button, string name, bool useAxis, bool negAxis, SDL3.SDL_GamepadAxis axis)
		{
			this.button = button;
			this.name = name;
			this.useAxis = useAxis;
			this.negAxis = negAxis;
			this.axis = axis;
			on = false;
			down = false;
			up = false;
		}

		internal void Update(bool buttonOn, float axisValue)
		{
			if (button == SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_INVALID) return;
			if (useAxis)
			{
				if (negAxis) UpdateInternal(axisValue < -.5f);
				else UpdateInternal(axisValue > .5f);
			}
			else
			{
				UpdateInternal(buttonOn);
			}
		}

		private void UpdateInternal(bool pressed)
		{
			down = false;
			up = false;
			if (on != pressed)
			{
				on = pressed;
				if (pressed) down = true;
				else up = true;
			}
		}
	}
}
