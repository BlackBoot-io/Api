using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avn.Domain.Dtos.ApiKey;

public class ApiKeyDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; }
}
