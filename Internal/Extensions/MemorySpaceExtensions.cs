using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.Memory;

namespace KamiToolKit.Internal.Extensions;

internal static unsafe class MemorySpaceExtensions {
    extension(ref IMemorySpace memorySpace) {

        public T* MallocZeroed<T>() where T : unmanaged {
            var blockSize = (nuint)sizeof(T);
            var memoryPointer = memorySpace.Malloc<T>();

            NativeMemory.Clear(memoryPointer, blockSize);

            return memoryPointer;
        }

        public T* AllocateZeroedArray<T>(int count) where T : unmanaged {
            var blockSize = (nuint)sizeof(T) * (uint)count;
            var memoryPointer = (T*)memorySpace.Malloc(blockSize, 8);

            NativeMemory.Clear(memoryPointer, blockSize);

            return memoryPointer;
        }

        public T* AllocateZeroedArray<T>(uint count) where T : unmanaged
            => memorySpace.AllocateZeroedArray<T>((int)count);

        public T* Realloc<T>(void* memory, int newCount) where T : unmanaged
            => (T*)memorySpace.AlignedRealloc(memory, (ulong)sizeof(T) * (uint)newCount, 16);

        public T* Realloc<T>(void* memory, uint newCount) where T : unmanaged
            => (T*) memorySpace.AlignedRealloc(memory, (ulong)sizeof(T) * newCount, 16);

        public static void Copy<T>(T* oldBuffer, T* newBuffer, int count) where T : unmanaged
            => NativeMemory.Copy(oldBuffer, newBuffer, (nuint)sizeof(T) * (nuint)count);

        public static void Copy<T>(T* oldBuffer, T* newBuffer, uint count) where T : unmanaged
            => NativeMemory.Copy(oldBuffer, newBuffer, (nuint)sizeof(T) * count);
    }
}
