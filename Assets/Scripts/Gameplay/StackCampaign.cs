using System;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Gameplay
{
    public readonly struct StackLevel
    {
        public readonly int Id, Target, Chapter, Goal;
        public readonly float Width, Speed;
        public StackLevel(int id)
        {
            Id=id; Chapter=(id-1)/10; Goal=(id-1)%3;
            Target=6+Mathf.FloorToInt((id-1)*24f/29f);
            Width=3.6f-(id-1)*.018f; Speed=1f+(id-1)*.018f;
        }
        public StackLevel(int target,float width,float speed,int goal)
        {Id=0;Target=target;Width=width;Speed=speed;Goal=goal;Chapter=0;}
    }
    public static class StackCampaign
    {
        public static StackLevel Level(int id)
        { if(id<1 || id>30) throw new ArgumentOutOfRangeException(nameof(id)); return new StackLevel(id); }
        public static int Stars(StackLevel level,StackRun run)
        {
            if(!run.Completed) return 0;
            var ratio=run.PerfectDrops/(float)level.Target;
            if(level.Goal==1)
                return 1+(run.MaxStreak>=3?1:0)+(run.MaxStreak>=Mathf.CeilToInt(level.Target*.65f)?1:0);
            if(level.Goal==2)
            {
                var area=run.Top.Size.x*run.Top.Size.y/(level.Width*level.Width);
                return 1+(area>=.50f?1:0)+(area>=.80f?1:0);
            }
            return 1+(ratio>=.50f?1:0)+(ratio>=.80f?1:0);
        }
        public static int Record(StackProfile profile,StackLevel level,StackRun run)
        {
            if(!run.Completed || run.Endless || run.Target!=level.Target) return 0;
            var stars=Stars(level,run); var old=profile.LevelStars[level.Id-1];
            var bonus=Mathf.Max(0,stars-old)*15+(old==0?25:0);
            profile.LevelStars[level.Id-1]=Mathf.Max(old,stars);
            profile.UnlockedLevel=Mathf.Max(profile.UnlockedLevel,Mathf.Min(30,level.Id+1));
            profile.Coins+=bonus; return bonus;
        }
        public static bool EndlessUnlocked(StackProfile profile) => profile.UnlockedLevel>=6;
        public static bool PowerUnlocked(StackProfile profile,StackPower power) => profile.UnlockedLevel>=(power==StackPower.Slow?3:power==StackPower.Center?5:9);
        public static StackBlockKind Kind(int tier,int count)
        {
            var next=count+1;
            if(tier>=21 && next%7==0)return StackBlockKind.Wind;
            if(tier>=11 && next%4==0)return StackBlockKind.Drift;
            if(tier>=6 && next%5==0)return StackBlockKind.Bonus;
            if(tier>=6 && next%3==0)return StackBlockKind.Fragile;
            return StackBlockKind.Standard;
        }
        public static int Buildings(StackProfile profile) {var count=0;foreach(var stars in profile.LevelStars)if(stars>0)count++;return count;}
        public static StackLevel Daily(DateTime date) => new StackLevel(12+(date.DayOfYear%5),3.3f,1.25f+(date.DayOfYear%3)*.06f,date.DayOfYear%3);
        public static int RecordDaily(StackProfile profile,StackLevel level,StackRun run,string date)
        {
            if(date!=DateTime.UtcNow.ToString("yyyy-MM-dd") || !run.Completed)return 0;
            if(profile.ChallengeDate!=date){profile.ChallengeDate=date;profile.ChallengeStars=0;profile.ChallengeRewarded=false;}
            profile.ChallengeStars=Mathf.Max(profile.ChallengeStars,Stars(level,run));
            if(profile.ChallengeStars<3 || profile.ChallengeRewarded)return 0;
            profile.ChallengeRewarded=true;profile.Coins+=75;return 75;
        }
        public static int StyleCost(int style) => style==0?0:style==1?75:style==2?150:250;
        public static bool SelectStyle(StackProfile profile,int style)
        {
            if(style<0 || style>7) return false;
            Monetization.CommerceRules.Normalize(profile);
            if(style>=4 && (profile.OwnedStyles&(1<<style))==0)return false;
            var bit=1<<style;
            if((profile.OwnedStyles&bit)==0)
            {
                var cost=StyleCost(style); if(profile.Coins<cost) return false;
                profile.Coins-=cost; profile.OwnedStyles|=bit;
            }
            profile.Style=style; return true;
        }
    }
}
