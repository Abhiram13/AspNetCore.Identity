using AspNetCore.Identity.Features.Company.Repository;

namespace AspNetCore.Identity.Features.Company.Services;

public sealed class CompanyService
{
    private readonly CompanyRepository _companyRepository;
    
    public CompanyService(CompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task CreateCompanyAsync(string companyName)
    {
        await _companyRepository.CreateOneCompanyAsync(companyName);
    }

    public async Task<IReadOnlyList<Entities.Company>> GetAllCompaniesAsync()
    {
        return await _companyRepository.GetAllCompaniesAsync();
    }
}