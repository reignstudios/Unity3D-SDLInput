using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

using SDL_InitFlags = System.UInt32;
using BOOL = System.Byte;
using Uint8 = System.Byte;
using Uint16 = System.UInt16;
using Uint32 = System.UInt32;
using Uint64 = System.UInt64;
using Sint16 = System.Int16;
using Sint32 = System.Int32;
using Sint64 = System.Int64;
using SDL_DisplayID = System.UInt32;
using SDL_WindowID = System.UInt32;
using SDL_KeyboardID = System.UInt32;
using SDL_MouseID = System.UInt32;
using SDL_Keycode = System.UInt32;
using SDL_Keymod = System.UInt16;
using SDL_MouseButtonFlags = System.UInt32;
using SDL_JoystickID = System.UInt32;
using SDL_AudioDeviceID = System.UInt32;
using SDL_CameraID = System.UInt32;
using SDL_SensorID = System.UInt32;
using SDL_TouchID = System.UInt64;
using SDL_FingerID = System.UInt64;
using SDL_PenID = System.UInt32;
using SDL_PenInputFlags = System.UInt32;
using SDL_Gamepad = System.IntPtr;
using System.Runtime.InteropServices;

namespace Reign
{
	public unsafe class SDLGamepad
	{
		public readonly string name;
		
		public SDLButton primary1, primary2, primary3, primary4;
		public SDLButton dpadUp, dpadDown, dpadLeft, dpadRight;
		public SDLButton menu, back;

		public SDLButton bumperLeft, bumperRight;
		public SDLTrigger triggerLeft, triggerRight;

		public SDLButton joystickButtonLeft, joystickButtonRight;
		public SDLJoystick joystickLeft, joystickRight;

		public readonly SDL_Gamepad device;

		public SDLGamepad(SDL_Gamepad device)
		{
			this.device = device;

			byte* namePtr = SDL3.SDL_GetGamepadName(device);
			name = Marshal.PtrToStringAnsi((IntPtr)namePtr);
			Debug.Log($"SDLInput: Gamepad connected: '{name}'");

			// buttons
			primary1 = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH, "Primary Down");
			primary2 = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST, "Primary Left");
			primary3 = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH, "Primary Up");
			primary4 = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST, "Primary Right");

			dpadDown = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN, "Dpad Down");
			dpadLeft = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT, "Dpad Left");
			dpadUp = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP, "Dpad Up");
			dpadRight = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT, "Dpad Right");

			menu = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START, "Menu");
			back = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK, "Back");

			bumperLeft = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER, "Bumper Left");
			bumperRight = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER, "Bumper Right");

			joystickButtonLeft = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK, "Joystick Button Left");
			joystickButtonRight = new SDLButton(SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK, "Joystick Button Right");

			// axis
			triggerLeft = new SDLTrigger(SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER, "Trigger Left");
			triggerRight = new SDLTrigger(SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER, "Trigger Right");
					
			joystickLeft = new SDLJoystick(SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX, SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY, "Joystick Left");
			joystickRight = new SDLJoystick(SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX, SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY, "Joystick Right");
		}

		internal void UpdateButton(SDL3.SDL_GamepadButton button, BOOL down)
		{
			switch (button)
			{
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH: primary1.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST: primary2.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH: primary3.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST: primary4.Update(down != 0, 0); break;

				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN: dpadDown.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT: dpadLeft.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP: dpadUp.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT: dpadRight.Update(down != 0, 0); break;

				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START: menu.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK: back.Update(down != 0, 0); break;

				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER: bumperLeft.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER: bumperRight.Update(down != 0, 0); break;

				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK: joystickButtonLeft.Update(down != 0, 0); break;
				case SDL3.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK: joystickButtonRight.Update(down != 0, 0); break;
			}
		}

		internal void UpdateAxis(SDL3.SDL_GamepadAxis axis, float value)
		{
			switch (axis)
			{
				case SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER: triggerLeft.Update(value); break;
				case SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER: triggerRight.Update(value); break;

				case SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX: joystickLeft.UpdateX(value); break;
				case SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY: joystickLeft.UpdateY(-value); break;

				case SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX: joystickRight.UpdateX(value); break;
				case SDL3.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY: joystickRight.UpdateY(-value); break;
			}
		}

		internal void UpdateFinish()
		{
			joystickLeft.UpdateFinish();
			joystickRight.UpdateFinish();
		}

		public void SetRumble(float lowFrequencyLeft, float highFrequencyRight)
		{
			SDL3.SDL_RumbleGamepad(device, (Uint16)(lowFrequencyLeft * Uint16.MaxValue), (Uint16)(highFrequencyRight * Uint16.MaxValue), 100);
		}
	}
}