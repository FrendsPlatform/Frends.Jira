﻿using Frends.Jira.UpdateIssue.Definitions;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Jira.UpdateIssue;

/// <summary>
/// Jira Task.
/// </summary>
public class Jira
{
    /// <summary>
    /// Delete Jira issue.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Jira.UpdateIssue)
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="input">Input parameters</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, string ErrorMessage }</returns>
    public static async Task<Result> UpdateIssue([PropertyTab] Connection connection, [PropertyTab] Input input, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new RestClient(connection.JiraBaseUrl);
            client.AddDefaultHeader("Authorization", $"Bearer {connection.Token}");

            var request = new RestRequest($"rest/api/latest/issue/{input.IdOrKey}", Method.Get);
            var response = await client.ExecuteAsync(request, cancellationToken);

            if (response.IsSuccessful)
            {
                var updateRequest = new RestRequest($"/rest/api/2/issue/{input.IdOrKey}", Method.Put);
                updateRequest.AddParameter("application/json", JsonConvert.SerializeObject(input.Content), ParameterType.RequestBody);

                var updateResponse = await client.ExecuteAsync(updateRequest, cancellationToken);

                if (updateResponse.IsSuccessful)
                    return new Result(true, null);
                else
                    return new Result(false, $"Error updating issue, Status code: {updateResponse.StatusCode}, Content: {updateResponse.Content}.");
            }
            else
            {
                return new Result(false, $"Error retrieving issue, Status code: {response.StatusCode}, Content: {response.Content}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred: {ex.Message}");
        }
    }
}