# Sitecore Instance Manager (SIM 2.0)

## Description

SIM2 is an app that sets up scaled Sitecore solutions: 
several Sitecore instances deployed to different servers acting with different roles.

## High-Level Overview

**SIM 1.x** was designed for development environment and rapid Sitecore installation 
of any version. The main purpose was to improve PSS support ticket workflow to perform 
investigation on clean Sitecore instance of given version. So entire process was built 
around this subject. 

Main features:

* bypass security - to save time on typing passwords
* app pool state control - to simulate production conditions
* installation of modules/packages/tweaks
* backup/restore - to recover previous instance state after unsuccessful experiment
* reinstall - to prepare an instance for next ticket

**SIM 2.0** is designed for different purpose, it is **a portable, solution-wide 
multi-environment Sitecore-specific CI system** that can be used for 
**simple non-production deployments** with support of 
* remote web/IIS servers, 
* remote SQL/Mongo/Solr servers

It should be possible to spin-up production-like one or several test environments 
with many Sitecore instances in each having either only a dev laptop or a bunch of 
geographically-distributed servers. 


## Conceptual Roadmap

**SIM 2.1** should expose [Sitecore Information Service](http://dl.sitecore.net/updater/info) to download necessary Sitecore versions on-the-fly.

**SIM 2.2** should deliver backup/restore and export/import functionality on the solution-scale level. 

## Format

It is a command-line interface that has only two major commands in v2.0:
* deploy - take the manifest and the environment file, and make a deployment
* remove - remove everything deployed by previous command

A solution manifest must not contain any connection strings, file paths etc. It must 
describe a solution configuration, number of servers, their roles and connections, but 
not the actual paths and connection strings. 

A solution environment file contains necessary variables for the manifest file e.g. 
paths, keys and connection strings to database servers and search providers. 

## Technical Aspects

0. Command-line interface with no progress indication (workaround: realtime log viewer is monitoring log file)
1. Not a Client-Server architecture. 
2. 8.1.3+ with Configuration Roles to avoid configuration hassle.
3. .NET 4.5 with further optional migration to .NET Core
4. TDD
5. JSON format of input and output data

## Coding Guidelines

* If a class is being instantitated by reflection at least in some cases - it must have a constructur marked as [UsedImplicitly]