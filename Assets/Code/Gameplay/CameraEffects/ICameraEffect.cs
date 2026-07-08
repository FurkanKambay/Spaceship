namespace FK.Spaceship.Gameplay.CameraEffects
{
    public interface ICameraEffect
    {
        bool IsEnabled { get; set; }
        public bool Tick();
        public void Reset();
        public void ForceStop();
    }
}
