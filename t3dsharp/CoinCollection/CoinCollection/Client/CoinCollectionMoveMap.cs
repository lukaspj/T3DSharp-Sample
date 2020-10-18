using T3DSharpFramework.Engine;
using T3DSharpFramework.Generated.Classes.Sim;
using T3DSharpFramework.Generated.Functions;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection.Client {
   [ConsoleClass]
   public class CoinCollectionMoveMap : ActionMap {
      private CoinCollectionMoveMap(string name) : base(name) {
         SetFieldValue("humanReadableName", "Coin Collection Movement");

         BindKeyboard("a", "MoveLeft");
         BindKeyboard("d", "MoveRight");
         BindKeyboard("left", "MoveLeft");
         BindKeyboard("right", "MoveRight");

         BindKeyboard("w", "MoveForward");
         BindKeyboard("s", "MoveBackward");
         BindKeyboard("up", "MoveForward");
         BindKeyboard("down", "MoveBackward");

         BindKeyboard("space", "Jump");

         BindKeyboard("f", "ShowScoreBoard");
      }

      private static int RemapCount {
         get => Global.GetConsoleInt("RemapCount");
         set => Global.SetConsoleInt("RemapCount", value);
      }

      private static void RemapKeyboard(string name, string command, string description) {
         Global.SetConsoleString($"RemapName[{RemapCount}]", name);
         Global.SetConsoleString($"RemapCmd[{RemapCount}]", command);
         Global.SetConsoleString($"RemapActionMap[{RemapCount}]", "CoinCollectionMoveMap");
         Global.SetConsoleString($"RemapDevice[{RemapCount}]", "keyboard");
         Global.SetConsoleString($"RemapDescription[{RemapCount}]", description);
         RemapCount++;
      }

      public static void Init() {
         RemapKeyboard("Forward", "MoveForward", "Forward Movement");
         RemapKeyboard("Backward", "MoveBackward", "Backward Movement");
         RemapKeyboard("Strafe Left", "MoveLeft", "Left Strafing Movement");
         RemapKeyboard("Strafe Right", "MoveRight", "Right Strafing Movement");
         RemapKeyboard("Jump", "Jump", "Jump");

         if (Global.IsObject("CoinCollectionMoveMap")) {
            Sim.FindObject<ActionMap>("CoinCollectionMoveMap").Delete();
         }

         var CoinCollectionMoveMap = new CoinCollectionMoveMap("CoinCollectionMoveMap");
         CoinCollectionMoveMap.RegisterObject();
         CoinCollectionMoveMap.Push();
      }

      public void BindKeyboard(string key, string function) {
         Bind("keyboard", key, function);
      }
   }
}
