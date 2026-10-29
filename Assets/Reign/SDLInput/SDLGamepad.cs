//#undef ENABLE_INPUT_SYSTEM

#if UINPUT_DISABLE_NEW
#undef ENABLE_INPUT_SYSTEM
#endif

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Haptics;
#endif

namespace Reign
{
	#if !ENABLE_INPUT_SYSTEM
	public enum SDLGamepadType
	{
		Unknown,
		Xbox,
		Playstation,
		SteelSeries
	}
	#endif

	public sealed class SDLGamepad
	{
		public readonly string name;
		
		public SDLButton primary1, primary2, primary3, primary4;
		public SDLButton dpadUp, dpadDown, dpadLeft, dpadRight;
		public SDLButton menu, back;

		public SDLButton bumperLeft, bumperRight;
		public SDLTrigger triggerLeft, triggerRight;

		public SDLButton joystickButtonLeft, joystickButtonRight;
		public SDLJoystick joystickLeft, joystickRight;

		#if ENABLE_INPUT_SYSTEM
		public readonly InputDevice device;
		private IDualMotorRumble dualMotorRumble;
		public readonly List<InputControl> controls;

		public SDLGamepad(InputDevice device)
		{
			this.device = device;
			name = device.name;
			dualMotorRumble = device as IDualMotorRumble;

			// grab only the controls we need
			var inputControls = device.allControls;
			controls = new List<InputControl>();
			foreach (var control in inputControls)
			{
				string path = control.path;
				if (control is ButtonControl button)
				{
					// ignore joysticks acting like dpad
					if (path.Contains("Stick/") || path.Contains("Trigger/")) continue;

					// map specific buttons to known primitives
					switch (control.name)
					{
						// actual buttons
						case "buttonSouth": primary1 = new UButton(button, "Primary Down"); break;
						case "buttonWest": primary2 = new UButton(button, "Primary Left"); break;
						case "buttonNorth": primary3 = new UButton(button, "Primary Up"); break;
						case "buttonEast": primary4 = new UButton(button, "Primary Right"); break;

						case "down": dpadDown = new UButton(button, "Dpad Down"); break;
						case "left": dpadLeft = new UButton(button, "Dpad Left"); break;
						case "up": dpadUp = new UButton(button, "Dpad Up"); break;
						case "right": dpadRight = new UButton(button, "Dpad Right"); break;

						case "start": menu = new UButton(button, "Menu"); break;
						case "select": back = new UButton(button, "Back"); break;

						case "leftShoulder": bumperLeft = new UButton(button, "Bumper Left"); break;
						case "rightShoulder": bumperRight = new UButton(button, "Bumper Right"); break;

						case "leftStickPress": joystickButtonLeft = new UButton(button, "Joystick Button Left"); break;
						case "rightStickPress": joystickButtonRight = new UButton(button, "Joystick Button Right"); break;

						// axis using button type
						case "leftTrigger": triggerLeft = new UTrigger(button, "Trigger Left"); break;
						case "rightTrigger": triggerRight = new UTrigger(button, "Trigger Right"); break;
					}
				}
				else if (control is StickControl stick)
				{
					switch (control.name)
					{
						case "leftStick": joystickLeft = new UJoystick(stick, "Joystick Left"); break;
						case "rightStick": joystickRight = new UJoystick(stick, "Joystick Right"); break;
					}
				}
			}
		}
		#else
		public readonly int deviceIndex;
		public readonly SDLGamepadType type;

