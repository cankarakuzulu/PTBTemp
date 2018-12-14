using nopact.Commons.Domain.Enum;
using Pathfinding.Serialization.JsonFx;

namespace nopact.Commons.Domain.Model
{
	public class Game {

		private string gameId;
		private GamePlatform platform;
		private string version;
	
		public Game (string gameId, GamePlatform platform, string version) {
		
			this.gameId 	= gameId;
			this.platform 	= platform;
			this.version 	= version;
		
		}
	
		[JsonName("gameId")]
		public string GameId {
			get {
				return this.gameId;
			}
			set{
				gameId = value;
			}
		}
	
		[JsonName("platform")]
		public GamePlatform Platform {
			get {
				return this.platform;
			}
			set{
				platform = value;
			}
		}
	
		[JsonName("version")]
		public string Version {
			get {
				return this.version;
			}
			set{
				version = value;
			}
		}

	}
}
