using System;
using T3DSharpFramework.Engine;
using T3DSharpFramework.Generated.Functions;
using T3DSharpFramework.Interop;

namespace CoinCollection.CoinCollection.Client {
   public class InputCommands {
      public static float MovementSpeed {
         get => Global.GetConsoleFloat("movementSpeed");
         set => Global.SetConsoleFloat("movementSpeed", value);
      }

      [ConsoleFunction]
      public static void EscapeFromGame() {
         Global.Call("disconnect");
      }

      [ConsoleFunction]
      public static void SetSpeed(float speed) {
         if (speed > 0) {
            MovementSpeed = speed;
         }
      }

      [ConsoleFunction]
      public static void MoveLeft(float val) {
         Global.SetConsoleFloat("mvLeftAction", val * MovementSpeed);
      }

      [ConsoleFunction]
      public static void MoveRight(float val) {
         Global.SetConsoleFloat("mvRightAction", val * MovementSpeed);
      }

      [ConsoleFunction]
      public static void MoveForward(float val) {
         Global.SetConsoleFloat("mvForwardAction", val * MovementSpeed);
      }

      [ConsoleFunction]
      public static void MoveBackward(float val) {
         Global.SetConsoleFloat("mvBackwardAction", val * MovementSpeed);
      }

      [ConsoleFunction]
      public static float GetMouseAdjustAmount(float val) {
         return val * (Global.GetConsoleFloat("cameraFov") / 90.0f) * 0.01f * Global.GetConsoleFloat("pref::Input::LinkMouseSensitivity");
      }

      [ConsoleFunction]
      public static void Yaw(float val) {
         var yawAdj = GetMouseAdjustAmount(val);
         if (Core.Objects.ServerConnection.IsControlObjectRotDampedCamera()) {
            // Clamp and scale
            yawAdj = Math.Clamp(yawAdj, -2.0f * (float)Math.PI + 0.01f, 2.0f * (float)Math.PI - 0.01f);
            yawAdj *= 0.5f;
         }

         Global.SetConsoleFloat("mvYaw", Global.GetConsoleFloat("mvYaw") + yawAdj);
      }

      [ConsoleFunction]
      public static void Pitch(float val) {
         var pitchAdj = GetMouseAdjustAmount(val);
         if (Core.Objects.ServerConnection.IsControlObjectRotDampedCamera()) {
            // Clamp and scale
            pitchAdj = Math.Clamp(pitchAdj, -2.0f * (float)Math.PI + 0.01f, 2.0f * (float)Math.PI - 0.01f);
            pitchAdj *= 0.5f;
         }

         Global.SetConsoleFloat("mvPitch", Global.GetConsoleFloat("mvPitch") + pitchAdj);
      }

      [ConsoleFunction]
      public static void Jump(float val) {
         Global.SetConsoleInt("mvTriggerCount2", Global.GetConsoleInt("mvTriggerCount2") + 1);
      }

      [ConsoleFunction]
      public static void ShowScoreBoard(float val) {
         if (val > 0) {
            Sim.FindObjectByName<ScoreBoardGUI>("ScoreBoardGUI").Toggle();
         }
      }
   }
}
