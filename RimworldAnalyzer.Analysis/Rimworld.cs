namespace RimworldAnalyzer.Installation;

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

	#region Workshop Management

	private static DirectoryInfo? _workshop = FindWorkshopDirectory();

	public static DirectoryInfo Workshop {
		get => _workshop!;
		set => _workshop = value;
	}

	public static bool IsWorkshopInstalled => _workshop is not null && _workshop.Exists;

	private static DirectoryInfo? FindWorkshopDirectory() {
		string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		string[] paths = new string[] {
			@"C:\Program Files (x86)\Steam\steamapps\workshop\content\294100",
			$"{user}/Library/Application Support/Steam/steamapps/workshop/content/294100",
			$"{user}/.steam/steam/steamapps/workshop/content/294100",
		};

		if (paths.FirstOrDefault(Directory.Exists) is not string path)
			return null;

		return new DirectoryInfo(path);
	}

	public static void RefreshWorkshop()
		=> _workshop = FindWorkshopDirectory();

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

	#region Official Module Management

	public static string GetOfficialModule(string module)
		=> Path.Combine(Installation.FullName, "Data", module);

	public static IEnumerable<string> GetAvailableOfficialModules()
		=> Directory.EnumerateDirectories(Path.Combine(Installation.FullName, "Data"));

	#endregion

	#region Installed Module Management

	public static string GetInstalledModule(string module)
		=> Path.Combine(Installation.FullName, "Mods", module);

	public static IEnumerable<string> GetAvailableInstalledModules()
		=> Directory.EnumerateDirectories(Path.Join(Installation.FullName, "Mods"));

	#endregion

	#region Workshop Module Management

	public static string GetWorkshopModule(string module)
		=> Path.Combine(Workshop.FullName, module);

	public static IEnumerable<string> GetAvailableWorkshopModules()
		=> Directory.EnumerateDirectories(Workshop.FullName);

	#endregion

}
