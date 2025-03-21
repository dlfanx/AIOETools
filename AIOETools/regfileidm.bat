@echo off
:: Disable Group Policies by modifying the Registry
echo Disabling specified Group Policies...

:: Remove Task Manager
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\System" /v DisableTaskMgr /f

:: Prevent Access to Command Prompt
reg delete "HKCU\Software\Policies\Microsoft\Windows\System" /v DisableCMD /f

:: Prevent Registry Tools
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\System" /v DisableRegistryTools /f

:: Don't run specified Windows apps
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v DisallowRun /f

:: Run only specified Windows apps
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v RestrictRun /f

:: Disable Known Folders
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoKnownFolders /f

:: Allow only per user shell extensions
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v EnforceShellExtensionSecurity /f

:: Hide specified drives in My PC
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoDrives /f

:: Remove File menu from Explorer
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoFileMenu /f

:: Hide Manage item on File Explorer context menu
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoManageMyComputerVerb /f

:: Remove Shared Documents from My PC
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoSharedDocuments /f

:: Remove File Explorer default context menu (Disables Right-Click in Explorer)
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoViewContextMenu /f

:: Prevent access to drives from My PC
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoViewOnDrive /f

:: Prevent users from adding files to the root of their User Files folder
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoWritingToUSB /f

echo Group Policies have been disabled.
pause