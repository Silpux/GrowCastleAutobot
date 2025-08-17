using gca.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca.Classes
{
    public static class Extensions
    {
        public static bool IsDungeon(this Dungeon d) => (d & Dungeon.Dungeons) != 0 && (d & (d - 1)) == 0;
        public static bool IsDragon(this Dungeon d) => (d & Dungeon.Dragons) != 0 && (d & (d - 1)) == 0;
        public static bool IsValidDungeon(this Dungeon d) => (d & Dungeon.Any) != 0 && (d & (d - 1)) == 0;
        public static bool IsMultiple(this Dungeon d) => (d & (d - 1)) != 0;
    }
}
