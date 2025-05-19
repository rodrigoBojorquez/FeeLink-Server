namespace FeeLink.Api.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class RequiredRolesAttribute(string[] roles) : Attribute
{
    public readonly string[] Roles = roles;
}