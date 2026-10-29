using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Reign
{
    [DefaultExecutionOrder(-32000)]
    public unsafe class SDLInput : MonoBehaviour
    {
        public static List<SDLGamepad> gamepads { get; private set; }

        public delegate void DevicesChangedCallbackMethod();
        public static event DevicesChangedCallbackMethod DevicesChangedCallback;
        
        private static bool devicesChanged;

        private void RefreshDevices()
        {
            Debug.Log("SDLInput: RefreshDevices");
            gamepads = new List<SDLGamepad>();
            

            DevicesChangedCallback?.Invoke();
        }

		private void Start()
		{
            // allow background input
            /*using (var flagString = new SDL_String(SDL3.SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS))
            using (var valueString = new SDL_String("1"))
            {
                if (SDL3.SDL_SetHint(flagString.ptr, valueString.ptr) == 0)
                {
                    Debug.LogError("SDLInput: SDL_SetHint failed");
                    return;
                }
            }*/

            // init SDL
            if (SDL3.SDL_Init(SDL3.SDL_INIT_GAMEPAD) == 0)
            {
                Debug.LogError("SDLInput: SDL_Init failed");
                return;
            }

            // load mappings
            string mappingPath = System.IO.Path.Combine(Application.streamingAssetsPath, "SDL", "gamecontrollerdb.txt");
            if (File.Exists(mappingPath))
            {
                using (var mappingPath = new SDL_String(mappingPath))
                {
                    int result = SDL3.SDL_AddGamepadMappingsFromFile(mappingPath.ptr);
                    if (result < 0) Debug.LogError("SDLInput: SDL_AddGamepadMappingsFromFile failed");
                }
            }

            RefreshDevices();
		}

        private void OnDestroy()
        {
            
        }

        private void Update()
        {
            // refresh devices
            if (devicesChanged)
            {
                devicesChanged = false;
                RefreshDevices();
            }

            // collect input from all devices
            foreach (var gamepad in gamepads)
            {
                gamepad.Update();
            }
        }
	}
}