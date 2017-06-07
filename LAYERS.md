# Layers

In this document arrow (->) shows reference flow.

## App

The App layer is a glue that grips all components together and injects project metadata to them.

Projects:

* **SIM** -> `SIM.exe`

## Framework

`App -> Framework`

The Framework layer is an abstract JSON-based command line interface engine that is completely generic and can be used for building other apps.

Projects:

* **SIM.App** -> `SIM.App.dll`
* *SIM.App.UnitTests*

## Commands

`App -> Commands`

The Commands layer holds the main appliaction abstract logic which represents main functions that SIM app does for user. 

## Foundation

`App -> Commands -> Foundation`

The Foundation layer represents independent components

Projects:

* **SIM.Foundation.SqlAdapter** -> `SIM.Foundation.SqlAdapter.dll`
* *SIM.Foundation.SqlAdapter.UnitTests*
* **SIM.Foundation.WebAdapter** -> `SIM.Foundation.WebAdapter.dll`
* *SIM.Foundation.WebAdapter*

## Base

`App -> Commands -> Foundation -> Base`

The base level with shared components.

Projects:

* **SIM.Base** -> SIM.Base.dll