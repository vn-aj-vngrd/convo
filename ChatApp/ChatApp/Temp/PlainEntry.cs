﻿using Xamarin.Forms;

namespace PlainEntryAndroidSample.CustomRenderers
{
    public class PlainEntry : RoutingEffect
    {
        public PlainEntry() : base("PlainEntryGroup.PlainEntryEffect")
        {
        }
    }
}

/*
    <Entry>
        <Entry.Effects>
            <effects:PlainEntry />
        </Entry.Effects>
    </Entry>
 */