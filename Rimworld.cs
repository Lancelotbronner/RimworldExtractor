namespace RimworldExtractor;

public static class Rimworld {

	#region Installation Management

	private static DirectoryInfo? _installation = FindInstallationDirectory();

	public static DirectoryInfo Installation {
		get => _installation!;
		set => _installation = value;
	}

	public static bool IsInstalled => _installation is not null && _installation.Exists;

	private static DirectoryInfo? FindInstallationDirectory() {
		string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		string[] paths = new string[] {
			@"C:\Program Files (x86)\Steam\steamapps\common\RimWorld",
			$"{user}/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app",
			$"{user}/.steam/steam/steamapps/common/RimWorld",
			$"{user}/GOG Games/RimWorld/game",
		};

		if (paths.FirstOrDefault(Directory.Exists) is not string path)
			return null;

		return new DirectoryInfo(path);
	}

	public static void RefreshInstallation()
		=> _installation = FindInstallationDirectory();

	#endregion

	#region Metadata Management

	private static string? _version = FindVersion();

	public static string? Version => _version;

	private static string? FindVersion() {
		if (_installation is null)
			return null;

		string path = Path.Combine(_installation.FullName, "Version.txt");
		if (!File.Exists(path))
			return null;

		return File.ReadAllText(path);
	}

	public static void RefreshVersion()
		=> _version = FindVersion();

	#endregion

	#region Module Management

	public static string GetModule(string module)
		=> Path.Combine(Installation.FullName, "Data", module);

	public static IEnumerable<string> GetAvailableModules()
		=> Directory.EnumerateDirectories(Path.Combine(Installation.FullName, "Data"));

	#endregion

}
