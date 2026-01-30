using Shpleeble;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamYClient.Game
{
    public struct LocalPlayerSnapshot
    {
        public Vector3 Position;
        public Vector3 EulerRotation;
        public CharacterMode Mode;
    }
}
