using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Reign
{
	unsafe static class SDL3
    {
        private const string DLL = "SDL3.dll";
        public const uint SDL_INIT_JOYSTICK = 0x00000200;
        public const uint SDL_INIT_GAMEPAD = 0x00002000;

        public enum GamepadButton
        {
            South = 0,
            East,
            West,
            North,
            Back,
            Guide,
            Start,
            LeftStick,
            RightStick,
            LeftShoulder,
            RightShoulder,
            DPadUp,
            DPadDown,
            DPadLeft,
            DPadRight,
            Misc1,
            RightPaddle1,
            LeftPaddle1,
            RightPaddle2,
            LeftPaddle2,
            Touchpad,
            Misc2,
            Misc3,
            Misc4,
            Misc5,
            Misc6,
            Count
        }

        public enum GamepadButtonLabel
        {
            Unknown,
            A,
            B,
            X,
            Y,
            Cross,
            Circle,
            Square,
            Triangle
        }

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SDL_InitSubSystem(uint flags);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_QuitSubSystem(uint flags);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetError();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetJoysticks(out int count);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_free(IntPtr ptr);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetJoystickNameForID(uint id);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort SDL_GetJoystickVendorForID(uint id);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort SDL_GetJoystickProductForID(uint id);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_OpenJoystick(uint id);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_CloseJoystick(IntPtr joystick);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SDL_JoystickConnected(IntPtr joystick);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetJoystickEventsEnabled([MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_UpdateJoysticks();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetNumJoystickAxes(IntPtr joystick);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetNumJoystickButtons(IntPtr joystick);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetNumJoystickHats(IntPtr joystick);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern short SDL_GetJoystickAxis(IntPtr joystick, int axis);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SDL_GetJoystickButton(IntPtr joystick, int button);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte SDL_GetJoystickHat(IntPtr joystick, int hat);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_AddGamepadMappingsFromFile(byte *file);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SDL_IsGamepad(uint id);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_OpenGamepad(uint id);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_CloseGamepad(IntPtr gamepad);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_UpdateGamepads();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetGamepadEventsEnabled(
            [MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SDL_GamepadHasButton(
            IntPtr gamepad, GamepadButton button);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SDL_GetGamepadButton(
            IntPtr gamepad, GamepadButton button);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_GetGamepadStringForButton(
            GamepadButton button);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern GamepadButtonLabel SDL_GetGamepadButtonLabel(
            IntPtr gamepad, GamepadButton button);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
public static extern IntPtr SDL_GetGamepadMappingForID(uint id);
    }
}