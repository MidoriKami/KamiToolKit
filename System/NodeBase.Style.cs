using System;
using System.IO;
using Dalamud.Utility;
using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace KamiToolKit.System;

public partial class NodeBase {
	public void Save(string filePath) {
		try {
			var fileText = JsonConvert.SerializeObject(this, Formatting.Indented);
			FilesystemUtil.WriteAllTextSafe(filePath, fileText);
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}

	public void Load(string filePath) {
		try {
			var fileData = File.ReadAllText(filePath);
			if (!fileData.IsNullOrEmpty()) {
				JsonConvert.PopulateObject(fileData, this);
			}
		}
		catch (FileNotFoundException) {
			Save(filePath);
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}
}