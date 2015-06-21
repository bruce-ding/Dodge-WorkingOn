using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticlesSettings;

namespace Particles2DPipelineParticle
{
    public class ParticleSystemSettingsList
    {
        private static ParticleSystemSettings explosionSettings;
        private static ParticleSystemSettings explosionSmokeSettings;
        private static ParticleSystemSettings smokePlumeSettings;
        private static ParticleSystemSettings emitterSettings;
        

        public ParticleSystemSettingsList()
        {
            
            Add();
        }

        public ParticleSystemSettings this[string settingName]
        {
            get
            {
                if (settingName.ToLower() == "explosionSettings".ToLower())
                {
                    return explosionSettings;
                }
                if (settingName.ToLower() == "explosionSmokeSettings".ToLower())
                {
                    return explosionSmokeSettings;
                }
                if (settingName.ToLower() == "emitterSettings".ToLower())
                {
                    return emitterSettings;
                }
                if (settingName.ToLower() == "smokePlumeSettings".ToLower())
                {
                    return smokePlumeSettings;
                }
                else 
                {
                    return null;
                }
            }
            
        }

        public void Add()
        {
            if (explosionSettings == null)
            {
                explosionSettings = new ParticleSystemSettings()
                {
                    MinNumParticles = 10,
                    MaxNumParticles = 12,
                    TextureFilename = "explosion",
                    MinInitialSpeed = 40,
                    MaxInitialSpeed = 500,
                    AccelerationMode = AccelerationMode.EndVelocity,
                    EndVelocity = 0,
                    MinRotationSpeed = -45,
                    MaxRotationSpeed = 45,
                    MinLifetime = 0.5f,
                    MaxLifetime = 1.0f,
                    MinSize = 0.3f,
                    MaxSize = 1.0f,
                    SourceBlend = Blend.SourceAlpha,
                    DestinationBlend = Blend.One
                };
            }

            if (explosionSmokeSettings == null)
            {
                explosionSmokeSettings = new ParticleSystemSettings()
                {
                    MinNumParticles = 5,
                    MaxNumParticles = 10,
                    TextureFilename = "smoke",
                    MinInitialSpeed = 20,
                    MaxInitialSpeed = 200,
                    AccelerationMode = AccelerationMode.Scalar,
                    MinAccelerationScale = -10,
                    MaxAccelerationScale = -50,
                    MinRotationSpeed = -45,
                    MaxRotationSpeed = 45,
                    MinLifetime = 1,
                    MaxLifetime = 2.5f,
                    MinSize = 1,
                    MaxSize = 2
                };
            }

            if (smokePlumeSettings == null)
            {
                smokePlumeSettings = new ParticleSystemSettings()
                {
                    MinNumParticles = 2,
                    MaxNumParticles = 8,
                    TextureFilename = "smoke",
                    MinDirectionAngle = 260,
                    MaxDirectionAngle = 280,
                    MinInitialSpeed = 20,
                    MaxInitialSpeed = 100,
                    AccelerationMode = AccelerationMode.Vector,
                    MinAccelerationVector = new Vector2(10, 0),
                    MaxAccelerationVector = new Vector2(50, 0),
                    MinRotationSpeed = -22.5f,
                    MaxRotationSpeed = 22.5f,
                    MinLifetime = 5,
                    MaxLifetime = 7,
                    MinSize = 0.5f,
                    MaxSize = 1
                };
            }

            if (emitterSettings == null)
            {
                emitterSettings = new ParticleSystemSettings()
                {
                    MinNumParticles = 1,
                    MaxNumParticles = 5,
                    TextureFilename = "BlockParticle",
                    MinDirectionAngle = 260,
                    MaxDirectionAngle = 280,
                    MinInitialSpeed = 20,
                    MaxInitialSpeed = 200,
                    AccelerationMode = AccelerationMode.None,
                    MinRotationSpeed = -45,
                    MaxRotationSpeed = 45,
                    MinLifetime = 1,
                    MaxLifetime = 2.5f,
                    MinSize = 0.5f,
                    MaxSize = 1,
                    Gravity = new Vector2(0, 300)
                };
            }

        }

    }
}
