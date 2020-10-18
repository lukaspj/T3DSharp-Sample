using T3DSharpFramework.Engine;
using T3DSharpFramework.Generated.Classes.Sim;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection.Server {
   [ConsoleClass]
   public class CoinCollector : Player {
      public CoinCollectionGameConnection Client {
         get => Sim.FindObject<CoinCollectionGameConnection>(GetFieldValue("client"));
         set => SetFieldValue("client", GenericMarshal.ToString(value));
      }
   }
}
