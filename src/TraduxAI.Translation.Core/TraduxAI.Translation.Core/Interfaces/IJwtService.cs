using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Services;

namespace TraduxAI.Translation.Core.Interfaces
{
    public interface IJwtService
    {
        TokenAccess GenerateToken(User user);
    }
}
