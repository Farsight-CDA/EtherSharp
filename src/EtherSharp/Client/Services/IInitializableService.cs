﻿namespace EtherSharp.Client.Services;
internal interface IInitializableService
{
    public ValueTask InitializeAsync();
}