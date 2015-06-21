using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeXaml.Levels
{
    public class LevelData
    {
        
        public int Number; // 敌人数量
        public int Health; // 玩家生命
        public int ScoresToWin; // 得到多少分数过关
        public TimeSpan MinWinTime; // 纪录过关的最少时间
    }
}
