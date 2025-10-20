namespace StudyAndChill.API.Enums
{
    [Flags]
    public enum AvailabilityType
    {
        None = 0,
        Regular = 1,
        MakeUp = 2,
        Both = Regular | MakeUp
    }
}