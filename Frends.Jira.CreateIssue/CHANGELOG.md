# Changelog

## [2.0.0] - 2026-10-21
### Changed
- Upgraded the Task to target .NET 8.
- [Breaking Change]: Task parameters are now ordered Input, Connection, Options, CancellationToken.
- [Breaking Change]: Option `ThrowOnError` was replaced by `ThrowErrorOnFailure` (default true).
- [Breaking Change]: Result property `ErrorMessage` was replaced by `Error` (with `Message` and `AdditionalInfo`).
- [Breaking Change]: The Task now connects to Jira Cloud by default (REST API v3). If you use Jira Data Center, set `DeploymentType` to `DataCenter`.
- [Breaking Change]: Jira Cloud requires a new `Email` setting. Use it together with an API token.
- [Breaking Change]: Error messages now show the Jira type and API version, for example `Error creating issue (DataCenter, API v2): BadRequest`.
- [Breaking Change]: Jira Cloud API v3 requires the description in Atlassian Document Format (ADF). Plain text is not converted. Use API v2 for plain text.
### Added
- Support for both Jira Cloud and Jira Data Center. Choose with `DeploymentType` in the Connection.
- Setting `CloudApiVersion` (v3 or v2). It is shown only for Jira Cloud. Data Center always uses v2.
- `Parameters` can now contain JSON for fields such as labels, select or user.
- Option `ErrorMessageOnFailure` for overriding the error message on failure.

## [1.1.0] - 2025-03-19
### Changed
- Updated packages:
  Newtonsoft.Json         12.0.1  -> 13.0.3
  RestSharp               110.2.0 -> 112.1.0
  coverlet.collector      3.1.2   -> 6.0.4
  Microsoft.NET.Test.Sdk  17.1.0  -> 17.13.0
  MSTest.TestAdapter      2.2.8   -> 3.8.3
  MSTest.TestFramework    2.2.8   -> 3.8.3

## [1.0.0] - 2023-08-17
### Added
- Initial implementation
