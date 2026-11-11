# Changelog

## [2.0.0] - 2026-10-21
### Changed
- Task now targets .NET 8.
- [Breaking Change]: Parameter order changed to Input, Connection, Options, CancellationToken. Existing Task usages must be reconfigured.
- [Breaking Change]: Options.ThrowOnError replaced by Options.ThrowErrorOnFailure (default true) and Options.ErrorMessageOnFailure for a custom error message.
- [Breaking Change]: Result.ErrorMessage replaced by Result.Error object with Message and AdditionalInfo. When an error is returned instead of thrown, Result.Data is no longer populated.
- [Breaking Change]: Jira Cloud is now the default deployment and uses REST API v3. For Jira Data Center, set `DeploymentType` to `DataCenter`.
- [Breaking Change]: Jira Cloud now requires the `Email` setting together with an API token.
- [Breaking Change]: Error messages now include the Jira deployment type and API version, for example `Error retrieving issue (DataCenter, API v2): NotFound`.
- [Breaking Change]: The `Jql` value is now URL-encoded by the Task. Do not encode it yourself.
- The Task now uses a fixed API version instead of `latest`.
- Empty issue ID/key, empty JQL, missing URL, token or email now give a clear error before the request is sent.
### Added
- Support for Jira Cloud and Jira Data Center. Choose the deployment with `DeploymentType` in Connection.
- `CloudApiVersion` selection for Jira Cloud (v3 or v2). Data Center always uses v2.
- [Breaking Change] JQL search on Jira Cloud uses the new search endpoint and returns all issue fields. Data Center uses the existing search endpoint.
- JQL search can fetch and combine pages using `GetAllPages`, with configurable `MaxResults` and a `MaxIssues` safety limit.
- JQL search supports manual paging with Cloud `NextPageToken` and Data Center `StartAt`.
### Fixed
- A non-JSON error response (for example from a proxy) no longer hides the real error.

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
