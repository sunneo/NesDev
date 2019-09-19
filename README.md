This project adopts following components:
* [nes-sdk](https://github.com/sunneo/nes-sdk/blob/master/README.md)
* [EmuNes](https://github.com/colinvella/EmuNes)
   * few modification for embeding into dockpanel
* [ScintillaNet](https://github.com/jacobslusser/ScintillaNET) - from nuget
* [dockpanelsuite](https://github.com/dockpanelsuite/dockpanelsuite) - from nuget
* [jint](https://github.com/sebastienros/jint) - from nuget
* [Json.net](https://github.com/JamesNK/Newtonsoft.Json) - from nuget
* [NAudio](https://github.com/naudio/NAudio) - from nuget
* [AForge.NET](https://github.com/andrewkirillov/AForge.NET) - from nuget
* [BumpKit](https://github.com/DataDink/Bumpkit)

initiate idea:
an IDE for nes, which provides code editor, debugger, program launcher, music/sound editor, and sprite editor.
Whereas scintilla will cover the code editor,  and an emulator will become a launcher and debugger. 

What if a function which can patch modifications on the fly? perhaps translating 6502 assembly into javascript functions, 
and modify context of javascript would able to hijack/patch at runtime. 

