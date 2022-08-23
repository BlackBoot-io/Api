﻿using Avn.Domain.Dtos.Projects;
using Avn.Domain.Entities;
using Avn.Services.Interfaces;

namespace Avn.Services.Implementations
{
    public class ProjectService : IProjectService
    {

        private readonly ApplicationDBContext _context;
        private readonly DbSet<Project> _project;

        public ProjectService(ApplicationDBContext context)
        {
            _context = context;
            _project = context.Set<Project>();
        }

        public async Task<IActionResponse<IEnumerable<Project>>> GetAllAsync(Guid userid, CancellationToken cancellationToken = default)
       => new ActionResponse<IEnumerable<Project>>(await _project.Where(X => X.UserId == userid) 
           .AsNoTracking()
           .ToListAsync());

        public async Task<IActionResponse<Project>> CreateAsync(Project item, CancellationToken cancellationToken = default)
        {
            var project = new Project(); 
            project.Name = item.Name;
            project.SourceIp = item.SourceIp;
            project.Website = item.Website;
            project.ApiKey = (new Guid()).ToString(); //TODO check GUID DataType


            await _context.AddAsync(project);
            var dbResult = await _context.SaveChangesAsync();

            if (!dbResult.ToSaveChangeResult())
                return new ActionResponse<Project>(ActionResponseStatusCode.ServerError, AppResource.AddProjectOperationFail);


            return new ActionResponse<Project>(new Project
            {
                Name = project.Name,
                SourceIp = project.SourceIp,
                UserId = project.UserId,
                Website = project.Website,
                ApiKey = project.ApiKey,
            });  
            
        }

        public async Task<IActionResponse<Project>> UpdateAsync(Project item, CancellationToken cancellationToken = default)
        {
            var project = await _project.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (project == null)  
                return new ActionResponse<Project>(  ActionResponseStatusCode.NotFound, AppResource.RecordNotFound);
            
            project.Name = item.Name; 
            project.Website=item.Website;
            project.SourceIp = item.SourceIp;
            project.ApiKey = item.ApiKey;

            await UpdateAsync(project);
            var dbResult = await _context.SaveChangesAsync();

            if (!dbResult.ToSaveChangeResult())
                return new ActionResponse<Project>(ActionResponseStatusCode.ServerError, AppResource.AddProjectOperationFail);

            return new ActionResponse<Project>(new Project
            {
                Name = project.Name,
                SourceIp = project.SourceIp,
                UserId = project.UserId,
                Website = project.Website,
                ApiKey = project.ApiKey,
            });
        }
    }
}
