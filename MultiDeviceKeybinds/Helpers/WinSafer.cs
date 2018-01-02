using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MultiDeviceKeybinds
{
    /// <summary>
    /// Specifies the behaviour of the SaferComputeTokenFromLevel method
    /// </summary>
    internal enum SaferTokenBehaviour : uint
    {
        /// <summary></summary>
        Default = 0x0,
        /// <summary>If the OutAccessToken parameter is not more restrictive than the InAccessToken parameter, the OutAccessToken parameter returns NULL.</summary>
        NullIfEqual = 0x1,
        /// <summary></summary>
        CompareOnly = 0x2,
        /// <summary></summary>
        MakeInert = 0x4,
        /// <summary></summary>
        WantFlags = 0x8
    }

    /// <summary>
    /// The level of the handle to be opened.
    /// </summary>
    internal enum SaferLevel : uint
    {
        /// <summary>Software will not run, regardless of the user rights of the user.</summary>
        Disallowed = 0,
        /// <summary>Allows programs to execute with access only to resources granted to open well-known groups, blocking access to Administrator and Power User privileges and personally granted rights.</summary>
        Untrusted = 0x1000,
        /// <summary>Software cannot access certain resources, such as cryptographic keys and credentials, regardless of the user rights of the user.</summary>
        Constrained = 0x10000,
        /// <summary>Allows programs to execute as a user that does not have Administrator or Power User user rights. Software can access resources accessible by normal users.</summary>
        NormalUser = 0x20000,
        /// <summary>Software user rights are determined by the user rights of the user.</summary>
        FullyTrusted = 0x40000
    }

    /// <summary>
    /// The scope of the level to be created.
    /// </summary>
    internal enum SaferLevelScope : uint
    {
        /// <summary>The created level is scoped by computer.</summary>
        Machine = 1,
        /// <summary>The created level is scoped by user.</summary>
        User = 2
    }

    internal enum SaferOpen : uint
    {
        Open = 1
    }

    [Flags]
    internal enum CreateProcessFlags : uint
    {
        DEBUG_PROCESS = 0x00000001,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        CREATE_SUSPENDED = 0x00000004,
        DETACHED_PROCESS = 0x00000008,
        CREATE_NEW_CONSOLE = 0x00000010,
        NORMAL_PRIORITY_CLASS = 0x00000020,
        IDLE_PRIORITY_CLASS = 0x00000040,
        HIGH_PRIORITY_CLASS = 0x00000080,
        REALTIME_PRIORITY_CLASS = 0x00000100,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_FORCEDOS = 0x00002000,
        BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
        ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
        INHERIT_PARENT_AFFINITY = 0x00010000,
        INHERIT_CALLER_PRIORITY = 0x00020000,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
        PROCESS_MODE_BACKGROUND_END = 0x00200000,
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000,
        PROFILE_USER = 0x10000000,
        PROFILE_KERNEL = 0x20000000,
        PROFILE_SERVER = 0x40000000,
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct STARTUPINFO
    {
        public Int32 cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
        public Int32 dwXCountChars;
        public Int32 dwYCountChars;
        public Int32 dwFillAttribute;
        public Int32 dwFlags;
        public Int16 wShowWindow;
        public Int16 cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    /// <summary> 
    /// The structure represents a security identifier (SID) and its  
    /// attributes. SIDs are used to uniquely identify users or groups. 
    /// </summary> 
    [StructLayout(LayoutKind.Sequential)]
    internal struct SID_AND_ATTRIBUTES
    {
        public IntPtr Sid;
        public UInt32 Attributes;
    }

    /// <summary> 
    /// The structure specifies the mandatory integrity level for a token. 
    /// </summary> 
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_MANDATORY_LABEL
    {
        public SID_AND_ATTRIBUTES Label;
    }

    internal enum TOKEN_INFORMATION_CLASS
    {
        /// <summary>
        /// The buffer receives a TOKEN_USER structure that contains the user account of the token.
        /// </summary>
        TokenUser = 1,

        /// <summary>
        /// The buffer receives a TOKEN_GROUPS structure that contains the group accounts associated with the token.
        /// </summary>
        TokenGroups,

        /// <summary>
        /// The buffer receives a TOKEN_PRIVILEGES structure that contains the privileges of the token.
        /// </summary>
        TokenPrivileges,

        /// <summary>
        /// The buffer receives a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly created objects.
        /// </summary>
        TokenOwner,

        /// <summary>
        /// The buffer receives a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created objects.
        /// </summary>
        TokenPrimaryGroup,

        /// <summary>
        /// The buffer receives a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.
        /// </summary>
        TokenDefaultDacl,

        /// <summary>
        /// The buffer receives a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is needed to retrieve this information.
        /// </summary>
        TokenSource,

        /// <summary>
        /// The buffer receives a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.
        /// </summary>
        TokenType,

        /// <summary>
        /// The buffer receives a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If the access token is not an impersonation token, the function fails.
        /// </summary>
        TokenImpersonationLevel,

        /// <summary>
        /// The buffer receives a TOKEN_STATISTICS structure that contains various token statistics.
        /// </summary>
        TokenStatistics,

        /// <summary>
        /// The buffer receives a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.
        /// </summary>
        TokenRestrictedSids,

        /// <summary>
        /// The buffer receives a DWORD value that indicates the Terminal Services session identifier that is associated with the token. 
        /// </summary>
        TokenSessionId,

        /// <summary>
        /// The buffer receives a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the restricted SIDs, and the authentication ID associated with the token.
        /// </summary>
        TokenGroupsAndPrivileges,

        /// <summary>
        /// Reserved.
        /// </summary>
        TokenSessionReference,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.
        /// </summary>
        TokenSandBoxInert,

        /// <summary>
        /// Reserved.
        /// </summary>
        TokenAuditPolicy,

        /// <summary>
        /// The buffer receives a TOKEN_ORIGIN value. 
        /// </summary>
        TokenOrigin,

        /// <summary>
        /// The buffer receives a TOKEN_ELEVATION_TYPE value that specifies the elevation level of the token.
        /// </summary>
        TokenElevationType,

        /// <summary>
        /// The buffer receives a TOKEN_LINKED_TOKEN structure that contains a handle to another token that is linked to this token.
        /// </summary>
        TokenLinkedToken,

        /// <summary>
        /// The buffer receives a TOKEN_ELEVATION structure that specifies whether the token is elevated.
        /// </summary>
        TokenElevation,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if the token has ever been filtered.
        /// </summary>
        TokenHasRestrictions,

        /// <summary>
        /// The buffer receives a TOKEN_ACCESS_INFORMATION structure that specifies security information contained in the token.
        /// </summary>
        TokenAccessInformation,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if virtualization is allowed for the token.
        /// </summary>
        TokenVirtualizationAllowed,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if virtualization is enabled for the token.
        /// </summary>
        TokenVirtualizationEnabled,

        /// <summary>
        /// The buffer receives a TOKEN_MANDATORY_LABEL structure that specifies the token's integrity level. 
        /// </summary>
        TokenIntegrityLevel,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if the token has the UIAccess flag set.
        /// </summary>
        TokenUIAccess,

        /// <summary>
        /// The buffer receives a TOKEN_MANDATORY_POLICY structure that specifies the token's mandatory integrity policy.
        /// </summary>
        TokenMandatoryPolicy,

        /// <summary>
        /// The buffer receives the token's logon security identifier (SID).
        /// </summary>
        TokenLogonSid,

        /// <summary>
        /// The maximum value for this enumeration
        /// </summary>
        MaxTokenInfoClass
    }

    internal enum IntegrityLevel
    {
        Low = 1,
        Medium,
        High,
        System
    }

    internal class WinSafer
    {
        /// <summary>
        /// The SaferCreateLevel function opens a SAFER_LEVEL_HANDLE.
        /// </summary>
        /// <param name="scopeId">The scope of the level to be created.</param>
        /// <param name="levelId">The level of the handle to be opened.</param>
        /// <param name="openFlags">Must be SaferOpenFlags.Open</param>
        /// <param name="levelHandle">The returned SAFER_LEVEL_HANDLE. When you have finished using the handle, release it by calling the SaferCloseLevel function.</param>
        /// <param name="reserved">This parameter is reserved for future use. IntPtr.Zero</param>
        /// <returns></returns>
        [DllImport("advapi32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SaferCreateLevel(SaferLevelScope scopeId, SaferLevel levelId, SaferOpen openFlags, out IntPtr levelHandle, IntPtr reserved);

        /// <summary>
        /// The SaferComputeTokenFromLevel function restricts a token using restrictions specified by a SAFER_LEVEL_HANDLE.
        /// </summary>
        /// <param name="levelHandle">SAFER_LEVEL_HANDLE that contains the restrictions to place on the input token. Do not pass handles with a LevelId of SAFER_LEVELID_FULLYTRUSTED or SAFER_LEVELID_DISALLOWED to this function. This is because SAFER_LEVELID_FULLYTRUSTED is unrestricted and SAFER_LEVELID_DISALLOWED does not contain a token.</param>
        /// <param name="inAccessToken">Token to be restricted. If this parameter is NULL, the token of the current thread will be used. If the current thread does not contain a token, the token of the current process is used.</param>
        /// <param name="outAccessToken">The resulting restricted token.</param>
        /// <param name="flags">Specifies the behavior of the method.</param>
        /// <param name="lpReserved">Reserved for future use. This parameter should be set to IntPtr.EmptyParam.</param>
        /// <returns></returns>
        [DllImport("advapi32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SaferComputeTokenFromLevel(IntPtr levelHandle, IntPtr inAccessToken, out IntPtr outAccessToken, SaferTokenBehaviour flags, IntPtr lpReserved);

        /// <summary>
        /// The SaferCloseLevel function closes a SAFER_LEVEL_HANDLE that was opened by using the SaferIdentifyLevel function or the SaferCreateLevel function.</summary>
        /// <param name="levelHandle">The SAFER_LEVEL_HANDLE to be closed.</param>
        /// <returns>TRUE if the function succeeds; otherwise, FALSE. For extended error information, call GetLastWin32Error.</returns>
        [DllImport("advapi32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SaferCloseLevel(IntPtr levelHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandles,
            CreateProcessFlags dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool ConvertStringSidToSid(string StringSid, out IntPtr ptrSid);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern Boolean SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength);

        [DllImport("advapi32.dll")]
        static extern uint GetLengthSid(IntPtr pSid);

        const uint SE_GROUP_INTEGRITY = 0x00000020;

        public static Process CreateSaferProcess(String fileName, string arguments, SaferLevel saferLevel, IntegrityLevel integrityLevel, CreateProcessFlags flags = CreateProcessFlags.CREATE_NEW_CONSOLE)
        {
            IntPtr saferLevelHandle = IntPtr.Zero;

            //Create a SaferLevel handle to match what was requested
            if (!SaferCreateLevel(SaferLevelScope.User, saferLevel, SaferOpen.Open, out saferLevelHandle, IntPtr.Zero)) throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                //Generate the access token to use, based on the safer level handle.
                IntPtr hToken = IntPtr.Zero;

                if (!SaferComputeTokenFromLevel(
                      saferLevelHandle,  // SAFER Level handle
                      IntPtr.Zero,       // NULL is current thread token.
                      out hToken,        // Target token
                      SaferTokenBehaviour.Default,      // No flags
                      IntPtr.Zero))      // Reserved
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // Get the Integrity SID
                IntPtr pIntegritySid = GetIntegritySid(integrityLevel);

                // Construct a structure describing the token integrity level
                var TIL = new TOKEN_MANDATORY_LABEL();
                TIL.Label.Attributes = SE_GROUP_INTEGRITY;
                TIL.Label.Sid = pIntegritySid;
                IntPtr pTIL = Marshal.AllocHGlobal(Marshal.SizeOf<TOKEN_MANDATORY_LABEL>());
                Marshal.StructureToPtr(TIL, pTIL, false);

                // Modify the token
                if (!SetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTIL, (uint)Marshal.SizeOf<TOKEN_MANDATORY_LABEL>() + GetLengthSid(pIntegritySid))) throw new Win32Exception();

                try
                {
                    //Now that we have a security token, we can lauch the process
                    //using the standard CreateProcessAsUser API
                    STARTUPINFO si = new STARTUPINFO();
                    si.cb = Marshal.SizeOf(si);
                    si.lpDesktop = String.Empty;

                    var processAttributes = new SECURITY_ATTRIBUTES();
                    var threadAttributes = new SECURITY_ATTRIBUTES();
                    // Spin up the new process
                    //bool result = CreateProcessAsUser(hToken, fileName, arguments,
                    bool result = CreateProcessAsUser(hToken, fileName, $"\"{fileName}\"{(arguments?.Length > 0 ? " " + arguments : "")}",
                          ref processAttributes,
                          ref threadAttributes,
                          false, //inherit handles
                          flags,
                          IntPtr.Zero, //environment
                          null, //current directory
                          ref si, //startup info
                          out PROCESS_INFORMATION pi); //process info

                    if (!result) throw new Win32Exception(Marshal.GetLastWin32Error());

                    if (pi.hProcess != IntPtr.Zero) CloseHandle(pi.hProcess);

                    if (pi.hThread != IntPtr.Zero) CloseHandle(pi.hThread);

                    return Process.GetProcessById(pi.dwProcessId);
                }
                finally
                {
                    if (hToken != IntPtr.Zero) CloseHandle(hToken);
                }
            }
            finally
            {
                SaferCloseLevel(saferLevelHandle);
            }
        }

        private static IntPtr GetIntegritySid(IntegrityLevel level) // https://msdn.microsoft.com/en-us/library/bb625963.aspx
        {
            if (!ConvertStringSidToSid($"S-1-16-{4096 * ((int)level)}", out IntPtr pIntegritySid)) throw new Win32Exception();

            return pIntegritySid;
        }
    }
}