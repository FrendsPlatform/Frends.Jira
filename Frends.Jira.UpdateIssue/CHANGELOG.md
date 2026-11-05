# Changelog

## [2.0.0] - 2026-10-21
### Changed
- Task now targets .NET 8.
- [Breaking] Task parameters are now in the order Input, Connection, Options, CancellationToken.
- [Breaking] Option `ThrowOnError` was replaced with `ThrowErrorOnFailure` (default true). Added option `ErrorMessageOnFailure` to override the error message.
- [Breaking] Result property `ErrorMessage` was replaced with `Error`, which contains `Message` and `AdditionalInfo`.
- [Breaking] Jira Cloud is now the default deployment and uses REST API v3. For Jira Data Center, set `DeploymentType` to `DataCenter`.
- [Breaking] Jira Cloud now requires the `Email` setting together with an API token.
- Error messages now include the Jira deployment type and API version, for example `Error updating issue (DataCenter, API v2): BadRequest`.
- When `ThrowErrorOnFailure` is true, the original exception is now thrown instead of being wrapped in an "An error occurred" exception.
### Added
- Support for Jira Cloud and Jira Data Center. Choose the deployment with `DeploymentType` in Connection.
- `CloudApiVersion` selection for Jira Cloud (v3 or v2). Data Center always uses v2.
- Clear validation errors for missing connection settings, issue ID/key, or update content.
- A non-JSON Jira error response no longer hides the HTTP status or response body.

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
