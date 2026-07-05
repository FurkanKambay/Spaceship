namespace FK.Spaceship.Gameplay.CameraEffects
{
    public interface ICameraEffect
    {
        public bool Tick();
        public void Stop();
        public void Cleanup();
    }
}
