using CoinCollection.CoinCollection.Client;
using CoinCollection.Core;
using T3DSharpFramework.Generated.Functions;
using T3DSharpFramework.Generated.Classes.Sim;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection {
   /// <summary>
   /// The CoinCollection ModuleDefinition is linked to the CoinCollection.module file.
   /// This is the underlying code for the CoinCollection module.
   /// </summary>
   [ConsoleClass]
   class CoinCollection : ModuleDefinition, IModuleDefinition {
      public void OnCreate() { }

      public void OnDestroy() { }

      public void InitServer() {
         // server initialization
      }

      public void OnCreateGameServer() {
         Call("registerDatablock", "./datablocks/Coin.cs");
         Call("registerDatablock", "./datablocks/Particles.cs");
         Call("registerDatablock", "./datablocks/Player.cs");
      }

      public void OnDestroyGameServer() { }

      public void InitClient() {
         Call("queueExec", "./guis/customProfiles.cs");
         Call("queueExec", "./guis/scoreboard.gui");
         ScoreBoardGUI.Init();
      }

      public void OnCreateClientConnection() {
         //client initialization

         CoinCollectionMoveMap.Init();
         Core.Objects.GlobalActionMap.Bind("keyboard", "tilde", "toggleConsole");

         string prefPath = Global.GetPrefsPath();
         if (Global.IsFile(prefPath + "/keybinds.cs"))
            Global.Exec(prefPath + "/keybinds.cs");
      }

      public void OnDestroyClientConnection() { }
   }
}
