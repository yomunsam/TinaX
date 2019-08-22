namespace TinaX
{
    /// <summary>
    /// TinaX bootstrap interface
    /// </summary>
    public interface IXBootstrap
    {
        /// <summary>
        /// Invoke before framework's systems start.
        /// </summary>
        void OnInit();

        /// <summary>
        /// Invoke after framework's systems start.
        /// </summary>
        void OnStart();

        void OnQuit();

        void OnAppRestart();
    }
}
