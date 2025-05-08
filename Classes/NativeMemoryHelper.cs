using System;
using FFXIVClientStructs.FFXIV.Client.System.Memory;

namespace KamiToolKit.Classes;

internal static class NativeMemoryHelper {
    public static unsafe T* UiAlloc<T>(uint elementCount = 1, ulong alignment = 8) where T : unmanaged {
        var allocSize = (ulong) sizeof(T) * elementCount;
        var memory = (T*) IMemorySpace.GetUISpace()->Malloc(allocSize, alignment);
        
        IMemorySpace.Memset(memory, 0, allocSize);

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

    public static unsafe nint Malloc(ulong size, ulong alignment = 8)
        => (nint) IMemorySpace.GetUISpace()->Malloc(size, alignment);
    
    public static unsafe void Free(void* memory, ulong size)
        => IMemorySpace.Free(memory, size);
}