/*
 * Copyright (C) 2020 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class GlossaryHit
    {
        private readonly String _term;
        private readonly int _begin;
        private readonly int _end;

        public GlossaryHit(String term, int begin, int end)
        {
            _term = term;
            _begin = begin;
            _end = end;
        }

        public String getTerm()
        {
            return _term;
        }

        public int getBegin()
        {
            return _begin;
        }

        public int getEnd()
        {
            return _end;
        }
    }
}
