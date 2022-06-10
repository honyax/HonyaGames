using System.Linq;

public class EnemyPlayer : BasePlayer
{
    public override Stone.Color MyColor { get { return Stone.Color.White; } }


    public override bool TryGetSelected(out int x, out int z)
    {
        var availablePoints = GetAvailablePoints();
        var maxCount = availablePoints.Values.Max();
        foreach (var availablePoint in availablePoints)
        {
            if (availablePoint.Value == maxCount)
            {
                x = availablePoint.Key.Item1;
                z = availablePoint.Key.Item2;
                return true;
            }
        }
        return base.TryGetSelected(out x, out z);
    }
}
