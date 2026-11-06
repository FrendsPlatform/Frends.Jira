# Changelog

## [2.0.0] - 2025-10-21
### Changed
- Task now targets .NET 8.
- [Breaking Change] Task parameters are now in the order Input, Connection, Options.
- [Breaking Change] Options.ThrowOnError was replaced by Options.ThrowErrorOnFailure (default true), and Options.ErrorMessageOnFailure was added for a custom error message.
- [Breaking Change] Jira Cloud API v3 requires the comment in Atlassian Document Format (ADF). Plain text is not converted. Use API v2 for plain text.
- Error messages now include the deployment type and API version, e.g. `Error adding comment (Cloud, API v3): ...`.

### Added
- Jira Cloud support: Connection.DeploymentType (Cloud/DataCenter, default Cloud), Connection.CloudApiVersion (V3/V2, Cloud only) and Connection.Email (Cloud only, Basic authentication with API token).
- Input validation for deployment type, base URL, token, email, issue ID/key and comment.
- Tests restructured: Jira Cloud integration tests (configured with `.env`) and Data Center tests using a WireMock server.

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
