using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Reign
{
    public unsafe class SDLJoystickLogger : MonoBehaviour
    {
        public int joystickIndex = 0;

        private IntPtr joystick;
        private bool initialized;

        private bool[] buttons;
        private int[] axes;
        private byte[] hats;

        
        private IntPtr gamepad = IntPtr.Zero;
        private bool[] mappedButtons = new bool[(int)SDL3.GamepadButton.Count];


        private void Start()
        {
            if (!SDL3.SDL_InitSubSystem(SDL3.SDL_INIT_JOYSTICK))
            {
                Debug.LogError("SDL3 Init failed: " +
                    Marshal.PtrToStringAnsi(SDL3.SDL_GetError()));
                return;
            }

            initialized = true;

            string mappingPath = System.IO.Path.Combine(Application.streamingAssetsPath, "gamecontrollerdb.txt");
            byte[] mappingPathData = Encoding.Default.GetBytes(mappingPath);
            fixed (byte* mappingPathDataPtr = mappingPathData)
            {
                int result = SDL3.SDL_AddGamepadMappingsFromFile(mappingPathDataPtr);
                Debug.Log("Mapping result: " + result);
                //if (result >= 0)
                //{
                //    int added = SDL3.SDL_AddGamepadMappingsFromFile(path);
                //    if (added < 0)
                //    {
                //        Debug.LogError("SDL3 mapping load failed: " +
                //            Marshal.PtrToStringAnsi(SDL3.SDL_GetError()));
                //    }
                //    else
                //    {
                //        Debug.Log($"SDL3 loaded {added} mappings");
                //    }
                //}
            }

            SDL3.SDL_SetJoystickEventsEnabled(false);

            IntPtr ids = SDL3.SDL_GetJoysticks(out int count);
            uint selectedID = 0;

            try
            {
                Debug.Log($"SDL3 found {count} joystick(s)");

                for (int i = 0; i < count; i++)
                {
                    uint id = unchecked((uint)Marshal.ReadInt32(ids, i * 4));

                    string name = Marshal.PtrToStringAnsi(
                        SDL3.SDL_GetJoystickNameForID(id));

                    ushort vid = SDL3.SDL_GetJoystickVendorForID(id);
                    ushort pid = SDL3.SDL_GetJoystickProductForID(id);

                    Debug.Log(
                        $"Joystick {i}: {name} VID={vid:X4} PID={pid:X4}");

                    if (i == joystickIndex)
                        selectedID = id;
                }
            }
            finally
            {
                if (ids != IntPtr.Zero)
                    SDL3.SDL_free(ids);
            }

            if (selectedID == 0)
            {
                Debug.LogWarning("Joystick not found");
                return;
            }

            if (SDL3.SDL_IsGamepad(selectedID))
            {
                gamepad = SDL3.SDL_OpenGamepad(selectedID);

                if (gamepad != IntPtr.Zero)
                {
                    SDL3.SDL_SetGamepadEventsEnabled(false);
                    Debug.Log("Gamepad mapping available");
                }
                else
                {
                    Debug.LogWarning("No Gamepad mapping available");
                }
            }
            else
            {
                Debug.LogWarning("No SDL3 gamepad mapping available");
            }

            joystick = SDL3.SDL_OpenJoystick(selectedID);

            if (joystick == IntPtr.Zero)
            {
                Debug.LogError("SDL3 OpenJoystick failed: " +
                    Marshal.PtrToStringAnsi(SDL3.SDL_GetError()));
                return;
            }

            buttons = new bool[Math.Max(0, SDL3.SDL_GetNumJoystickButtons(joystick))];
            axes = new int[Math.Max(0, SDL3.SDL_GetNumJoystickAxes(joystick))];
            hats = new byte[Math.Max(0, SDL3.SDL_GetNumJoystickHats(joystick))];

            Debug.Log(
                $"Opened joystick: Buttons={buttons.Length}, " +
                $"Axes={axes.Length}, Hats={hats.Length}");
        }

        private void Update()
        {
            if (joystick == IntPtr.Zero)
                return;

            SDL3.SDL_UpdateJoysticks();

            if (!SDL3.SDL_JoystickConnected(joystick))
            {
                Debug.LogWarning("Joystick disconnected");
                SDL3.SDL_CloseJoystick(joystick);
                joystick = IntPtr.Zero;
                return;
            }

            // Buttons
            for (int i = 0; i < buttons.Length; i++)
            {
                bool value = SDL3.SDL_GetJoystickButton(joystick, i);

                if (value != buttons[i])
                {
                    buttons[i] = value;
                    Debug.Log($"Button {i}: {(value ? "Pressed" : "Released")}");
                }
            }

            // Analog axes
            for (int i = 0; i < axes.Length; i++)
            {
                short value = SDL3.SDL_GetJoystickAxis(joystick, i);

                // Quantize to reduce noisy logs.
                int step = Mathf.RoundToInt(value / 4096f);

                if (step != axes[i])
                {
                    axes[i] = step;
                    Debug.Log($"Axis {i}: {value}");
                }
            }

            // D-pad / POV hats
            for (int i = 0; i < hats.Length; i++)
            {
                byte value = SDL3.SDL_GetJoystickHat(joystick, i);

                if (value != hats[i])
                {
                    hats[i] = value;
                    Debug.Log($"Hat {i}: {value}");
                }
            }
        }

        private void OnDestroy()
        {
            if (joystick != IntPtr.Zero)
            {
                SDL3.SDL_CloseJoystick(joystick);
                joystick = IntPtr.Zero;
            }

            if (initialized)
            {
                SDL3.SDL_QuitSubSystem(SDL3.SDL_INIT_JOYSTICK);
                initialized = false;
            }
        }
    }
}