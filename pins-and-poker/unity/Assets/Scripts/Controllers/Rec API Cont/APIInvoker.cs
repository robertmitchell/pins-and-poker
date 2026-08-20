using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class APIInvoker : Singleton<APIInvoker>
{
    private class ApiRequest
    {
        public Func<IEnumerator> RequestMethod;
        public float Interval;
        public float NextRunTime;
        public bool IsRunning;
    }

    private List<ApiRequest> apiRequests = new List<ApiRequest>();
    private int maxConcurrentRequests = 2; // Limit active API calls to prevent server overload

    private void Start()
    {
        StartCoroutine(ProcessRequests());
    }

    /// <summary>
    /// Adds an API request to the recursive queue
    /// </summary>
    public void AddApiRequest(Func<IEnumerator> apiMethod, float interval)
    {
        apiRequests.Add(new ApiRequest
        {
            RequestMethod = apiMethod,
            Interval = interval,
            NextRunTime = Time.time + UnityEngine.Random.Range(0, interval / 2), // Stagger calls
            IsRunning = false
        });
    }

    /// <summary>
    /// Removes a request if needed
    /// </summary>
    public void RemoveApiRequest(Func<IEnumerator> apiMethod)
    {
        apiRequests.RemoveAll(req => req.RequestMethod == apiMethod);
    }

    /// <summary>
    /// Process queued API calls smartly
    /// </summary>
    private IEnumerator ProcessRequests()
    {
        while (true)
        {
            float currentTime = Time.time;

            // Get requests that need execution and aren't already running
            var pendingRequests = apiRequests
                .Where(req => !req.IsRunning && currentTime >= req.NextRunTime)
                .Take(maxConcurrentRequests) // Limit concurrent requests
                .ToList();

            foreach (var request in pendingRequests)
            {
                request.IsRunning = true;
                StartCoroutine(ExecuteRequest(request)); // Run in parallel if needed
                request.NextRunTime = Time.time + request.Interval;
            }

            yield return new WaitForSeconds(1f); // Reduce CPU usage
        }
    }

    /// <summary>
    /// Executes an API request safely
    /// </summary>
    private IEnumerator ExecuteRequest(ApiRequest request)
    {
        yield return request.RequestMethod.Invoke();
        request.IsRunning = false; // Mark as finished
        Debug.Log("Requests Count");
    }
}
