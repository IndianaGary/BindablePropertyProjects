# BindablePropertyProjects

This repo contains the components of a BindableProperty incremetal source code generator, an analyzer, 
and a code fix. In addition there are unit tests for each and an integration test project.

## Components

### BindablePropertyAnalyzerAndCodeFix
Roslyn Analyzer and Codefix project.

### BindablePropertyAnalyzerAndCodeFix.UnitTests
Unit Tests for the Analyzer and CodeFix. These unit tests use MSTest based upon the standard template.

### BindablePropertyAttributes
Contains the definition of the BindablePropertyAttribute.

### BindablePropertyGenerator
The BindableProperty Roslyn Incremental Source Code Generator.

### BindablePropertyGenerator.UnitTests
Unit Tests for the source code generator. These unit tests use Xunit and Verify to provide Snapshot
tests. The "verified" tests are in the Snapshots folder and are checked into source control.

### BindablePropertyMauiTest
This is a Maui application based upon the standard template. In the Controls folder is a test control
named BottomDrawer that has been implemented using the BindableProperty incremental generator. It is not 
intended to run, but it is intended to compile successfully.