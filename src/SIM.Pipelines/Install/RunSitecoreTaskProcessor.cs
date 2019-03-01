﻿using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install
{
  public class RunSitecoreTaskProcessor : Processor
  {
    string taskName;
    public RunSitecoreTaskProcessor(string TaskName)
    {
      Assert.ArgumentNotNullOrEmpty(TaskName, nameof(TaskName));
      this.taskName = TaskName;
    }

    protected override void Process([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      SitecoreTask task=arguments.Tasker.Tasks.FirstOrDefault(t => t.Name == this.taskName);
      Assert.ArgumentNotNull(task, nameof(task));
      string result= task.Run();
      if (task.State == TaskState.Failed)
      {
        throw new AggregateException(string.Format("Failed to execute {0} task. \n{1}",task.Name,result));
      }      
    }
  }
}
