namespace AspNetCore.Identity.Shared.Constants;

public static class DatabaseTables
{
    public static class Company
    {
        public const string TABLE_NAME = "companies";
        public const string NAME = "name";
    }
    
    public static class Community
    {
        public const string TABLE_NAME = "communities";
        public const string NAME = "name";
        public const string PARENT_COMPANY_ID = "parent_company_id";
    }

    public static class CompanyUsers
    {
        public const string TABLE_NAME = "company_users";
        public const string COMPANY_ID = "company_id";
        public const string USER_ID = "user_id";
        public const string ROLE_ID = "role_id";
    }
    
    public static class CommunityUsers
    {
        public const string TABLE_NAME = "community_users";
        public const string COMMUNITY_ID = "community_id";
        public const string USER_ID = "user_id";
        public const string ROLE_ID = "role_id";
    }
}

public static class Policies
{
    public const string IS_COMPANY_ADMIN = "IsCompanyAdmin";
    public const string IS_COMPANY_USER = "IsCompanyUser";
    public const string IS_SUPER_ADMIN = "IsSuperAdmin";
}