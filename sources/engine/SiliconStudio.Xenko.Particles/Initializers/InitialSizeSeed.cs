﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;

namespace SiliconStudio.Xenko.Particles.Initializers
{
    [DataContract("InitialSizeSeed")]
    [Display("Initial Size by seed")]
    public class InitialSizeSeed : ParticleInitializer
    {
        public InitialSizeSeed()
        {
            RequiredFields.Add(ParticleFields.Size);
            RequiredFields.Add(ParticleFields.RandomSeed);

            InheritLocation = InheritLocation.Scale;
        }

        public unsafe override void Initialize(ParticlePool pool, int startIdx, int endIdx, int maxCapacity)
        {
            if (!pool.FieldExists(ParticleFields.Size) || !pool.FieldExists(ParticleFields.RandomSeed))
                return;

            var sizeField = pool.GetField(ParticleFields.Size);
            var rndField = pool.GetField(ParticleFields.RandomSeed);
            
            var minSize = WorldScale.X * RandomSize.X;
            var sizeGap = WorldScale.X * RandomSize.Y - minSize;

            var i = startIdx;
            while (i != endIdx)
            {
                var particle = pool.FromIndex(i);
                var randSeed = particle.Get(rndField);

                (*((float*)particle[sizeField])) = minSize + sizeGap * randSeed.GetFloat(RandomOffset.Offset1A + SeedOffset);

                i = (i + 1) % maxCapacity;
            }
        }

        [DataMember(8)]
        [Display("Seed offset")]
        public UInt32 SeedOffset { get; set; } = 0;

        [DataMember(30)]
        [Display("Random size")]
        public Vector2 RandomSize { get; set; } = new Vector2(0.5f, 1);
        
    }
}
