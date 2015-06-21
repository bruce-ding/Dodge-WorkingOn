using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DodgeXaml
{
    public class ParticleData : GameObject
    {
        public float BirthTime; // 生成时间
        public float MaxAge; // 最大存活时间
        public Vector2 OrginalPosition; // 初始位置
        public Vector2 Accelaration; // 加速度a
        public Vector2 Direction; // 方向
        public float Scaling; // 缩放
        public Color ModColor; // 变化颜色
    }
}
