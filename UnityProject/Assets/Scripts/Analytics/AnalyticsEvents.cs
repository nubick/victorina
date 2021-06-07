using Assets.Scripts.Utils;

namespace Victorina
{
    public static class AnalyticsEvents
    {
        public static GameEvent<string> SavePackageAsArchive { get; } = new GameEvent<string>();
        public static GameEvent CrafterOpen { get; } = new GameEvent();
        public static GameEvent<string> LoadPackageToPlay { get; } = new GameEvent<string>();
        public static GameEvent<int> FirstRoundQuestionStart { get; } = new GameEvent<int>();
        public static GameEvent<int> LastRoundQuestionStart { get; } = new GameEvent<int>();
    }
}