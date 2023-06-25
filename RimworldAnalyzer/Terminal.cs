namespace RimworldExtractor;

public static class Terminal {

	private static int _indent = 0;
	private static string Indent => new string('\t', _indent);

	public static void Start(string label, int total) {
		Milestone(label);
		_indent++;
	}

	public static void Progress(string label, int steps = 1) {
		Console.WriteLine(Indent + label);
	}

	public static void Complete(string label) {
		_indent--;
		Console.WriteLine(Indent + label);
	}

	public static void Milestone(string message) {
		Console.WriteLine($"{Indent}==> {message}");
	}

	public static void Information(string message) {
		Console.WriteLine(Indent + message);
	}
	
}
