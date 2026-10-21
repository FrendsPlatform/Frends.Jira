# Changelog

## [2.0.0] - 2026-10-21
### Changed
- The Task now targets .NET 8.
- Task parameters are now ordered Input, Connection, Options. Existing process steps must be reconfigured.
- Replaced the `ThrowOnError` option with `ThrowErrorOnFailure` (default true) and added the `ErrorMessageOnFailure` option for a custom error message.
- The result now contains an `Error` object (`Message`, `AdditionalInfo`) instead of the `ErrorMessage` string. `Error` is null when the Task succeeds.
- Invalid or missing input, connection and option values are now reported as validation errors.

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