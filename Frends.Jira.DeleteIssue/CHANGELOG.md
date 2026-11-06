# Changelog

## [2.0.0] - 2026-10-21
### Changed
- The Task now targets .NET 8.
- [Breaking Change] Task parameters are now ordered Input, Connection, Options, CancellationToken. Existing process steps must be reconfigured.
- [Breaking Change] Replaced the `ThrowOnError` option with `ThrowErrorOnFailure` (default true) and added the `ErrorMessageOnFailure` option for a custom error message.
- [Breaking Change] The result now contains an `Error` object (`Message`, `AdditionalInfo`) instead of the `ErrorMessage` string. `Error` is null when the Task succeeds.
- [Breaking Change] Jira Cloud is now the default deployment and uses REST API v3. For Jira Data Center, set `DeploymentType` to `DataCenter`.
- [Breaking Change] Jira Cloud now requires the `Email` setting together with an API token.
- Errors include the Jira deployment type and API version, for example `Error deleting issue (DataCenter, API v2): Forbidden`.
- Invalid or missing input and connection values are reported as validation errors.
### Added
- Support for deleting issues from Jira Cloud and Jira Data Center.
- `CloudApiVersion` selection for Jira Cloud (v3 or v2). Data Center always uses v2.
- Non-JSON error responses preserve the HTTP status and response body in the task error.

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
