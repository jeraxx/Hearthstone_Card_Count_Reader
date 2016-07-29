#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility.Logging;

#endregion

namespace Hearthstone_Deck_Tracker
{
    public static class Core
	{
		internal const int UpdateDelay = 100;
		private static OverlayWindow _overlay;
		private static int _updateRequestsPlayer;
		private static int _updateRequestsOpponent;
		public static Version Version { get; set; }
		public static GameV2 Game { get; set; }

		public static bool Initialized { get; private set; }

		public static OverlayWindow Overlay => _overlay ?? (_overlay = new OverlayWindow(Game));

		internal static bool UpdateOverlay { get; set; } = true;
		internal static bool Update { get; set; }
		internal static bool CanShutdown { get; set; }

		public static void Initialize()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			var newUser = !Directory.Exists(Config.AppDataPath);
			Config.Load();
			Log.Initialize();

			Game = new GameV2();

			UpdateOverlayAsync();
			Initialized = true;


		}

		private static async void UpdateOverlayAsync()
		{
			//if(Config.Instance.CheckForUpdates)
			//	Updater.CheckForUpdates(true);
			var hsForegroundChanged = false;
			while(UpdateOverlay)
			{
				if(User32.GetHearthstoneWindow() != IntPtr.Zero)
				{
					Overlay.UpdatePosition();

					if(!Game.IsRunning)
					{
						Overlay.Update(true);
					}
					Game.IsRunning = true;

				}
				else if(Game.IsRunning)
				{
					Game.IsRunning = false;
					Overlay.ShowOverlay(false);

					Log.Info("Exited game");
					Game.CurrentRegion = Region.UNKNOWN;
					Log.Info("Reset region");
					await Reset();
					Game.IsInMenu = true;
					Overlay.HideRestartRequiredWarning();
				}
				await Task.Delay(UpdateDelay);
			}
			CanShutdown = true;
		}

		private static bool _resetting;
		public static async Task Reset()
		{

			if(_resetting)
			{
				Log.Warn("Reset already in progress.");
				return;
			}
			_resetting = true;
			Game.Reset();
			await Task.Delay(1000);

			Overlay.HideSecrets();
			Overlay.Update(false);
			_resetting = false;
		}


		//public static class Windows
		//{
		//	private static PlayerWindow _playerWindow;
		//	private static OpponentWindow _opponentWindow;
		//	private static TimerWindow _timerWindow;
		//	private static StatsWindow _statsWindow;

		//	public static PlayerWindow PlayerWindow => _playerWindow ?? (_playerWindow = new PlayerWindow(Game));
		//	public static OpponentWindow OpponentWindow => _opponentWindow ?? (_opponentWindow = new OpponentWindow(Game));
		//	public static TimerWindow TimerWindow => _timerWindow ?? (_timerWindow = new TimerWindow(Config.Instance));
		//	public static StatsWindow StatsWindow => _statsWindow ?? (_statsWindow = new StatsWindow());
		//	public static CapturableOverlayWindow CapturableOverlay;
		//}
	}
}