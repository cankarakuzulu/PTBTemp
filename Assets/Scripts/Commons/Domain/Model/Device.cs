using System.Text;
using nopact.Commons.Utility;
using Pathfinding.Serialization.JsonFx;
using UnityEngine;

namespace nopact.Commons.Domain.Model
{
	public class Device {

		private string deviceId;
		private DeviceType type;
		private string model;
		private string operatingSystem;
		private int memorySize;
		private int graphicsMemorySize;
		private string name;
		private string infoHash;
	
		public Device (string deviceId, DeviceType deviceType, string model, string operatingSystem,
			int memorySize, int graphicsMemorySize, string name){
		
			this.deviceId 			= deviceId;
			this.type 				= deviceType;
			this.model 				= model;
			this.operatingSystem 	= operatingSystem;
			this.memorySize 		= memorySize;
			this.graphicsMemorySize = graphicsMemorySize;
			this.name 				= name;
		
		}
	
		[JsonName("deviceId")]
		public string DeviceId {
			get {
				return this.deviceId;
			}
			set{
				deviceId = value;
			}
		}
	
		[JsonName("type")]
		public DeviceType Type {
			get {
				return this.type;
			}
			set{
				type = value;
			}
		}
	
		[JsonName("model")]
		public string Model {
			get {
				return this.model;
			}
			set{
				model = value;
			}
		}
	
		[JsonName("operatingSystem")]
		public string OperatingSystem {
			get {
				return this.operatingSystem;
			}
			set{
				operatingSystem = value;
			}
		}
	
		[JsonName("memorySize")]
		public int MemorySize {
			get {
				return this.memorySize;
			}
			set{
				memorySize = value;
			}
		}
	
		[JsonName("graphicsMemorySize")]
		public int GraphicsMemorySize {
			get {
				return this.graphicsMemorySize;
			}
			set{
				graphicsMemorySize = value;
			}
		}
	
		[JsonName("name")]
		public string Name {
			get {
				return this.name;
			}
			set{
				name = value;
			}
		}
	
		[JsonName("infoHash")]
		public string InfoHash {
			get {
				return this.infoHash;
			}
			set {
				infoHash = value;
			}
		}
	
		public string HashData {
			get {
			
				StringBuilder sb = new StringBuilder();
				sb.Append(deviceId).Append(type.ToString()).Append(FormatUtility.ReturnEmptyStringIfNull(model))
					.Append(FormatUtility.ReturnEmptyStringIfNull(operatingSystem)).Append(memorySize).Append(graphicsMemorySize)
					.Append(FormatUtility.ReturnEmptyStringIfNull(name));
			
				return sb.ToString();
			
			}
		}
	
	}
}
