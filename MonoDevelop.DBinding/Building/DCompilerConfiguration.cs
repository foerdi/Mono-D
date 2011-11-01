
using MonoDevelop.Core.Serialization;
namespace MonoDevelop.D.Building
{
	/// <summary>
	/// Stores compiler commands and arguments for compiling and linking D source files.
	/// </summary>
	[DataItem("CompilerConfiguration")]
	public class DCompilerConfiguration
	{
		public DCompilerConfiguration() { }

		/// <summary>
		/// Initializes all commands and arguments (also debug&amp;release args!) with default values depending on given target compiler type
		/// </summary>
		public DCompilerConfiguration(DCompilerVendor type)
		{
			ICompilerDefaultArgumentProvider cmp = null;
			switch (type)
			{
				case DCompilerVendor.DMD:
					cmp = new Dmd();
					break;
				case DCompilerVendor.GDC:
					cmp = new Gdc();
					break;
				case DCompilerVendor.LDC:
					cmp = new Ldc();
					break;
			}

			cmp.ResetCompilerConfiguration(this);

			cmp.ResetBuildArguments(DebugBuildArguments, true);
			cmp.ResetBuildArguments(ReleaseBuildArguments, false);
		}

		[ItemProperty]
		public DCompilerVendor CompilerType;

		[ItemProperty]
		public string CompilerExecutable;
		[ItemProperty]
		public string LinkerExecutable;

		[ItemProperty("DebugArguments")]
		public DArgumentConfiguration DebugBuildArguments = new DArgumentConfiguration { IsDebug=true};
		[ItemProperty("ReleaseArguments")]
		public DArgumentConfiguration ReleaseBuildArguments=new DArgumentConfiguration();

		public DArgumentConfiguration GetArgumentCollection(bool DebugArguments = false)
		{
			return DebugArguments ? DebugBuildArguments : ReleaseBuildArguments;
		}
	}

	/// <summary>
	/// Provides raw argument strings that will be used when building projects.
	/// Each DCompilerConfiguration holds both Debug and Release argument sub-configurations.
	/// </summary>
	[DataItem("ArgumentConfiguration")]
	public class DArgumentConfiguration
	{
		[ItemProperty]
		public bool IsDebug;

		[ItemProperty]
		public string SourceCompilerArguments;

		[ItemProperty]
		public string ExecutableLinkerArguments;
		[ItemProperty]
		public string ConsolelessLinkerArguments;

		[ItemProperty]
		public string SharedLibraryLinkerArguments;
		[ItemProperty]
		public string StaticLibraryLinkerArguments;

		public string GetLinkerArgumentString(DCompileTarget targetType)
		{
			switch (targetType)
			{
				case DCompileTarget.SharedLibrary:
					return SharedLibraryLinkerArguments;
				case DCompileTarget.StaticLibrary:
					return StaticLibraryLinkerArguments;
				case DCompileTarget.ConsolelessExecutable:
					return ConsolelessLinkerArguments;
			}

			return ExecutableLinkerArguments;
		}
	}
}

