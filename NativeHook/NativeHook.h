/*#ifdef NATIVEHOOK_EXPORTS
#define NATIVEHOOK_API __declspec(dllexport)
#else
#define NATIVEHOOK_API __declspec(dllimport)
#endif

extern "C" NATIVEHOOK_API BOOL InstallHook(HWND hwndParent);

extern "C" NATIVEHOOK_API BOOL UninstallHook();*/

extern "C" __declspec(dllexport) BOOL InstallHook(HWND hwndParent);

extern "C" __declspec(dllexport) BOOL UninstallHook();