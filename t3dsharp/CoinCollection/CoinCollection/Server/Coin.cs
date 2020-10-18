using System.Collections.Generic;
using T3DSharpFramework.Engine;
using T3DSharpFramework.Engine.Util;
using T3DSharpFramework.Generated.Classes.Sim;
using T3DSharpFramework.Generated.Functions;
using T3DSharpFramework.Generated.Structs.Math;
using T3DSharpFramework.Interop;
using Point3F = T3DSharpFramework.Engine.Structs.Point3F;

namespace CoinCollection.CoinCollection.Server {
   [ConsoleClass]
   public class Coin : StaticShapeData {
      public void OnCollision(StaticShape coin, CoinCollector other, Point3F vec, float len) {
         ParticleEmitterNode emitterNode = new ParticleEmitterNode {
            DataBlock = Sim.FindObject<ParticleEmitterNodeData>("CoinNode"),
            Emitter = Sim.FindObject<ParticleEmitterData>("CoinEmitter"),
            Position = coin.GetPosition()
         };
         emitterNode.RegisterObject();
         emitterNode.Schedule("200", "delete");
         coin.Delete();

         SimGroup Coins = Sim.FindObject<SimGroup>("Coins");
         other.Client.CoinsFound++;
         Core.Functions.MessageAll("MsgCoinPickedUp".Tag(), "-1", other.Client.PlayerName,
            other.Client, other.Client.CoinsFound.ToString(), other.Client.Kills.ToString(), other.Client.Deaths.ToString());
         if (Coins is null || Coins.GetCount() > 0) return;
         CoinCollectionGameConnection winnerClient =
            Core.Objects.ClientGroup.GetObject(0).As<CoinCollectionGameConnection>();

         List<CoinCollectionGameConnection> loserClients = new List<CoinCollectionGameConnection>();
         for (uint i = 1; i < Core.Objects.ClientGroup.GetCount(); i++) {
            var client = Core.Objects.ClientGroup.GetObject(i).As<CoinCollectionGameConnection>();
            if (client.CoinsFound > winnerClient.CoinsFound) {
               loserClients.Add(winnerClient);
               winnerClient = client;
            } else {
               loserClients.Add(client);
            }
         }

         loserClients.ForEach(client =>
            Global.CommandToClient(
               client,
               "ShowDefeat".Tag(),
               client.CoinsFound.ToString()
            ));

         Global.CommandToClient(
            winnerClient,
            "ShowVictory".Tag(),
            winnerClient.CoinsFound.ToString());
      }
   }
}
