using System;
using System.Collections.Generic;

namespace Microsoft.OpenAIRateLimiter.API.Models
{
    public class BudgetAlert
    {
        public string schemaId { get; set; }
        public Data data { get; set; }
    }
    public class AlertContext
    {
        public string AlertCategory { get; set; }
        public AlertData AlertData { get; set; }
    }

    public class AlertData
    {
        public string Scope { get; set; }
        public string ThresholdType { get; set; }
        public string BudgetType { get; set; }
        public string BudgetThreshold { get; set; }
        public string NotificationThresholdAmount { get; set; }
        public string BudgetName { get; set; }
        public string BudgetId { get; set; }
        public string BudgetStartDate { get; set; }
        public string BudgetCreator { get; set; }
        public string Unit { get; set; }
        public string SpentAmount { get; set; }
    }

    public class Data
    {
        public Essentials essentials { get; set; }
        public AlertContext alertContext { get; set; }
    }

    public class Essentials
    {
        public string monitoringService { get; set; }
        public DateTime firedDateTime { get; set; }
        public string description { get; set; }
        public string essentialsVersion { get; set; }
        public string alertContextVersion { get; set; }
        public string alertId { get; set; }
        public object alertRule { get; set; }
        public object severity { get; set; }
        public object signalType { get; set; }
        public object monitorCondition { get; set; }
        public object alertTargetIDs { get; set; }
        public List<string> configurationItems { get; set; }
        public object originAlertId { get; set; }
    }

}