namespace Exchange.Common
{
    public enum CommandTypeEnum
    {
        None,
        RequestPrice,
        ResponsePrice,
        RequestOrder,
        RequestOrderCancel,
        ResponseOrderSaved,
        ResponseOrderCanceled,
        Error,
        Exit
    }
}