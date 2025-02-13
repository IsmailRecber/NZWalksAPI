﻿namespace NZWalks.API.Models.DTO
{
    public class WalkDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public double LenghthInKm { get; set; }

        public string? WalkImageUrl { get; set; }


        public RegionDto Region { get; set; }

        public DifficultyDto Difficulty { get; set; }
    }
}
