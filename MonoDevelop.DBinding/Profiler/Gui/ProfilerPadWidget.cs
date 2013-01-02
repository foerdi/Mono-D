using System;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Components;
using System.IO;
using MonoDevelop.Ide;
using Gtk;
using D_Parser.Parser;
using D_Parser.Dom;
using MonoDevelop.D.Profiler.Commands;

namespace MonoDevelop.D.Profiler.Gui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ProfilerPadWidget : Gtk.Bin
	{
		ListStore traceFunctionsStore;
		DProfilerPad profilerPad;
	
		public ProfilerPadWidget (DProfilerPad pad)
		{
			profilerPad = pad;
			this.Build ();
			
			traceFunctionsStore = new ListStore (typeof(long), typeof(long), typeof(long), typeof(long), typeof(string));
			
			TreeModelSort cardSort = new TreeModelSort (traceFunctionsStore);
			
			nodeView.Model = cardSort;
			
			AddColumn("Num Calls", 0);
			AddColumn("Tree Time", 1);
			AddColumn("Func Time", 2);
			AddColumn("Per Call", 3);
			AddColumn("Func Symbol", 4);
			
			nodeView.ShowAll();
			RefreshSwitchProfilingIcon();
		}

		protected void OnRefreshActionActivated (object sender, EventArgs e)
		{
			IdeApp.CommandService.DispatchCommand( "MonoDevelop.D.Profiler.Commands.ProfilerCommands.AnalyseTaceLog");
		}
		
		public void ClearTracedFunctions()
		{
			traceFunctionsStore.Clear();
		}
		
		public void AddTracedFunction(long numCalls, long treeTime, long funcTime, long perCall, string symbol)
		{
			traceFunctionsStore.AppendValues(numCalls, treeTime, funcTime, perCall, symbol);
		}

		protected void OnNodeViewRowActivated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			TreeModel model;
			nodeView.Selection.GetSelected(out model, out iter);
			string function = model.GetValue(iter,4) as String;
			profilerPad.TraceParser.GoToFunction(function);
		}
		
		private void AddColumn(string title, int index)
		{
			TreeViewColumn column = nodeView.AppendColumn (title, new Gtk.CellRendererText (), "text", index);
			column.Resizable = true;
			column.SortColumnId = index;
			column.SortIndicator = true;
		}
		
		public void RefreshSwitchProfilingIcon()
		{
			if(ProfilerModeHandler.IsProfilerMode)
				switchProfilingModeAction.StockId = "gtk-yes";
			else
				switchProfilingModeAction.StockId = "gtk-no";
		}
		
		protected void OnSwitchProfilingModeActionActivated (object sender, EventArgs e)
		{
			ProfilerModeHandler.IsProfilerMode = !ProfilerModeHandler.IsProfilerMode;
		}

	}
}

