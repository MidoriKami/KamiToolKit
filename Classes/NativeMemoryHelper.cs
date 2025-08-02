using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.Memory;

namespace KamiToolKit.Classes;

internal static class NativeMemoryHelper {
    public static unsafe T* UiAlloc<T>(int elementCount, ulong alignment = 8) where T : unmanaged
        => UiAlloc<T>((uint)elementCount, alignment);

    public static unsafe T* UiAlloc<T>(uint elementCount = 1, ulong alignment = 8) where T : unmanaged {
        var allocSize = (ulong)sizeof(T) * elementCount;
        var memory = (T*)IMemorySpace.GetUISpace()->Malloc(allocSize, alignment);

        IMemorySpace.Memset(memory, 0, allocSize);

        if (memory is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        return memory;
    }

    public static unsafe void UiFree<T>(T* memory) where T : unmanaged
        => IMemorySpace.Free(memory);

    public static unsafe void UiFree<T>(T* memory, uint elementCount) where T : unmanaged
        => IMemorySpace.Free(memory, (ulong)sizeof(T) * elementCount);

    public static unsafe T* Create<T>() where T : unmanaged, ICreatable {
        var memory = IMemorySpace.GetUISpace()->Create<T>();

        if (memory is null) {
            throw new Exception($"Unable to allocate memory for {typeof(T)}");
        }

        return memory;
    }

    public static unsafe nint Malloc(ulong size, ulong alignment = 8)
        => (nint)IMemorySpace.GetUISpace()->Malloc(size, alignment);

    public static unsafe void Free(void* memory, ulong size)
        => IMemorySpace.Free(memory, size);

    public static unsafe void ResizeArray<T>(ref T* array, int oldSize, uint newSize) where T : unmanaged
        => ResizeArray(ref array, oldSize, (int)newSize);

    public static unsafe void ResizeArray<T>(ref T* array, uint oldSize, uint newSize) where T : unmanaged
        => ResizeArray(ref array, (int)oldSize, (int)newSize);

    public static unsafe void ResizeArray<T>(ref T* array, uint oldSize, int newSize) where T : unmanaged
        => ResizeArray(ref array, (int)oldSize, newSize);

    public static unsafe void ResizeArray<T>(ref T* array, int oldSize, int newSize) where T : unmanaged {
        var newBuffer = UiAlloc<T>((uint)newSize);

        Copy(array, newBuffer, oldSize);

        if (array is not null) {
            UiFree(array, (uint)oldSize);
        }

        array = newBuffer;
    }

    public static unsafe void Copy<T>(T* oldBuffer, T* newBuffer, int count) where T : unmanaged
        => Copy(oldBuffer, newBuffer, (uint)count);

    public static unsafe void Copy<T>(T* oldBuffer, T* newBuffer, uint count) where T : unmanaged
        => NativeMemory.Copy(oldBuffer, newBuffer, (nuint)(sizeof(T) * count));

    public static unsafe void MemCopy<T>(T* oldBuffer, T* newBuffer, uint byteCount) where T : unmanaged
        => NativeMemory.Copy(oldBuffer, newBuffer, byteCount);
}
