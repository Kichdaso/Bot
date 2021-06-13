﻿using Interfaces;
using Octokit;
using Services.GitHubAPI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GitHubBot
{
    internal class Programmer
    {
        private readonly GitHubStorage gitHubAPI;

        private static readonly TimeSpan MinimumInteractionInterval = new(0, 0, 0, 0, 1200);

        private readonly List<ITrigger<Issue>> triggers;

        public Programmer(List<ITrigger<Issue>> triggers,ICodeStorage<Issue> gitHubAPI)
        {
            this.gitHubAPI = (GitHubStorage)gitHubAPI;
            this.triggers = triggers;
        }

        private void ProcessIssues(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var issues = gitHubAPI.GetIssues();
                foreach (var trigger in triggers)
                {
                    foreach (var issue in issues)
                    {
                        if (trigger.Condition(issue))
                        {
                            trigger.Action(issue);
                        }
                    }
                }
                Thread.Sleep(MinimumInteractionInterval);
            }
        }

        public void Start(CancellationToken cancellationToken)
        {
            ProcessIssues(cancellationToken);
        }
    }
}
