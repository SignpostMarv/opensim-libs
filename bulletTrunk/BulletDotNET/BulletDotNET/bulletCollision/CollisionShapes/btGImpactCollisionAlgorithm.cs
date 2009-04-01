namespace BulletDotNET
{
    public static class btGImpactCollisionAlgorithm
    {
        public static void registerAlgorithm(btCollisionDispatcher dispatcher)
        {
            dispatcher.RegisterGImpact();
        }
    }
}
