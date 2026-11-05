# Changelog

## [2.0.0] - 2025-10-21
### Changed
- Breaking: Task now targets .NET 8.
- Breaking: Task parameters are now in the order Input, Connection, Options.
- Breaking: Options.ThrowOnError was replaced by Options.ThrowErrorOnFailure (default true), and Options.ErrorMessageOnFailure was added for a custom error message.
- Breaking: Result.ErrorMessage was replaced by Result.Error, which contains Message and AdditionalInfo. Error is null on success.

## [1.1.0] - 2025-03-19
### Changed
- Update packages:
  Newtonsoft.Json         12.0.1  -> 13.0.3
  RestSharp               110.2.0 -> 112.1.0
  coverlet.collector      3.1.2   -> 6.0.4
  Microsoft.NET.Test.Sdk  17.1.0  -> 17.13.0
  MSTest.TestAdapter      2.2.8   -> 3.8.3
  MSTest.TestFramework    2.2.8   -> 3.8.3

## [1.0.0] - 2023-08-18
### Added
- Initial implementation