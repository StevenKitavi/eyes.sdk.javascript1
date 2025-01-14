﻿using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools
{
    /// <summary>
    /// Encapsulates data required to start session using the Session API.
    /// </summary>
    public class SessionStartInfo
    {
        public SessionStartInfo(
            string agentId,
            string appName,
            string verId,
            string testName,
            BatchInfo batchInfo,
            string baselineEnvName,
            object environment,
            string envName,
            ImageMatchSettings defaultMatchSettings,
            string branchName,
            string parentBranchName,
            string baselineBranchName,
            bool? saveDiffs,
            bool? render,
            string agentSessionId,
            int? timeout,
            PropertiesCollection properties,
            string agentRunId)
        {
            ArgumentGuard.NotEmpty(agentId, nameof(agentId));
            ArgumentGuard.NotEmpty(appName, nameof(appName));
            ArgumentGuard.NotEmpty(testName, nameof(testName));
            ArgumentGuard.NotNull(batchInfo, nameof(batchInfo));
            ArgumentGuard.NotNull(environment, nameof(environment));
            ArgumentGuard.NotNull(defaultMatchSettings, nameof(defaultMatchSettings));

            AgentId = agentId;
            AppIdOrName = appName;
            VerId = verId;
            ScenarioIdOrName = testName;
            BatchInfo = batchInfo;
            BaselineEnvName = baselineEnvName;
            Environment = environment;
            EnvironmentName = envName;
            DefaultMatchSettings = defaultMatchSettings;
            BranchName = branchName;
            ParentBranchName = parentBranchName;
            BaselineBranchName = baselineBranchName;
            SaveDiffs = saveDiffs;
            Render = render;
            AgentSessionId = agentSessionId;
            Timeout = timeout;
            Properties = properties;
            AgentRunId = agentRunId;
        }

        public int ConcurrencyVersion => 2;

        public string AgentId { get; private set; }

        public string AppIdOrName { get; private set; }

        public string VerId { get; private set; }

        public string ScenarioIdOrName { get; private set; }

        public BatchInfo BatchInfo { get; private set; }

        public string BaselineEnvName { get; private set; }

        public string EnvironmentName { get; private set; }

        public object Environment { get; private set; }

        public ImageMatchSettings DefaultMatchSettings { get; private set; }

        public string BranchName { get; private set; }

        public string ParentBranchName { get; private set; }

        public string BaselineBranchName { get; private set; }

        public bool? SaveDiffs { get; private set; }

        public bool? Render { get; set; }
        
        public string AgentSessionId { get; }

        public int? Timeout { get; private set; }
        
        public PropertiesCollection Properties { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AgentRunId { get; }
    }
}
