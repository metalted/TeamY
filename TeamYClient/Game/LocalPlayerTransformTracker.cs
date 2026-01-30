using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamYClient.Game
{
    public class LocalPlayerTransformTracker : MonoBehaviour
    {
        private float updateInterval = 0.15f;
        private float timer;

        public void Update()
        {
            timer += Time.deltaTime;
            if (timer < updateInterval)
                return;

            timer = 0f;

            Plugin.Instance.LocalPlayerTracker.SetTransform(transform.position, transform.rotation);
        }
    }
}
