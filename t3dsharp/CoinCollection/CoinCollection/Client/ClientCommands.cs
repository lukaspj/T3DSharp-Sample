using CoinCollection.Core;
using T3DSharpFramework.Generated.Functions;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection.Client {
   public class ClientCommands {
      [ConsoleFunction]
      public static void ClientCmdShowVictory(int score) {
         Functions.MessageBoxOK(
            "You Win!",
            $"Congratulations you found {score} coins!",
            "disconnect();"
            );
      }
   }
}
