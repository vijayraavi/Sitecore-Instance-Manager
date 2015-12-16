﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines.Export;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Wizards;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ExportInstanceButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("Export");

      if (instance != null)
      {
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("export", mainWindow, new ExportArgs(instance, false, true, true, true, false, false, false, false, false), null, () => MainWindowHelper.MakeInstanceSelected(id), instance, string.Empty);
      }
    }

    #endregion
  }
}