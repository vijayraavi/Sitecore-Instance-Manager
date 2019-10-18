﻿using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install
{
  public class RunPSTasksProcessor : ProcessorHive
  {
    public override IEnumerable<Processor> CreateProcessors(ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      List<Processor> processors = new List<Processor>();
      arguments.Tasker.EvaluateGlobalParams();
      Processor root = null;
      foreach (PowerShellTask task in arguments.Tasker.Tasks)
      {
        if (!task.ShouldRun)
        {
          continue;
        }

        Processor proc=null;
        if (arguments.ScriptsOnly)
        {
          proc = new GenerateScriptProcessor(task.Name);
        }
        else
        {
          proc = new RunPSTaskProcessor(task.Name);          
        }        

        proc.ProcessorDefinition.Title = task.Name;
        proc.ProcessorDefinition.Type = proc.GetType();
        if (root == null)
        {          
          processors.Add(proc);
        }
        else
        {
          root._NestedProcessors.Add(proc);
        }
        root = proc;
      }

      return processors;
    }
  }
}
