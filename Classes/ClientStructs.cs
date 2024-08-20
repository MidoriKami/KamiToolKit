namespace KamiToolKit.Classes;

public unsafe class ClientStructs {
    public static ClientStructs Instance => internalInstance ??= new ClientStructs();
    private static ClientStructs? internalInstance;
}