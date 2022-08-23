using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avn.Domain.Dtos.Projects
{
    public class ProjectDto
    { 
        public Guid Id { get; set; }
        public Guid? UserId { get; set; } 
        public string Name { get; set; }
        public string SourceIp { get; set; }
        public string Website { get; set; }
        public string ApiKey { get; set; } 
    }
}