		public SDLGamepad(int deviceIndex, string name)
		{
			this.deviceIndex = deviceIndex;
			string i = deviceIndex.ToString();

			#if UNITY_STANDALONE_WIN
			if (name.Contains("Xbox")) type = SDLGamepadType.Xbox;

			// =====================
			// Xbox
			// =====================
			if (type == SDLGamepadType.Xbox || type == SDLGamepadType.Unknown)
			{
				// buttons
				primary1 = new SDLButton($"button_{deviceIndex}_0", "Primary Down");
				primary2 = new SDLButton($"button_{deviceIndex}_2", "Primary Left");
				primary3 = new SDLButton($"button_{deviceIndex}_3", "Primary Up");
				primary4 = new SDLButton($"button_{deviceIndex}_1", "Primary Right");

				dpadDown = new SDLButton($"axis_{deviceIndex}_6", "Dpad Down", true, true);
				dpadLeft = new SDLButton($"axis_{deviceIndex}_5", "Dpad Left", true, true);
				dpadUp = new SDLButton($"axis_{deviceIndex}_6", "Dpad Up", true, false);
				dpadRight = new SDLButton($"axis_{deviceIndex}_5", "Dpad Right", true, false);

				menu = new SDLButton($"button_{deviceIndex}_7", "Menu");
				back = new SDLButton($"button_{deviceIndex}_6", "Back");

				bumperLeft = new SDLButton($"button_{deviceIndex}_4", "Bumper Left");
				bumperRight = new SDLButton($"button_{deviceIndex}_5", "Bumper Right");

				joystickButtonLeft = new SDLButton($"button_{deviceIndex}_8", "Joystick Button Left");
				joystickButtonRight = new SDLButton($"button_{deviceIndex}_9", "Joystick Button Right");

				// triggers
				triggerLeft = new SDLTrigger($"axis_{deviceIndex}_8", "Trigger Left");
				triggerRight = new SDLTrigger($"axis_{deviceIndex}_9", "Trigger Right");

				// joysticks
				joystickLeft = new SDLJoystick($"axis_{deviceIndex}_0", $"axis_{deviceIndex}_1", "Joystick Left");
				joystickRight = new SDLJoystick($"axis_{deviceIndex}_3", $"axis_{deviceIndex}_4", "Joystick Right");
			}
			#elif UNITY_STANDALONE_OSX
			if (name.Contains("Sony")) type = SDLGamepadType.Playstation;
			else if (name.Contains("SteelSeries") || name.Contains("Nimbus")) type = SDLGamepadType.SteelSeries;

			// =====================
			// Playstation
			// =====================
			if (type == SDLGamepadType.Playstation || type == SDLGamepadType.Unknown)
			{
				// buttons
				primary1 = new UButton($"button_{deviceIndex}_1", "Primary Down");
				primary2 = new UButton($"button_{deviceIndex}_0", "Primary Left");
				primary3 = new UButton($"button_{deviceIndex}_3", "Primary Up");
				primary4 = new UButton($"button_{deviceIndex}_2", "Primary Right");

				dpadDown = new UButton($"axis_{deviceIndex}_7", "Dpad Down", true, false);
				dpadLeft = new UButton($"axis_{deviceIndex}_6", "Dpad Left", true, true);
				dpadUp = new UButton($"axis_{deviceIndex}_7", "Dpad Up", true, true);
				dpadRight = new UButton($"axis_{deviceIndex}_6", "Dpad Right", true, false);

				menu = new UButton($"button_{deviceIndex}_9", "Menu");
				back = new UButton($"button_{deviceIndex}_8", "Back");

				bumperLeft = new UButton($"button_{deviceIndex}_4", "Bumper Left");
				bumperRight = new UButton($"button_{deviceIndex}_5", "Bumper Right");

				joystickButtonLeft = new UButton($"button_{deviceIndex}_10", "Joystick Button Left");
				joystickButtonRight = new UButton($"button_{deviceIndex}_11", "Joystick Button Right");

				// triggers
				triggerLeft = new UTrigger($"axis_{deviceIndex}_4", "Trigger Left", 1, 2, 0);
				triggerRight = new UTrigger($"axis_{deviceIndex}_5", "Trigger Right", 1, 2, 0);

				// joysticks
				joystickLeft = new UJoystick($"axis_{deviceIndex}_0", $"axis_{deviceIndex}_1", "Joystick Left");
				joystickRight = new UJoystick($"axis_{deviceIndex}_2", $"axis_{deviceIndex}_3", "Joystick Right");
			}
			else if (type == SDLGamepadType.SteelSeries)// NOTE: only joysticks and menu button seem to work
			{
				menu = new UButton($"button_{deviceIndex}_0", "Menu");

				// joysticks
				joystickLeft = new UJoystick($"axis_{deviceIndex}_0", $"axis_{deviceIndex}_1", "Joystick Left", false, false);
				joystickRight = new UJoystick($"axis_{deviceIndex}_2", $"axis_{deviceIndex}_3", "Joystick Right", false, false);
			}
			#endif

			Debug.Log("Gamepad Type: " + type.ToString());
		}
		#endif

		internal void Update()
		{
			//TestMappings();

			primary1.Update();
			primary2.Update();
			primary3.Update();
			primary4.Update();

			dpadDown.Update();
			dpadLeft.Update();
			dpadUp.Update();
			dpadRight.Update();

			menu.Update();
			back.Update();

			bumperLeft.Update();
			bumperRight.Update();
			triggerLeft.Update();
			triggerRight.Update();

			joystickButtonLeft.Update();
			joystickButtonRight.Update();
			joystickLeft.Update();
			joystickRight.Update();
		}

		public void SetRumble(float lowFrequencyLeft, float highFrequencyRight)
		{
			#if ENABLE_INPUT_SYSTEM
			if (dualMotorRumble != null)
			{
				dualMotorRumble.SetMotorSpeeds(lowFrequencyLeft, highFrequencyRight);
			}
			#endif
		}

		private void TestMappings()
		{
			#if ENABLE_INPUT_SYSTEM
			// NOTE: test mapping code
			var inputControls = device.allControls;
			foreach (var control in inputControls)
			{
				string path = control.path;
				//Debug.Log($"Control: '{control.name}' Path:'{path}' Type:'{control.GetType()}'");

				if (control is ButtonControl button)
				{
					if (button.wasPressedThisFrame) Debug.Log($"Button: '{control.name}' Path:'{path}'");
					if (button.value != 0) Debug.Log($"Axis: '{control.name}' Path:'{path}' Value:'{button.value}'");// IDK why this is an button but ok Unity
				}
				else if (control is StickControl stick)
				{
					var value = stick.value;
					if (value.magnitude != 0) Debug.Log($"Stick: '{control.name}' Path:'{path}'");
				}
			}
			#else
			// buttons
			for (int i = 0; i != 28; ++i)
			{
				string b = i.ToString();
				if (Input.GetButtonDown($"button_{deviceIndex}_{b}"))
				{
					Debug.Log($"Gamepad_{deviceIndex} Button_{b}");
				}
			}

			// axes
			for (int i = 0; i != 28; ++i)
			{
				string a = i.ToString();
				float value = Input.GetAxis($"axis_{deviceIndex}_{a}");
				if (Mathf.Abs(value) >= .1f)
				{
					Debug.Log($"Gamepad_{deviceIndex} Axis_{a} Value = {value}");
				}
			}
			#endif
		}
	}
}