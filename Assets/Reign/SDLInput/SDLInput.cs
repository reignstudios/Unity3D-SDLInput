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

namespace Reign
{
    [DefaultExecutionOrder(-32000)]
    public unsafe class SDLInput : MonoBehaviour
    {
        public static List<SDLGamepad> gamepads { get; private set; }

        public delegate void DevicesChangedCallbackMethod(SDLGamepad gamepad, bool add);
        public static event DevicesChangedCallbackMethod DevicesChangedCallback;
        
        public bool initAllowingBackgroundInput, loadGamepadMappings = true;
        private bool isInit;

		private void Start()
		{
            Debug.Log("SDL3Input: v1.0.1");
            gamepads = new List<SDLGamepad>();

            // get version
            Debug.Log("SDL3Input: Initializing...");
            int version = SDL3.SDL_GetVersion();
            Debug.Log($"SDL3Input: Version: {SDL3.SDL_VERSIONNUM_MAJOR(version)}.{SDL3.SDL_VERSIONNUM_MINOR(version)}.{SDL3.SDL_VERSIONNUM_MICRO(version)}");

            // allow background input
            if (initAllowingBackgroundInput)
            {
                using (var flagString = new SDL_String(SDL3.SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS))
                using (var valueString = new SDL_String("1"))
                {
                    if (SDL3.SDL_SetHint(flagString.ptr, valueString.ptr) == 0)
                    {
                        Debug.LogError("SDLInput: SDL_SetHint failed");
                        return;
                    }
                }
            }

            // init SDL
            if (SDL3.SDL_Init(SDL3.SDL_INIT_GAMEPAD) == 0)
            {
                Debug.LogError("SDLInput: SDL_Init failed");
                return;
            }

            // load mappings
            if (loadGamepadMappings)
            {
                string mappingPath = Path.Combine(Application.streamingAssetsPath, "SDL", "gamecontrollerdb.txt").Replace('/', Path.DirectorySeparatorChar);
                if (File.Exists(mappingPath))
                {
                    Debug.Log("SDLInput: Loading gamecontrollerdb.txt");
                    using (var mappingPathString = new SDL_String(mappingPath))
                    {
                        int result = SDL3.SDL_AddGamepadMappingsFromFile(mappingPathString.ptr);
                        if (result < 0) Debug.LogError("SDLInput: SDL_AddGamepadMappingsFromFile failed");
                        else Debug.Log("SDLInput: Mappings loaded: " + result.ToString());
                    }
                }

                mappingPath = Path.Combine(Application.streamingAssetsPath, "SDL", "gamecontrollerdb.local.txt").Replace('/', Path.DirectorySeparatorChar);
                if (File.Exists(mappingPath))
                {
                    Debug.Log("SDLInput: Loading gamecontrollerdb.local.txt");
                    using (var mappingPathString = new SDL_String(mappingPath))
                    {
                        int result = SDL3.SDL_AddGamepadMappingsFromFile(mappingPathString.ptr);
                        if (result < 0) Debug.LogError("SDLInput: SDL_AddGamepadMappingsFromFile failed");
                        else Debug.Log("SDLInput: Mappings loaded: " + result.ToString());
                    }
                }
            }

            isInit = true;
            Debug.Log("SDLInput: Init");
		}

        private void OnDestroy()
        {
            Debug.Log("SDL3Input: shutdown");
            SDL3.SDL_Quit();
        }

        private void Update()
        {
            if (!isInit) return;

            SDL3.SDL_Event e;
            while (SDL3.SDL_PollEvent(&e) != 0)
            {
                switch (e.type)
                {
                    case (int)SDL3.SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
                    {
                        SDL_JoystickID id = e.gdevice.which;
                        SDL_Gamepad device = SDL3.SDL_OpenGamepad(id);
                        if (device != IntPtr.Zero)
                        {
                            var gamepad = new SDLGamepad(device);
                            gamepads.Add(gamepad);
                            DevicesChangedCallback?.Invoke(gamepad, true);
                        }
                    }
                    break;

                    case (int)SDL3.SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
                    {
                        SDL_JoystickID id = e.gdevice.which;
                        SDL_Gamepad device = SDL3.SDL_GetGamepadFromID(id);
                        var gamepad = FindGamepad(device);
                        if (gamepad != null)
                        {
                            Debug.Log($"SDLInput: Gamepad disconnected: '{gamepad.name}'");
                            gamepads.Remove(gamepad);
                            DevicesChangedCallback?.Invoke(gamepad, false);
                        }
                    }
                    break;

                    case (int)SDL3.SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
                    case (int)SDL3.SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
                    {
                        SDL_JoystickID id = e.gbutton.which;
                        SDL_Gamepad device = SDL3.SDL_OpenGamepad(id);
                        if (device != IntPtr.Zero)
                        {
                            var gamepad = FindGamepad(device);
                            if (gamepad != null) gamepad.UpdateButton((SDL3.SDL_GamepadButton)e.gbutton.button, e.gbutton.down);
                        }
                    }
                    break;

                    case (int)SDL3.SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
                    {
                        SDL_JoystickID id = e.gaxis.which;
                        SDL_Gamepad device = SDL3.SDL_OpenGamepad(id);
                        if (device != IntPtr.Zero)
                        {
                            var gamepad = FindGamepad(device);
                            if (gamepad != null) gamepad.UpdateAxis((SDL3.SDL_GamepadAxis)e.gaxis.axis, e.gaxis.value / (float)Sint16.MaxValue);
                        }
                    }
                    break;
                }
            }

            // update finish
            foreach (var gamepad in gamepads) gamepad.UpdateFinish();
        }

        private SDLGamepad FindGamepad(SDL_Gamepad device)
        {
            for (int i = 0; i != gamepads.Count; ++i)
            {
                if (gamepads[i].device == device) return gamepads[i];
            }
            return null;
        }
	}
}