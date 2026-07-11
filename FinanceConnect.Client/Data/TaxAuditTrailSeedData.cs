using static FinanceConnect.Client.ViewModels.TaxAuditTrailViewModel;

namespace FinanceConnect.Client.Data
{
    public static class TaxAuditTrailSeedData
    {
        private static readonly Guid Co = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private static readonly Guid Br = Guid.Parse("20000000-0000-0000-0000-000000000001");

        public static List<TaxAuditTrailModel> Get()
        {
            return new List<TaxAuditTrailModel>
            {
                Build("72000000-0000-0000-0000-000000000001","TAUDIT-2026-00001",new DateTime(2026,1,10,9,15,0),"TaxRateVersion","63000000-0000-0000-0000-000000000001","VER-IGST-18-V2","IGST 18% Rate Version 2","MasterData","Updated","High","GST","User","Rajesh Kumar","Tax Admin","COR-20260110-001","2026-01",true,"[\"RatePercent\",\"EffectiveFrom\"]","{\"RatePercent\":18.0,\"EffectiveFrom\":\"2025-07-01\"}","{\"RatePercent\":18.0,\"EffectiveFrom\":\"2026-01-01\"}","RateCorrection","Effective date correction per notification",false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000002","TAUDIT-2026-00002",new DateTime(2026,1,15,10,30,0),"TaxTransaction","30000000-0000-0000-0000-000000000001","TAX-2026-00001","GST Output - INV-2026-00101","Posting","Created","Info","GST","System","System Posting Engine",null,"COR-20260115-001","2026-01",false,null,null,"{\"TaxType\":\"GST\",\"TaxableValueTotal\":100000}",null,null,false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000003","TAUDIT-2026-00003",new DateTime(2026,1,15,10,30,5),"TaxTransaction","30000000-0000-0000-0000-000000000001","TAX-2026-00001","GST Output - INV-2026-00101","Posting","Posted","Info","GST","System","System Posting Engine",null,"COR-20260115-001","2026-01",false,"[\"TaxTransactionStatus\"]","{\"Status\":\"Draft\"}","{\"Status\":\"Posted\"}",null,null,false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000004","TAUDIT-2026-00004",new DateTime(2026,2,18,14,30,0),"TaxSettlement","70000000-0000-0000-0000-000000000001","TAXSET-2026-00001","GST Cash Payment - Jan 2026","Settlement","Posted","Info","GST","User","Priya Finance","Controller","COR-20260218-001","2026-01",true,"[\"SettlementStatus\"]","{\"Status\":\"Approved\"}","{\"Status\":\"Posted\",\"Amount\":118000}",null,null,false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000005","TAUDIT-2026-00005",new DateTime(2026,1,26,16,0,0),"TaxSettlement","70000000-0000-0000-0000-000000000006","TAXSET-2026-00006","TDS Remittance Reversal","Settlement","Reversed","High","TDS","User","Priya Finance","Controller","COR-20260126-001","2026-01",true,"[\"SettlementStatus\"]","{\"Status\":\"Posted\"}","{\"Status\":\"Reversed\"}","ManualOverride","Wrong bank account used",false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000006","TAUDIT-2026-00006",new DateTime(2026,2,12,10,0,0),"GSTReturnRun","71000000-0000-0000-0000-000000000001","GSTRUN-2026-00001","GST Return Run - Jan 2026","ReturnPreparation","Finalized","High","GST","User","Priya Finance","Controller","COR-20260212-001","2026-01",true,null,null,"{\"IncludedCount\":4150,\"Hash\":\"SHA256:a1b2c3\"}",null,null,false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000007","TAUDIT-2026-00007",new DateTime(2026,2,15,16,30,0),"GSTReturnRun","71000000-0000-0000-0000-000000000001","GSTRUN-2026-00001","GST Return Run - Jan 2026","Filing","Filed","High","GST","User","Rajesh Kumar","Tax Admin","COR-20260215-001","2026-01",true,null,null,"{\"FilingStatus\":\"Acknowledged\"}",null,null,false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000008","TAUDIT-2026-00008",new DateTime(2026,2,20,11,45,0),"TaxCode","62000000-0000-0000-0000-000000000001","GST_INTRA_18","GST Intra-State 18%","Security","AccessDenied","Critical","GST","User","Arun Intern","Junior Accountant","COR-20260220-001",null,false,null,null,null,null,"Attempted to modify locked TaxCode",true,"LockedRecordEditAttempt","Failed","Record is locked"),
                Build("72000000-0000-0000-0000-000000000009","TAUDIT-2026-00009",new DateTime(2026,1,5,14,0,0),"TaxCategoryMapping","64000000-0000-0000-0000-000000000001","MAP_GST_INTRA_B2B","GST Intra B2B Standard 18%","MasterData","Updated","High","GST","User","Rajesh Kumar","Tax Admin","COR-20260105-001",null,true,"[\"Priority\"]","{\"Priority\":2}","{\"Priority\":1}","RateCorrection","Priority adjusted for FY26",false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000010","TAUDIT-2026-00010",new DateTime(2026,1,20,14,0,0),"GSTReturnRun","71000000-0000-0000-0000-000000000005","GSTRUN-2025-00012","GST Return Run - Dec 2025","ReturnPreparation","Reopened","Critical","GST","User","Priya Finance","Controller","COR-20260120-001","2025-12",true,"[\"Status\",\"IsLocked\"]","{\"Status\":\"Finalized\"}","{\"Status\":\"Reopened\"}","ReopenFinalizedReturn","Large invoice reversed after finalization",false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000011","TAUDIT-2026-00011",new DateTime(2025,12,15,10,0,0),"TDSConfig","67000000-0000-0000-0000-000000000001","TDS-CFG-194C","TDS Config - Section 194C","MasterData","Created","Info","TDS","User","Rajesh Kumar","Tax Admin","COR-20251215-001",null,false,null,null,"{\"SectionCode\":\"194C\",\"BaseRate\":1.0}",null,null,false,null,"Passed",null),
                Build("72000000-0000-0000-0000-000000000012","TAUDIT-2026-00012",new DateTime(2026,3,5,11,0,0),"TaxTransaction","30000000-0000-0000-0000-000000000050","TAX-2026-00050","GST Output - Duplicate Ref","Calculation","ValidationFailed","Warning","GST","System","Return Validation Engine",null,"COR-20260305-001","2026-03",false,null,null,null,null,null,false,null,"Failed","Duplicate invoice reference detected"),
            };
        }

        private static TaxAuditTrailModel Build(string id, string eventNum, DateTime ts, string entityType, string entityId, string entityNum, string entityName, string category, string eventType, string severity, string taxScope, string actorType, string actorName, string? actorRole, string corrId, string? period, bool sensitive, string? changedFields, string? before, string? after, string? reasonCode, string? reasonText, bool secFlag, string? secType, string valStatus, string? valMsg)
        {
            return new()
            {
                Id = Guid.Parse(id), CompanyId = Co, CompanyName = "Acme Pvt Ltd",
                AuditEventNumber = eventNum, EventTimestamp = ts, EventDate = ts.Date,
                EntityType = entityType, EntityId = Guid.Parse(entityId),
                EntityNumberSnapshot = entityNum, EntityDisplayNameSnapshot = entityName,
                EventCategory = category, EventType = eventType, EventSeverity = severity,
                TaxTypeScope = taxScope, ActorType = actorType, ActorNameSnapshot = actorName,
                ActorRoleSnapshot = actorRole ?? "",
                ActorIpAddress = actorType == "User" ? "192.168.1.50" : null,
                CorrelationId = corrId, TaxPeriodKey = period ?? "", BranchId = Br, BranchName = "Chennai HQ",
                ChangedFieldListJson = changedFields, BeforeStateJson = before, AfterStateJson = after,
                IsSensitiveChange = sensitive, ReasonCode = reasonCode, ReasonText = reasonText ?? "",
                ValidationStatus = valStatus, ValidationMessage = valMsg ?? "",
                SecurityEventFlag = secFlag, SecurityEventType = secType ?? "",
                IsImmutable = true, RetentionCategory = "Statutory", ArchiveStatus = "Active",
                CreatedAt = ts, CreatedBy = "system",
            };
        }
    }
}
