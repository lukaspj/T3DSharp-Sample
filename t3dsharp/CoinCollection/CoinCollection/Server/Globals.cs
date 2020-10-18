using T3DSharpFramework.Generated.Functions;

namespace CoinCollection.CoinCollection.Server {
   public class Globals {
      public static int CoinsFound {
         get => Global.GetConsoleInt("CoinsFound");
         set => Global.SetConsoleInt("CoinsFound", value);
      }
   }
}
