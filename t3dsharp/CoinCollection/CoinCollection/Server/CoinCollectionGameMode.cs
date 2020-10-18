using CoinCollection.CoinCollection.Client;
using T3DSharpFramework.Engine;
using T3DSharpFramework.Engine.Util;
using T3DSharpFramework.Generated.Classes.Sim;
using T3DSharpFramework.Generated.Functions;
using T3DSharpFramework.Generated.Structs.Math;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection.Server {
   [ConsoleClass]
   public class CoinCollectionGameMode : ScriptObject {
      private CoinCollectionGameMode(string name) : base(name) { }

      public int Duration {
         get => GenericMarshal.StringTo<int>(GetFieldValue("duration").OrDefault("0"));
         set => SetFieldValue("duration", GenericMarshal.ToString(value));
      }

      public bool Running {
         get => GenericMarshal.StringTo<bool>(GetFieldValue("running").OrDefault("false"));
         set => SetFieldValue("running", GenericMarshal.ToString(value));
      }

      public int GameSchedule {
         get => GenericMarshal.StringTo<int>(GetFieldValue("gameSchedule").OrDefault("-1"));
         set => SetFieldValue("gameSchedule", GenericMarshal.ToString(value));
      }

      public static ScriptObject OnCreateGame() {
         ScriptObject gameMode = new CoinCollectionGameMode("CoinCollectionGameMode");
         gameMode.RegisterObject();
         return gameMode;
      }

      public void OnMissionStart() {
         // Set up the game and game variables
         InitGameVars();

         if (Running) {
            Global.Error("onMissionStart: End the game first!");
            return;
         }

         // Start the game timer
         if (Duration > 0) {
            GameSchedule = Global.Schedule((Duration * 1000).ToString(), "0", "onGameDurationEnd");
         }

         Running = true;
      }

      public void OnMissionEnded() {
         if (!Running) {
            Global.Error("onMissionEnded: No game running!");
            return;
         }

         // Stop game timer
         Global.Cancel(GameSchedule);

         Running = false;
      }

      public void OnMissionReset() {
         InitGameVars();
      }

      public void OnGameDurationEnd() {
         if (Duration > 0 && !(Core.Editor.EditorIsActive() && Core.Editor.GuiEditorIsActive())) {
            OnMissionEnded();
         }
      }

      public void OnClientConnect(CoinCollectionGameConnection client) { }

      public void OnClientEnterGame(CoinCollectionGameConnection client) {
         // Set the player name based on the client's connection data
         client.SetPlayerName(client.ConnectData);
         client.CoinsFound = 0;
         client.Deaths = 0;
         client.Kills = 0;

         SpawnControlObject(client);

         // Inform the client of all the other clients
         for (uint i = 0; i < Core.Objects.ClientGroup.GetCount(); i++) {
            var other = Core.Objects.ClientGroup.GetObject(i).As<CoinCollectionGameConnection>();
            if (other.GetId() != client.GetId()) {
               Core.Functions.MessageClient(
                  client, "MsgClientJoin".Tag(), "",
                  other.PlayerName,
                  other,
                  other.CoinsFound.ToString(),
                  other.Kills.ToString(),
                  other.Deaths.ToString(),
                  other.IsAIControlled().ToString());
            }
         }

         // Inform the client we've joined up
         Core.Functions.MessageClient(
            client,
            "MsgClientJoin".Tag(),
            "\\c2Welcome to the Torque demo app %1.".ColorEscape().Tag(),
            client.PlayerName,
            client,
            client.CoinsFound.ToString(),
            client.Kills.ToString(),
            client.Deaths.ToString(),
            client.IsAIControlled().ToString()
         );

         // Inform all the other clients of the new guy
         Core.Functions.MessageAllExcept(
            client,
            "-1",
            "MsgClientJoin".Tag(),
            "\\c1 % 1 joined the game.".ColorEscape().Tag(),
            client,
            client.CoinsFound.ToString(),
            client.Kills.ToString(),
            client.Deaths.ToString(),
            client.IsAIControlled().ToString());
      }

      public void OnClientLeaveGame(CoinCollectionGameConnection client) {
         // Inform all the other clients that the client has left
         Core.Functions.MessageAllExcept(
            client,
            "-1",
            "MsgClientDrop".Tag(),
            "\\c1 % 1 left the game.".ColorEscape().Tag(),
            client,
            client.CoinsFound.ToString(),
            client.Kills.ToString(),
            client.Deaths.ToString(),
            client.IsAIControlled().ToString());
      }

      public void OnInitialControlSet() { }

      public void SpawnControlObject(CoinCollectionGameConnection client) {
         int playerId = Global.SpawnObject("Player", "CoinCollectorPlayer");
         if (!Global.IsObject(playerId.ToString())) {
            return;
         }

         CoinCollector player = Sim.FindObject<CoinCollector>(playerId);

         TransformF spawnTransform = new TransformF("0 0 1 0 0 0 0 0");
         SpawnSphere sphere = Sim.FindObject<SpawnSphere>("CoinCollectorSpawnSphere");
         if (sphere != null) {
            spawnTransform = sphere.GetTransform();
         }

         player.SetTransform(spawnTransform);
         player.Client = client;

         Core.Objects.MissionCleanup.Add(player);
         client.SetControlObject(player);
         client.Player = player;

         client.SetFirstPerson(false);
      }

      public void InitGameVars() {
         Duration = 30 * 60;
      }
   }
}
