using nopact.Commons.Domain.Enum;
using nopact.Commons.Player;

namespace nopact.Commons.Domain.Model
{
	public class GameInfo{
	
		private static Device device;
		private static Game game;
	
		public static Device Device {
			get {
				return device;
			}
			set {
				device = value;
			}
		}

		public static Game Game {
			get {
				return game;
			}
			set{
				game = value;
			}
		}

		public static string Language {
		
			get{
			
				string language = null;
			
				switch (PlayerPreferences<SimplePlayerPreferencesData>.Instance.Data.Lang) {
				
					case GameLanguage.Turkish:
				
						language = "tr-tr";
						break;
				
					default:
					
						language = "en-us";
						break;
				}
			
				return language;
			
			}

		}
	
		public static GamePlatform Platform {
		
			get{
			
#if UNITY_WEBPLAYER
			return GamePlatform.WEB;
			#endif
			
#if UNITY_EDITOR
				return GamePlatform.EDITOR;
#endif

#if UNITY_STANDALONE
			return GamePlatform.STANDALONE;
			#endif
			
#if UNITY_IPHONE
				return GamePlatform.IOS;
#endif
		
#if UNITY_ANDROID
			return GamePlatform.ANDROID;
			#endif			
			
			}
		
		}
	
	}
}
