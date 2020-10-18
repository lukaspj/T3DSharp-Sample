using T3DSharpFramework.Engine;
using T3DSharpFramework.Engine.Util;
using T3DSharpFramework.Generated.Classes.Sim;
using T3DSharpFramework.Generated.Functions;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection.Client {
   [ConsoleClass]
   public class ScoreBoardGUI : GuiControl {
      public static GuiTextListCtrl ScoreBoardList => Sim.FindObject<GuiTextListCtrl>("ScoreBoardGUIList");

      public static void Init() {
         Core.Functions.AddMessageCallback("MsgClientJoin".Tag(), "SBGuiPlayerJoined");
         Core.Functions.AddMessageCallback("MsgClientDrop".Tag(), "SBGuiPlayerLeft");
         Core.Functions.AddMessageCallback("MsgCoinPickedUp".Tag(), "SBGuiScoreChanged");
      }

      public void Toggle() {
         if (IsAwake()) {
            Core.Objects.Canvas.PopDialog(this);
         } else {
            Core.Objects.Canvas.PushDialog(this);
         }
      }

      public new void Clear() {
         // Delegate to the internal list
         ScoreBoardList.Clear();
      }

      public void AddPlayer(int clientId, string name, int score, int kills, int deaths) {
         string text = $"{Global.StripMLControlChars(name)}\t{score}\t{kills}\t{deaths}";

         // Update or add the player to the control
         if (ScoreBoardList.GetRowNumById(clientId) == -1) {
            ScoreBoardList.AddRow(clientId, text);
         } else {
            ScoreBoardList.SetRowById(clientId, text);
         }

         // Sorts by score
         ScoreBoardList.SortNumerical(1, false);
         ScoreBoardList.ClearSelection();
      }

      public void RemovePlayer(int clientId) {
         ScoreBoardList.RemoveRowById(clientId);
      }

      public void UpdatePlayer(int clientId, int score, int kills, int deaths) {
         string text = ScoreBoardList.GetRowTextById(clientId);
         text = Global.SetField(text, 1, score.ToString());
         text = Global.SetField(text, 2, kills.ToString());
         text = Global.SetField(text, 3, deaths.ToString());
         ScoreBoardList.SetRowById(clientId, text);

         ScoreBoardList.SortNumerical(1, false);
         ScoreBoardList.ClearSelection();
      }

      public void ResetScores() {
         for (int i = 0; i < ScoreBoardList.RowCount(); i++) {
            string text = ScoreBoardList.GetRowText(i);
            text = Global.SetField(text, 1, "0");
            text = Global.SetField(text, 2, "0");
            text = Global.SetField(text, 3, "0");
            ScoreBoardList.SetRowById(ScoreBoardList.GetRowId(i), text);
         }

         ScoreBoardList.ClearSelection();
      }

      [ConsoleFunction]
      public static void SBGuiPlayerJoined(string msgType, string msgString, string clientName, int clientId, int score,
         int kills, int deaths, bool isAI) {
         Sim.FindObject<ScoreBoardGUI>("ScoreBoardGUI").AddPlayer(clientId, clientName, 0, kills, deaths);
      }

      [ConsoleFunction]
      public static void SBGuiPlayerLeft(string msgType, string msgString, string clientName, int clientId) {
         Sim.FindObject<ScoreBoardGUI>("ScoreBoardGUI").RemovePlayer(clientId);
      }

      [ConsoleFunction]
      public static void SBGuiScoreChanged(string msgType, string msgString, string clientName, int clientId, int score,
         int kills, int deaths) {
         Sim.FindObject<ScoreBoardGUI>("ScoreBoardGUI").UpdatePlayer(clientId, score, kills, deaths);
      }
   }
}
