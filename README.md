# anti-gg
A "Bypass" for nProtect GameGuard.
For educational purposes only.
Last tested working with Drift City in ~2010.  I couldn't find a game that still uses this anticheat.

# Usage
Start anti-gg.exe the moment that GameMon.exe starts.
Anytime the game freezes and refuses to move forward, tap the Numpad 0 key (the unpause key).
Only press the unpause key as necessary.

# How this works
This will repeatedly suspend and resume all threads in the GameMon process using win32 API Win32::SuspendThread and Win32::ResumeThread

Currently configured to unpause for 10ms every minute to avoid halting the game when it tries to access necessary GG functions.

Every game will be different but the default values seem to have worked fairly well for Drift City.

GG needs time to initialize and start performing its duties.  This makes that initialization process take over 2 hours when last tested.

During this time window, you can use any detected utility.  Default Cheat Engine's Speedhack function works.
