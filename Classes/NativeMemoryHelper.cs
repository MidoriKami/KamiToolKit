using System;
using FFXIVClientStructs.FFXIV.Client.System.Memory;

namespace KamiToolKit.Classes;

public static class NativeMemoryHelper {
    public static unsafe T* UiAlloc<T>() where T : unmanaged {
        var memory = (T*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(T), 8);

        if (memory is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        return memory;
    }

    public static unsafe void UiFree<T>(T* memory) where T : unmanaged 
        => IMemorySpace.Free(memory);
}