﻿using System;
using FFXIVClientStructs.FFXIV.Client.System.Memory;

namespace KamiToolKit.Classes;

internal static class NativeMemoryHelper {
    public static unsafe T* UiAlloc<T>(uint elementCount = 1) where T : unmanaged {
        var memory = (T*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(T) * elementCount, 8);

        if (memory is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        return memory;
    }

    public static unsafe void UiFree<T>(T* memory) where T : unmanaged 
        => IMemorySpace.Free(memory);

    public static unsafe void UiFree<T>(T* memory, uint elementCount) where T : unmanaged
        => IMemorySpace.Free(memory, (ulong) sizeof(T) * elementCount);

    public static unsafe T* Create<T>() where T : unmanaged, ICreatable {
        var memory = IMemorySpace.GetUISpace()->Create<T>();

        if (memory is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        return memory;
    }
}