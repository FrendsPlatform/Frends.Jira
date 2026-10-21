# Changelog

## [2.0.0] - 2026-10-21
### Changed
- Task now targets .NET 8.
- Parameter order changed to Input, Connection, Options. Existing Task usages must be reconfigured.
- Options.ThrowOnError replaced by Options.ThrowErrorOnFailure (default true) and Options.ErrorMessageOnFailure for a custom error message.
- Result.ErrorMessage replaced by Result.Error object with Message and AdditionalInfo. When an error is returned instead of thrown, Result.Data is no longer populated.

## [1.1.0] - 2025-03-19
### Changed
- Update packages:
  Newtonsoft.Json         12.0.1  -> 13.0.3
  RestSharp               110.2.0 -> 112.1.0
  coverlet.collector      3.1.2   -> 6.0.4
  Microsoft.NET.Test.Sdk  17.1.0  -> 17.13.0
  MSTest.TestAdapter      2.2.8   -> 3.8.3
  MSTest.TestFramework    2.2.8   -> 3.8.3

## [1.0.0] - 2023-08-17
### Added
- Initial implementation