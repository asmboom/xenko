﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core.Mathematics;

namespace SiliconStudio.Xenko.Particles
{
    public static class RandomOffset
    {
        /// <summary>
        /// Lifetime offset should always be 0 so that it can easily be retrieved from the random seed.
        /// </summary>
        public const UInt32 Lifetime = 0;

        /// <summary>
        /// Random seed offset used for coupling 1-dimensional random values
        /// </summary>
        public const UInt32 Offset1A = 1112;

        /// <summary>
        /// Random seed offset used for coupling 2-dimensional random values
        /// </summary>
        public const UInt32 Offset2A = 2223;
        public const UInt32 Offset2B = 3334;

        /// <summary>
        /// Random seed offset used for coupling 3-dimensional random values
        /// </summary>
        public const UInt32 Offset3A = 4445;
        public const UInt32 Offset3B = 5556;
        public const UInt32 Offset3C = 6667;

    }

    public class ParticleRandomSeedGenerator
    {
        private UInt32 rngSeed;

        public ParticleRandomSeedGenerator(UInt32 seed)
        {
            rngSeed = seed;
        }

        public double GetNextDouble() => GetNextSeed().GetDouble(0);

        public RandomSeed GetNextSeed()
        {
            return new RandomSeed(unchecked(rngSeed++)); // We want it to overflow
        }
    }
}
