#include "stdafx.h"
//#include <stdio.h>
#include "NativeHook.h"

#pragma data_seg (".SHARED")
// Windows message for communication between main executable and DLL module
UINT const WM_HOOK = WM_APP + 1;
// HWND of the main executable (managing application)
HWND hwndServer = NULL;
#pragma data_seg ()
#pragma comment (linker, "/section:.SHARED,RWS")

HINSTANCE instanceHandle;
HHOOK hookHandle;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		instanceHandle = hModule;
		hookHandle = NULL;

		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

// Keyboard Hook procedure
static LRESULT CALLBACK KeyboardProc(int code, WPARAM wParam, LPARAM lParam)
{
	// Report the event to the main window. If the return value is 1, block the input; otherwise pass it along the Hook Chain
	if (code == 0 && SendMessage(hwndServer, WM_HOOK, wParam, lParam)) return 1;

	return CallNextHookEx(hookHandle, code, wParam, lParam);
}

BOOL InstallHook(HWND hwndParent)
{
	if (hwndServer != NULL) return FALSE; // Already hooked

	// Register keyboard Hook
	hookHandle = SetWindowsHookEx(WH_KEYBOARD, (HOOKPROC)KeyboardProc, instanceHandle, 0);
	if (hookHandle == NULL) return FALSE;

	hwndServer = hwndParent;

	return TRUE;
}

BOOL UninstallHook()
{
	if (hookHandle == NULL) return TRUE;

	// If unhook attempt fails, check whether it is because of invalid handle (in that case continue)
	if (!UnhookWindowsHookEx(hookHandle))
	{
		DWORD error = GetLastError();

		if (error != ERROR_INVALID_HOOK_HANDLE) return FALSE;
	}

	hwndServer = NULL;
	hookHandle = NULL;

	return TRUE;
}