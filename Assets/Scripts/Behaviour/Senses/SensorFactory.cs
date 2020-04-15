
public static class SensorFactory
{
    public static AbstractSensor SightSensor(float sightLength, float horisontalFOV, float verticalFOV)
    {
        return new AreaSensor(sightLength, horisontalFOV, verticalFOV, false, SensorType.SIGHT);
    }

    public static AbstractSensor SmellSensor(float senseRadius)
    {
        return new AreaSensor(senseRadius, 360, 360, false, SensorType.SMELL);
    }

    public static AbstractSensor TouchSensor(float senseRadius)
    {
        return new AreaSensor(senseRadius, 360, 360, false, SensorType.TOUCH);
    }
}
