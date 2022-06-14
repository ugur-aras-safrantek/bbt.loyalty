namespace Bbt.Campaign.Api.Base
{
    public class Authorization
    {
        public static void CheckAuthorization() 
        { 
            List<RoleAuthorization> roleAuthorizationList = new List<RoleAuthorization>();
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1});
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });
            roleAuthorizationList.Add(new RoleAuthorization() { RoleTypeId = 1, ModuleTypeId = 1, AuthorizationTypeId = 1 });

        }
    }
}
