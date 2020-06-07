using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX;
using TinaX.Services;

namespace Nekonya
{
    public class Startup : MonoBehaviour
    {
        private async void Start()
        {
            await XCore.New()
                .UseVFS()
                .RunAsync();
        }
    }
}

