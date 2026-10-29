using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Reign
{
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

		public readonly InputDevice device;

		public SDLGamepad(InputDevice device)
		{
			this.device = device;
			name = device.name;

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