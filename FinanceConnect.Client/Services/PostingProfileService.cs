using FinanceConnect.Client.Data;
using FinanceConnect.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceConnect.Client.Services
{
    public class PostingProfileService
    {
        private readonly MasterDataService _masterDataService;
        private readonly PostingProfileSeed _profileSeed;
        private static List<PostingProfileModel> _profile = new();
        private readonly List<PostingProfileModel> _seedProfile = new();
        private readonly List<CompanyModel> _companies = new();

        public PostingProfileService(MasterDataService masterDataService,
        PostingProfileSeed profileSeed)
        {
            _masterDataService = masterDataService;
            _profileSeed = profileSeed;

            _companies = _masterDataService
                .GetAllCompanies()
                .Where(c => c.Status == "Active")
                .ToList();

            _seedProfile = Seed();
            ResetToSeed();
        }

        private static List<T> CloneList<T>(IEnumerable<T> source) where T : class
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new();
        }

        public void ResetToSeed()
        {
            _profile = CloneList(_seedProfile);
        }


        public List<CompanyModel> GetCompanies()
        {
            return _masterDataService
            .GetAllCompanies()
            .Where(c => c.Status == "Active")
            .ToList();
        }

        public List<PostingProfileModel> GetAll()
        {
            return _profile
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
        }

        public PostingProfileModel? GetById(Guid id)
        {
            return _profile.FirstOrDefault(x => x.PostingProfileId == id);
        }

        public void Create(PostingProfileModel model)
        {
            model.PostingProfileId = Guid.NewGuid();
            model.CreatedAt = DateTime.UtcNow;
            _profile.Add(model);
        }

        public void Update(PostingProfileModel model)
        {
            var existing = GetById(model.PostingProfileId);
            if (existing == null) return;

            model.UpdatedAt = DateTime.UtcNow;

            int index = _profile.IndexOf(existing);
            _profile[index] = model;
        }

        public void Delete(Guid id)
        {
            var existing = GetById(id);
            if (existing == null) return;

            _profile.Remove(existing);
        }

        // Status
        public void Activate(Guid id)
        {
            var p = GetById(id);
            if (p == null) return;

            p.IsActive = true;
            p.UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate(Guid id)
        {
            var p = GetById(id);
            if (p == null) return;

            p.IsActive = false;
            p.UpdatedAt = DateTime.UtcNow;
        }

        private List<PostingProfileModel> Seed()
        {
            var seededProfiles = _profileSeed.SeedForCompanies(_companies);
            return seededProfiles;
        }

    }
}
