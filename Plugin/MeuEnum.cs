﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    public static class MeuEnum
    {
        public enum Stage
        {
            PreValidation =10,
            PreOperation=20,
            PosOperation =40
        }

        public enum Mode
        {
            Asynchronous =1,
            Synchronous =0
        }
    }
}
