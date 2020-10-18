using T3DSharpFramework.Generated.Classes.Sim;
using T3DSharpFramework.Generated.Classes.Sim.Net;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection.Server {
   [ConsoleClass]
   public class CoinCollectionGameConnection : GameConnection {
      public string ConnectData {
         get => GetFieldValue("connectData");
         set => SetFieldValue("connectData", value);
      }

      public Player Player {
         get => GenericMarshal.StringTo<Player>(GetFieldValue("player"));
         set => SetFieldValue("player", GenericMarshal.ToString(value));
      }

      public int CoinsFound {
         get => GenericMarshal.StringTo<int>(GetFieldValue("coinsfound"));
         set => SetFieldValue("coinsfound", GenericMarshal.ToString(value));
      }

      public int Kills {
         get => GenericMarshal.StringTo<int>(GetFieldValue("kills"));
         set => SetFieldValue("kills", GenericMarshal.ToString(value));
      }

      public int Deaths {
         get => GenericMarshal.StringTo<int>(GetFieldValue("deaths"));
         set => SetFieldValue("deaths", GenericMarshal.ToString(value));
      }

      public string PlayerName {
         get => GetFieldValue("playerName");
         set => SetFieldValue("playerName", value);
      }

      public void SetPlayerName(string name) {
         Call("SetPlayerName", name);
      }
   }
}
