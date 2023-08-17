﻿using Frends.Jira.DeleteIssue.Definitions;
using RestSharp;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Jira.DeleteIssue;

/// <summary>
/// Jira Task.
/// </summary>
public class Jira
{
    /// <summary>
    /// Delete Jira issue.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Jira.DeleteIssue)
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="input">Input parameters</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, string EntityId, string ErrorMessage }</returns>
    public static async Task<Result> DeleteIssue([PropertyTab] Connection connection, [PropertyTab] Input input, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new RestClient(connection.JiraBaseUrl);
            
            var request = new RestRequest($"rest/api/latest/issue/{input.IdOrKey}", Method.Get);
            request.AddHeader("Authorization", $"Bearer {connection.Token}");

            var response = await client.ExecuteAsync(request, cancellationToken);

            if (response.IsSuccessful)
            {
                var deleteRequest = new RestRequest($"rest/api/latest/issue/{input.IdOrKey}", Method.Delete);
                var deleteResponse = await client.ExecuteAsync(deleteRequest, cancellationToken);

                if (deleteResponse.IsSuccessful)
                    return new Result(true, "Issue deleted successfully.", null);
                else
                    return new Result(false, null, $"Error deleting issue: {deleteResponse.StatusCode}");
            }
            else
            {
                return new Result(false, response.Content, $"Error retrieving issue: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred: {ex.Message}");
        }
    }
}