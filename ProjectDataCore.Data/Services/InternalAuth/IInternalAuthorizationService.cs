using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.InternalAuth;
public interface IInternalAuthorizationService
{
    public void RegisterToken(string token, InternalAuthorizationType scope);
    public void UnregisterToken(string token);
    public bool CheckToken(string token, InternalAuthorizationType scope);
}

public enum InternalAuthorizationType
{
    ImageUpload
}