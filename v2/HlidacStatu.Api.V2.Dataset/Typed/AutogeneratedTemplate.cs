﻿using System;
using System.Collections.Generic;
using System.Text;

using static HlidacStatu.Api.V2.Dataset.ClassicTemplate;

namespace HlidacStatu.Api.V2.Dataset.Typed
{
    public  class AutogeneratedTemplate<T>
    {
        ClassicDetailTemplate detail = new ClassicDetailTemplate();
        ClassicSearchResultTemplate search = new ClassicSearchResultTemplate();

        public AutogeneratedTemplate()
        {
            generate();
        }

        public V2.CoreApi.Model.Template Detail() => detail;
        public V2.CoreApi.Model.Template Search() => search;



        private void generate()
        { 

        
        }
    }
}
