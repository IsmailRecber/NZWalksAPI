using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Diagnostics.Eventing.Reader;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IWalkRepository walkRepository;

        public WalksController(IWalkRepository walkRepository)
        {
            this.walkRepository = walkRepository;
        }


        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
           
                //Map Dto to Domain model
                //  var walkDomainModel = //burada işlemleri yap geçtim şimdilik

                //  await walkRepository.CreateAsync(walkDomainModel);

                //Map or Convert DTO to Domain model
                var walkDomainModel = new Walk
                {
                    Name = addWalkRequestDto.Name,
                    Description = addWalkRequestDto.Description,
                    LenghthInKm = addWalkRequestDto.LenghthInKm,
                    WalkImageUrl = addWalkRequestDto.WalkImageUrl,
                    DifficultyId = addWalkRequestDto.DifficultyId,
                    RegionId = addWalkRequestDto.RegionId,
                };

                //Use Domain Model to Create Walk
                walkDomainModel = await walkRepository.CreateAsync(walkDomainModel);

                //Map Domain Model Back to DTO
                var walksDto = new WalkDto
                {
                    Name = walkDomainModel.Name,
                    Description = walkDomainModel.Description,
                    LenghthInKm = walkDomainModel.LenghthInKm,
                    WalkImageUrl = walkDomainModel.WalkImageUrl,
                    Difficulty = {
                Id = walkDomainModel.DifficultyId,
                Name = walkDomainModel.Difficulty.Name,
                },
                    Region = {
                    Id=walkDomainModel.RegionId,
                    Name=walkDomainModel.Region.Name,
                    Code=walkDomainModel.Region.Code,
                    RegionImageUrl=walkDomainModel.Region.RegionImageUrl,
                     },
                };

                return Ok(walksDto);
            

            
        }
        
        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize=1000)
        {
            var walksDomain = await walkRepository.GetAllAsync(filterOn, filterQuery,sortBy,isAscending ?? true,pageNumber,pageSize);

            if (walksDomain == null || !walksDomain.Any())
            {
                return NotFound("Hiç yürüyüş kaydı bulunamadı.");
            }

            var walksDto = new List<WalkDto>();

            foreach (var walk in walksDomain)
            {
                walksDto.Add(new WalkDto
                {
                    Name = walk.Name,
                    Description = walk.Description,
                    LenghthInKm = walk.LenghthInKm,
                    WalkImageUrl = walk.WalkImageUrl,
                    Difficulty = walk.Difficulty != null ? new DifficultyDto
                    {
                        Id = walk.DifficultyId,
                        Name = walk.Difficulty.Name,
                    } : null,
                    Region = walk.Region != null ? new RegionDto
                    {
                        Id = walk.RegionId,
                        Name = walk.Region.Name,
                        Code = walk.Region.Code,
                        RegionImageUrl = walk.Region.RegionImageUrl,
                    } : null,
                });
            }

            return Ok(walksDto);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.GetByIdAsync(id);


            if (walkDomainModel == null)
            {
                return NotFound();
            }
            var walksDto = new WalkDto();
            walksDto.Name = walkDomainModel.Name;
            walksDto.Description = walkDomainModel.Description;
            walksDto.LenghthInKm = walkDomainModel.LenghthInKm;
            walksDto.WalkImageUrl = walkDomainModel.WalkImageUrl;
            walksDto.Difficulty = walkDomainModel.Difficulty != null ? new DifficultyDto
            {
                Id = walkDomainModel.DifficultyId,
                Name = walkDomainModel.Difficulty.Name,
            } : null;
            walksDto.Region = walkDomainModel.Region != null ? new RegionDto
            {
                Id = walkDomainModel.RegionId,
                Name = walkDomainModel.Region.Name,
                Code = walkDomainModel.Region.Code,
                RegionImageUrl = walkDomainModel.Region.RegionImageUrl,
            } : null;

            return Ok(walksDto);
        }


        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
           

                var walkDomainModel = new Walk();
                {
                    walkDomainModel.Name = updateWalkRequestDto.Name;
                    walkDomainModel.WalkImageUrl = updateWalkRequestDto.WalkImageUrl;
                    walkDomainModel.DifficultyId = updateWalkRequestDto.DifficultyId;
                    walkDomainModel.RegionId = updateWalkRequestDto.RegionId;
                    walkDomainModel.Description = updateWalkRequestDto.Description;
                }

                walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);
                if (walkDomainModel == null)
                {
                    return NotFound();
                }

                var walksDto = new WalkDto();
                walksDto.Name = walkDomainModel.Name;
                walksDto.Description = walkDomainModel.Description;
                walksDto.LenghthInKm = walkDomainModel.LenghthInKm;
                walksDto.WalkImageUrl = walkDomainModel.WalkImageUrl;
                walksDto.Difficulty = walkDomainModel.Difficulty != null ? new DifficultyDto
                {
                    Id = walkDomainModel.DifficultyId,
                    Name = walkDomainModel.Difficulty.Name,
                } : null;
                walksDto.Region = walkDomainModel.Region != null ? new RegionDto
                {
                    Id = walkDomainModel.RegionId,
                    Name = walkDomainModel.Region.Name,
                    Code = walkDomainModel.Region.Code,
                    RegionImageUrl = walkDomainModel.Region.RegionImageUrl,
                } : null;

                return Ok(walksDto);
           

        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var deletedWalkDomainModel=await walkRepository.DeleteAsync(id);
            if (deletedWalkDomainModel == null)
            {
                return NotFound();
            }
            var walksDto = new WalkDto();
            walksDto.Name = deletedWalkDomainModel.Name;
            walksDto.Description = deletedWalkDomainModel.Description;
            walksDto.LenghthInKm = deletedWalkDomainModel.LenghthInKm;
            walksDto.WalkImageUrl = deletedWalkDomainModel.WalkImageUrl;
            walksDto.Difficulty = deletedWalkDomainModel.Difficulty != null ? new DifficultyDto
            {
                Id = deletedWalkDomainModel.DifficultyId,
                Name = deletedWalkDomainModel.Difficulty.Name,
            } : null;
            walksDto.Region = deletedWalkDomainModel.Region != null ? new RegionDto
            {
                Id = deletedWalkDomainModel.RegionId,
                Name = deletedWalkDomainModel.Region.Name,
                Code = deletedWalkDomainModel.Region.Code,
                RegionImageUrl = deletedWalkDomainModel.Region.RegionImageUrl,
            } : null;

            return Ok(walksDto);
        }

    }
}

