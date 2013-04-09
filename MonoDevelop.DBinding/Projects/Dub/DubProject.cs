﻿using MonoDevelop.Core;
using MonoDevelop.Projects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonoDevelop.D.Projects.Dub
{
	public class DubProject : AbstractDProject
	{
		#region Properties
		public override string Name
		{
			get { return ParentSolution.Name; }
			set { ParentSolution.Name = value; }
		}

		protected override List<FilePath> OnGetItemFiles(bool includeReferencedFiles)
		{
			var files = new List<FilePath>();

			foreach(var dir in GetSourcePaths(null))
				foreach (var f in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
					files.Add(new FilePath(f));

			return files;
		}

		public override IEnumerable<string> GetSourcePaths(ConfigurationSelector sel)
		{
			yield return BaseDirectory.Combine("source").ToString();
		}
		#endregion

		#region Constructor & Init
		public DubProject(DubSolution solution)
		{
			BaseDirectory = solution.BaseDirectory;
			solution.RootFolder.AddItem(this, false);
		}

		public void UpdateFilelist()
		{
			Items.Clear();
			foreach (var f in GetItemFiles(true))
				Items.Add(new ProjectFile(f));
		}
		#endregion

		public void AddProjectAndSolutionConfiguration(DubProjectConfiguration cfg)
		{
			var slnCfg = ParentSolution.AddConfiguration(cfg.Name, false);
			slnCfg.AddItem(this, true, "");
			slnCfg.Id = cfg.Id;
			slnCfg.Platform = cfg.Platform;

			Configurations.Add(cfg);
		}
	}
}
